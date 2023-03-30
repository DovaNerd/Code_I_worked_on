using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
/*
 * Welcome to the playerSM this is script that the player will be using in order to actually interact with the states individually it inherits
 * from the StateMachine parent class that has the definitions for how our functions actually change states, what the playerSm actually does is store
 * relevant information that its states may need as well as what states are included in this specific statemachine.
 */
public class PlayerSM : StateMachine
{
    [SerializeField] private bool _grounded;
    private float dragForce;
    private bool startDrag;
    private ParticleSystem dust;
    private bool swapReady;
    private float dashValue = 70;
    private float dashTotalTime = 0.15f;
    private float dashGravTime = 0.1f;
    private float playerDirectionValue = 0;
    [HideInInspector] private float inputx;
    [HideInInspector] private float inputy;
    [HideInInspector] public bool dashReady = true;
    [HideInInspector] public float dashCooldown = 1.3f;
    public bool magicReady = true;
    [SerializeField] public float magicCooldown = 0.5f;
    //public bool dashCheck;
    public bool dashed;
    //Creation of state variables for changing between states
    [HideInInspector]
    public Idle idleState;
    [HideInInspector]
    public Moving movingState;
    [HideInInspector]
    public Jumping jumpingState;
    [HideInInspector]
    public Falling fallingState;
    [HideInInspector]
    public WallSlide wallslideState;
    [HideInInspector]
    public WallJump walljumpState;
    [HideInInspector]
    public Dashing dashingState;
    [HideInInspector]
    public Heal healState;
    [HideInInspector]
    public Projectile projectileState;
    [HideInInspector]
    public Attack attackState;
    [HideInInspector]
    public Menu menuState;
    [HideInInspector]
    public Damage damageState;
    [HideInInspector]
    public Death deathState;


    //Lists containing what general state certain states inherit from
    public List<BaseState> airStates = new List<BaseState>();
    public List<BaseState> groundedStates = new List<BaseState>();

    //These bools handle the transitions to certain states based off of button presses
    public bool jumpAction;
    public bool dashAction;
    public bool healAction;
    public bool projectileAction;
    public bool attackAction;
    public bool holdWall;

    //These bools are for utility so for things like whether or not the player is currently in a menu
    public bool menuAction;
    public GameObject menuObj;

    //Movement related Variables
    [HideInInspector]
    public Vector2 moveinput;
    [HideInInspector]
    public Vector2 movement;
    [HideInInspector]
    public float speed = 6.0f;
    public Rigidbody2D characterRigidbody;
    public Collider2D lCollider;
    public Collider2D rCollider;
    public Collider2D dCollider;

    //Dash effect Variables 
    public float distanceBetweenImages;
    public float lastImageXpos;
    
    

    //Character component variables that store references to othe components on the player
    public Player_Stats _pStats;

    //This is where I have initialised the variables we are going to be using for Attacking
    public Transform _attackPoint;
    public GameObject _projectile;
    public LayerMask _enemyLayer;

    //Animator reference and variables
    public Animator anim;
    public Transform playerAnimation;

    //The players main collider and a reference to the player stats
    public Player_Stats playerStats;
    public CapsuleCollider2D PlayerCollider;

    // Reference to the Tpose player prefab
    public GameObject playerTpose;

    // Variable for checking if the damage I frames are done
    public bool damageStop;


    // Storing the players dashing collider
    public CapsuleCollider2D dashCol;

    public ParticleSystem dashParticle;

    #region Movement callback contexts
    /*
     * All the below functions work with the unity input manager in order to determine what button the player is currently pressing
     * Currently we only use them to store what action the player is currently pressing to handle state transitions we can also do 
     * this however by getting specific actions but this is much easier for getting if the button is pressed or not pressed since it
     * updates both ways.
     */

    public void OnMove(InputAction.CallbackContext context)
    {
        moveinput = context.ReadValue<Vector2>(); 
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumpAction = context.performed;
    }

    public void OnSpecial(InputAction.CallbackContext context)
    {
        //dashCheck = context.performed;//for drag 

        if (dashReady)//if able to dash
        {
            dashAction = true;
        }
    }
    
