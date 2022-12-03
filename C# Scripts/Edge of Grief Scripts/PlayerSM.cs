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
    /*
    [HideInInspector]
    
    */
    //Lists containing what general state certain states inherit from
    public List<BaseState> airStates = new List<BaseState>();
    public List<BaseState> groundedStates = new List<BaseState>();

    //These bools handle the transitions to certain states based off of button presses
    public bool jumpAction;
    public bool dashAction;
    public bool healAction;
    public bool projectileAction;
    public bool attackAction;


    //Movement related Variables
    [HideInInspector]
    public Vector2 moveinput;
    [HideInInspector]
    public Vector2 movement;
    [HideInInspector]
    public float speed = 6.0f;
    public Rigidbody2D characterbody;
    public Collider2D lCollider;
    public Collider2D rCollider;
    public Collider2D dCollider;
    [HideInInspector]
    public bool dashed;
    [HideInInspector]
    public int dashes = 0;

    //Character component variables that store references to othe components on the player
    public Player_Stats _pStats;

    //This is where I have initialised the variables we are going to be using for Attacking
    public Transform _attackPoint;
    public GameObject _projectile;
    public LayerMask _enemyLayer;

    //Animator reference and variables
    public Animator anim;
    public Transform playerAnimation;

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
        dashAction = context.performed;
    }
    
    public void OnMagic(InputAction.CallbackContext context)
    {
        if (context.duration < 1.2 && context.performed)
        {
            projectileAction = true;
        }
        if (context.duration >= 1.2f && context.performed)
        {
            healAction = true;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        attackAction = context.performed;
    }
    #endregion
    private void Awake()
    {
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

        //Adding states to their proper list to demonstrate their general state if they have one
        groundedStates.Add(idleState);
        groundedStates.Add(movingState);

        airStates.Add(jumpingState);
        airStates.Add(fallingState);
        airStates.Add(wallslideState);
        airStates.Add(walljumpState);

        //assigning references to componenets

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

    #region Movement functions
    /* 
     * Contained in here are functions that certain states may need to handle specific movement elements such as moving in general or something like jumping
     */

    //This is a basic movement function that when called will continually update the players position on the x based off whether or not they are holding A or D
    public void Move()
    {
        float inputx = moveinput.x;
        movement = new Vector2(inputx * speed, 0);

        movement *= Time.deltaTime;

        transform.Translate(movement);
    }

    //This function will allow the player to jump whenever its called in code
    public void Jump()
    {
        characterbody.velocity = new Vector2(characterbody.velocity.x, 0);
        characterbody.AddForce(new Vector2(0, 13), ForceMode2D.Impulse);
    }

    /*
     * We have this function in order to allow our states to call coroutines since they don't inherit from monobehavior and therfore cannot call
     * the coroutines themselves
     */
    public void DashCoroutine()
    {
        StartCoroutine(Dash());
    }

    /*
     * This code will allow create a simple projectile that travels based off of the direction the player is holding (WASD) it generates the
     * projectile prefab assigned to this script.
     */
    public void Projectile()
    {
        switch (moveinput.y)
        {
            case 0:
                _attackPoint.localPosition = new Vector3(2 * moveinput.x, 0, 0);
                break;
            case 1:
                _attackPoint.localPosition = new Vector3(0, 2 * moveinput.y, 0);
                break;
            case -1:
                _attackPoint.localPosition = new Vector3(0, 2 * moveinput.y, 0);
                break;
            default:
                _attackPoint.localPosition = new Vector3(2 * 1, 0, 0);
                break;
        }
        GameObject projectile = Instantiate(_projectile, _attackPoint.position, Quaternion.identity);
        Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
        projectileRB.AddForce(new Vector3(10.0f * moveinput.x, 10.0f * moveinput.y, 0.0f) * 50);
    }

    /*
     * Here is the Dash coroutine that allows the player to dash either to the left or the right based off input
     */
    IEnumerator Dash()
    {
        float startTime = Time.time;
        characterbody.velocity = Vector2.zero;
        characterbody.gravityScale = 0.0f;
        if (moveinput.x > 0)
        {
            characterbody.AddForce(new Vector2(12.5f, 0), ForceMode2D.Impulse);
        }
        else
        {
            characterbody.AddForce(new Vector2(-12.5f, 0), ForceMode2D.Impulse);
        }

        while (Time.time < startTime + 1.0f)
        {
            if (Time.time >= startTime + 0.5f)
            {
                characterbody.velocity = new Vector2(0,characterbody.velocity.y);
                characterbody.gravityScale = 2;
                dashed = true;
                StopCoroutine(Dash());
            }
            yield return null;
        }
    }

    //We use this function to setermine if the animation should be mirrored if the player is going the oposite direction
    public void AnimationMirror()
    {
        if (moveinput.x != 0)
        {
            Vector3 currentScale = playerAnimation.localScale;
            currentScale.z = moveinput.x;
            playerAnimation.localScale = currentScale;
        }
    }
    #endregion
}
