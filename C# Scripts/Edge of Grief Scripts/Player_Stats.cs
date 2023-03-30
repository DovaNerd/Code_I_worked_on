using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player_Stats : MonoBehaviour
{
    private Slider healthBar;//holds the healthbar slider
    private Slider manaBar; //holds the manabar slider
    [SerializeField] private float mana = 20; //mana value
    [SerializeField] private float maxMana; // max mana you can have
    [SerializeField] private float health = 10; //health value
    [SerializeField] private int Maxhealth; // max health you can have

    [SerializeField] private int damage;
    [SerializeField] private int dGems = 1;
    [SerializeField] private int hGems = 1;

    //This variable will store the last campfire the players has touched so when they die they will respawn there instead of at the start
    [SerializeField] private Vector3 checkpoint;

    //These variables will store what powerup the player has picked and and which ones he has buffed
    [SerializeField] private bool dashUpgrade;
    [SerializeField] private bool wallHangUpgrade;

    // Reference to the Tpose player prefab
    public GameObject playerTpose;

    public CapsuleCollider2D playerCollider;

    //Refernece to the player statemachine
    private PlayerSM _playerSM;


    private bool regenHealth = false;
    public void Start()
    {

        healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        manaBar = GameObject.Find("ManaBar").GetComponent<Slider>();
        playerCollider = this.GetComponent<CapsuleCollider2D>();
        _playerSM = this.GetComponent<PlayerSM>();
        // SetMaxHealth(Maxhealth, health); //sets the max health
        healthBar.value = Maxhealth; //initalizes the healthbar to max health
        checkpoint = this.transform.position;
    }

    public void Update()
    {
        if (mana < maxMana) // if mana is not full
        {
            StartCoroutine(RegenerateMana());//start regenerating it
        }
        else
        {
            StopCoroutine(RegenerateMana());//Stop regenerating mana
        }

        if (regenHealth)//if regenerate health is true
        {
            if (health < Maxhealth) // if health is not full
            {
                StartCoroutine(RegenerateHealth());//start regenerating health
            }
            else
            {
                regenHealth = false;//sets regen to false
                StopCoroutine(RegenerateHealth());//Stop regenerating health
                health = Maxhealth;//sets it to full health in case its a float
            }
        }

    }

    public Player_Stats() //player base stats set
    {
        damage = dGems * 2;
        Maxhealth = hGems + 10;
        maxMana = 20;
    }

    public void TrapDamage(int damage) //for player taking trap damage
    {

        health -= damage; // player loses health
        SetHealth(health); // sets the new health on the bar
    }

    public void Damage(Collision2D enemy_ref) //for player taking damage
    {
        if (_playerSM.GetCurrentState() != _playerSM.deathState)
        {
            health -= enemy_ref.gameObject.GetComponent<Enemy_Stats>().damage; // player loses health
            SetHealth(health); // sets the new health on the bar
            if (health <= 0)
            {
                _playerSM.ChangeState(_playerSM.deathState);
            }
        }
    }

    public void Damage(Collider2D enemy_ref)
    {
        if (_playerSM.GetCurrentState() != _playerSM.deathState)
        {
            health -= enemy_ref.gameObject.GetComponentInParent<Enemy_Stats>().damage; // player loses health
            SetHealth(health); // sets the new health on the bar
            if (health <= 0)
            {
                _playerSM.ChangeState(_playerSM.deathState);
            }
        }
    }
    public void Kill() //kills the player instantly
    {
        health = -1; // sets the health to zero
        SetHealth(health); // health bar set to zero
        if (health <= 0)
        {
            _playerSM.ChangeState(_playerSM.deathState);
        }
    }

    public void heal() //sets health and mana after heal
    {
        if ((health <= Maxhealth - 2) && mana >= 10)//if you have enough mana and your health is not full
        {
            health += 2; //updates health and mana after a heal
            SetHealth(health);
            mana -= 10;
            SetMana(mana);
            Debug.Log(health);
        }
        else if (health == Maxhealth)
        {
            // do nothing
        }
        else if ((health >= Maxhealth - 2) && health < Maxhealth)//for the one case
        {
            health = Maxhealth;//updates health and mana after a heal
            SetHealth(health);
            mana -= 10;
            SetMana(mana);
        }
        

    }

    public void Respawn()
    {
        health = Maxhealth;
        SetHealth(health);
        this.transform.position = checkpoint;
        _playerSM.ChangeState(_playerSM.idleState);
    }

    public void RangedAttack() //takes mana away for ranged attacks
    {
        mana -= 7;
        SetMana(mana);
        
    }

    public void HealthPowerUp()
    {
        hGems += 1;
        health += 1;
        SetHealth(health);
        // SetMaxHealth(hGems, health);
    }

    public void DamagePowerUp()
    {
        dGems += 1;
        damage = dGems * 2;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            _playerSM.ChangeState(_playerSM.damageState);
            Damage(collision);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bonfire" && health < Maxhealth)//if touching the fire and can heal
        {
            regenHealth = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bonfire")//if leaving the fire
        {
            regenHealth = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Dash")
        {
            Damage(collision);
        }
    }

    //Health UI/Getters and Setters

    public float GetHealth()//gets health value
    {
        return health;
    }
    public void SetHealth(float H)//sets health value
    {
        healthBar.value = H;
    }

    public float GetMana()//gets mana value
    {
        return mana;
    }
    public void SetMana(float M)//sets mana value
    {
        manaBar.value = M;
    }

    public int GetHGems()
    {
        return hGems;
    }

    public Vector3 GetCheckpoint()
    {
        return checkpoint;
    }

    public void SetMaxHealth(int HG, float H)
    {
        /* hGems = HG;
         health = H;
         Maxhealth = hGems + 10;
         HealthBar.maxValue = Maxhealth + 10;
         HealthBar.value = health;*/
    }

    public int GetPlayerDamage()
    {
        return damage;
    }

    public int GetdGems()
    {
        return dGems;
    }

    public void SetdGemes(int DG)
    {
        dGems = DG;
        damage = dGems * 2;
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }

    public bool GetDashUpgrade()
    {
        return dashUpgrade;
    }

    public bool GetWallHangUpgrade()
    {
        return wallHangUpgrade;
    }

    IEnumerator RegenerateMana()//regenerates mana over time
    {
        mana += 0.01f;
        manaBar.value = mana;
        yield return new WaitForSeconds(Time.deltaTime);
    }

    IEnumerator RegenerateHealth()//regenerates mana over time
    {
        health += 0.025f;
        healthBar.value = health;
        yield return new WaitForSeconds(Time.deltaTime);
    }

}