    public void OnMagic(InputAction.CallbackContext context)
    {
        if(magicReady)
        {
            if (context.duration < 1.2 && context.performed && GetComponent<Player_Stats>().GetMana() >= 7)
            {
                GetComponent<Player_Stats>().RangedAttack();
                projectileAction = context.performed;
            }
            else if (context.duration >= 1.2f && context.performed && GetComponent<Player_Stats>().GetMana() >= 10)
            {
                //GetComponent<Player_Stats>().heal();
                healAction = context.performed;
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        attackAction = context.performed;
    }

    public void OnWallHold(InputAction.CallbackContext context)
    {
        if (playerStats.GetWallHangUpgrade() == true)
        holdWall = context.performed;
    }

    #endregion
    private void Awake()
    {
        swapReady = false;//not swaping movement sides
        dust = GameObject.Find("DustPS").GetComponent<ParticleSystem>();//sets up the dust particle system

        //Initialisation of states that this statemachine will be using
        idleState = new Idle(this);
        movingState = new Moving(this);
        jumpingState = new Jumping(this);
        fallingState = new Falling(this);
        wallslideState = new WallSlide(this);
        walljumpState = new WallJump(this);
        dashingState = new Dashing(this);
        healState = new Heal(this);
        projectileState = new Projectile(this);
        attackState = new Attack(this);
        menuState = new Menu(this);
        damageState = new Damage(this);
        deathState = new Death(this);

        //Adding states to their proper list to demonstrate their general state if they have one
        groundedStates.Add(idleState);
        groundedStates.Add(movingState);

        airStates.Add(jumpingState);
        airStates.Add(fallingState);
        airStates.Add(wallslideState);
        airStates.Add(walljumpState);

        //assigning references to componenets

    }

    public void FixedUpdate()
    {
        //ground check, checks the ground layer
        if (characterRigidbody.velocity.y < Mathf.Epsilon && dCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            _grounded = true;
        }
        else
        {
            _grounded = false;
        }

        if (startDrag && dragForce > 0.5) // decreases drag over time when triggered
        {
            dragForce -= Time.deltaTime * 10;
        }

        //code for applying drag
        if (moveinput.x == 0 && _grounded && !dashAction)//if not moving, are grounded and not dashing
        {
            dragForce = 10;// set the drag to 7
            startDrag = false; //reset bool
        }
        else if (_grounded && !dashAction)//if moving, grounded and not dashing, start reducing drag
        {
            startDrag = true;//trigger timer bool
            characterRigidbody.drag = dragForce; //set drag to new value
        }
        else // otherwise reset all variables and set no drag
        {
            characterRigidbody.drag = 0; //reset drag
            dragForce = 7; //drag starts at 7
            startDrag = false; //reset bool
        }
       
        
    }

    //This is a function that allows us to set up what the intial state in this statemachine will be
    protected override BaseState GetInitialState()
    {
        return idleState;
    }

    //This function allows us to get whatever the previous state was in-case we would like to switch back to it
    public BaseState GetPreviousState()
    {
        return previousState;
    }

    //This function is to streamline the playing of animations in the animator we pass through the name in and set it up in the animator
    public void AnimationPlay(string newAnim)
    {
        anim.Play(newAnim);
    }

    #region Movement functions
    /* 
     * Contained in here are functions that certain states may need to handle specific movement elements such as moving in general or something like jumping
     */

    


    //This is a basic movement function that when called will continually update the players position on the x based off whether or not they are holding A or D
    public void Move()
    {
        if (_grounded) 
        {
            createDust();
        }
        inputx = moveinput.x;

        if (inputx > 0)//set up so only left or right whole values
        {
            inputx = 1;
        }
        else if (inputx < 0)
        {
            inputx = -1;
        }
        characterRigidbody.velocity = (new Vector2((inputx * speed), characterRigidbody.velocity.y));
    }

    //This function will allow the player to jump whenever its called in code
    public void Jump()
    {
        createDust(); //creates a particle effect
            characterRigidbody.velocity = new Vector2(characterRigidbody.velocity.x, 0);
            characterRigidbody.AddForce(new Vector2(0, 13), ForceMode2D.Impulse);
    }

    /*
     * We have this function in order to allow our states to call coroutines since they don't inherit from monobehavior and therfore cannot call
     * the coroutines themselves
     */
    public void DashCoroutine()
    {
        StartCoroutine(DashCooldown());//starts the cooldown for another dash
        StartCoroutine(Dash());  //actually dashes
    }
    
    public void MagicCoroutine()
    {
        StartCoroutine(MagicCooldown());
    }

    /*
     * This code will allow create a simple projectile that travels based off of the direction the player is holding (WASD) it generates the
     * projectile prefab assigned to this script.
     */
    public void Projectile()
    {
        inputy = moveinput.y;
        if (inputy > 0)//set up so only left or right whole values
        {
            inputx = 1;
        }
        else if (inputy < 0)
        {
            inputy = -1;
        }

        switch (inputy)
        {
            case 1:
                _attackPoint.localPosition = new Vector3(0, 2 * inputy, 0);
                break;
            case -1:
                _attackPoint.localPosition = new Vector3(0, 2 * inputy, 0);
                break;
            default:
                _attackPoint.localPosition = new Vector3(2 * playerDirectionValue, 0, 0);
                break;
        }
        
        GameObject projectile = Instantiate(_projectile, _attackPoint.position, Quaternion.identity);
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
        if (moveinput.y != 0) // If the player is holding either up or down
        {
            projectileRB.AddForce(new Vector3(0, 20 * moveinput.y, 0) * 50);
            characterRigidbody.velocity = Vector3.zero;
            CinemachineShake.Instance.ShakeCamera(2f, 1);
            characterRigidbody.AddForce(new Vector3(0, (7.5f * (moveinput.y * -1)), 0),ForceMode2D.Impulse);
        }
        else if (playerDirectionValue > 0)//if last moving right add corresponding force
        {
            projectileRB.AddForce(new Vector3(20.0f * playerDirectionValue, 20.0f * moveinput.y, 0.0f) * 50);
        }
        else if (playerDirectionValue < 0) // if last moving left
        {
            projectileRB.AddForce(new Vector3(20.0f * playerDirectionValue, 20.0f * moveinput.y, 0.0f) * 50);
        }
        else //if player is holding direction
        {
            projectileRB.AddForce(new Vector3(20.0f * moveinput.x, 20.0f * moveinput.y, 0.0f) * 50);
        }
    }

    /*
     * Here is the Dash coroutine that allows the player to dash either to the left or the right based off input
     */
    IEnumerator Dash()
    {
        
        float startTime = Time.time;
        characterRigidbody.velocity = Vector2.zero;
        characterRigidbody.gravityScale = 0.0f;

        
        if (playerDirectionValue > 0)//if last moving right add corresponding force
        {
            characterRigidbody.AddForce(new Vector2(dashValue, 0), ForceMode2D.Impulse);
        }
        else // if last moving left
        {
            characterRigidbody.AddForce(new Vector2(-dashValue, 0), ForceMode2D.Impulse);
        }

        while (Time.time < startTime + dashTotalTime)//while time is less ten the total dash time
        {
            if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages) // If the Image is a certain distance from the player
            {
                AfterImagePool.Instance.GetFromPool(); // Get an object refence from the pool
                lastImageXpos = transform.position.x; // Setting up the position of the new image
            }
            if (Time.time >= startTime + dashGravTime)//turns off the dash
            {
                characterRigidbody.velocity = new Vector2(0,characterRigidbody.velocity.y);
                characterRigidbody.gravityScale = 2;
                dashed = true;
                StopCoroutine(Dash());
            }
            yield return null;
        }
        

    }
    IEnumerator DashCooldown()//the timer for dash cooldown
    {
        yield return new WaitForSeconds(dashCooldown); //wait for time
        dashParticle.Play();
        dashReady = true;                             
    }

    IEnumerator MagicCooldown()//the timer for dash cooldown
    {
        yield return new WaitForSeconds(magicCooldown); //wait for time
        magicReady = true;
    }

    public IEnumerator Damage()
    {
        float flashInterval = 0.08333f;
        Physics2D.IgnoreLayerCollision(7, 3, true);

        for (int i = 0; i < 5; i++)
        {
            playerTpose.SetActive(false);
            yield return new WaitForSeconds(flashInterval);
            playerTpose.SetActive(true);
            yield return new WaitForSeconds(flashInterval);
        }

        damageStop = true;
        Physics2D.IgnoreLayerCollision(7, 3, false);
        
    }

    //We use this function to setermine if the animation should be mirrored if the player is going the oposite direction
    public void AnimationMirror()
    {
        if (moveinput.x != 0 && swapReady)//when changing directions
        {
            createDust();//plays dust affect
            swapReady = false;//so it dosent constantly play
        }

        if (moveinput.x != 0)
        {
            Vector3 currentScale = playerAnimation.localScale;
            playerDirectionValue = inputx; //keeps it 1 or -1
            currentScale.z = playerDirectionValue; //sets the direction
            playerAnimation.localScale = currentScale;
        }
        else
        {
            swapReady = true;//ready for a side swap
        }

    }

    //This function allows for other pieces of code to send the state machine to the menu state with a reference to the the menu that gets activated so it knows when to return to the previous state
    public void MenuTransition(GameObject objectRef)
    {
        if (menuAction == false)
        {
            menuObj = objectRef;
            menuAction = true;
        }
        else
        {
            menuObj = objectRef;
            menuAction = false;
        }
    }

    public void createDust()
    {
        dust.Play();//plays dust particles
    }
    #endregion

    #region Geters
    public float GetPlayerDirectionValue()
    {
        return playerDirectionValue;
    }

    public float GetInputx()
    {
        return inputx;
    }
    #endregion
}
