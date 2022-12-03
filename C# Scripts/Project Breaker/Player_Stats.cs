using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class Player_Stats : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
    [DllImport("PBFighter")]
    private static extern void FillStatsF(float[] stats, int length);
    [DllImport("PBRogue")]
    private static extern void FillStatsR(float[] stats, int length);
    [DllImport("PBTank")]
    private static extern void FillStatsT(float[] stats, int length);
    [DllImport("PBMusket")]
    private static extern void FillStatsM(float[] stats, int length);
#endif


    public int classSelect;
    Animator anim;
    private float dTime = 0.583f;

    private enum pClasses
    {
        Rogue,
        Tank,
        Fighter,
        Musket
    }
    private pClasses pClass;
    private int level;
    private int xp;
    private float maxHealth;
    [SerializeField]private float health=1;
    [SerializeField]public float speed;
    [SerializeField]private float hDamage;
    [SerializeField]private float lDamage;
    [SerializeField] private float score = 0;

    public float[] classSpeeds = { 1.5f, 1.5f, 1.0f, 1.0f };

    //TODO might be wise to make these private variables with getters and setters but that is cleanup
    public float dashSpeed = 15f;
    public float dashTime = 0.25f;
    public float dashcooldown;


    void Start()
    {
        // This set the intial class of the characterSelect variable and then intializes the class stats based off of the players class
        classSelect = PlayerPrefs.GetInt("PlayerClass");
        anim = GetComponentInChildren<Animator>();
        SetPclass(classSelect);
        Debug.Log(classSelect);
        float[] stats=new float[10];

        switch(classSelect)
        {
            //rogue
            case 0:
                stats = new float[] { 10, 4f, 5, 4,1};
                break;
            //gunner
            case 1:
                stats = new float[] { 12, 4f, 4, 4,1};
                break;
            //zwei
            case 2:
                stats = new float[] { 7, 4.5f, 6, 3,1};
                break;
            //musket
            case 3:
                stats = new float[] { 6, 4.5f, 5, 3,1};
                break;
        }

        health = stats[0];
        maxHealth = health;
        speed = stats[1];
        hDamage = stats[2];
        lDamage = stats[3];
        dashcooldown = stats[4];

    }

    public void ManagedUpdate()
    {
        if (health <= 0)
        {
            anim.SetBool("isDead", true);
            if (NetworkingManager.Instance != null)
            {
                NetworkingManager.Instance.TCPSend(20, "isDead;true");
            }

            if (NetworkingManager.Instance != null)
            {
                Invoke("NetworkedRestart", 1.5f);
            }
            else
            {
                Invoke("Restart", 1.5f);
            }
        }


        if (anim.GetBool("isDamaged") == true && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            anim.SetBool("isDamaged", false);
            if (NetworkingManager.Instance != null)
            {
                NetworkingManager.Instance.TCPSend(20, "isDead;false");
            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NetworkedRestart()
    {
        //ask harry if we need to do anything to escape the death state
        anim.SetBool("isDead", false);
        ResetHealth();
        gameObject.transform.position = Player_Manager.Instance.startPos;
    }

    //This is for dealing damage to the player and returning whether or not the player has died
    float Damage(float dDealt)
    {
        anim.SetBool("isDamaged",true);
        if (NetworkingManager.Instance != null)
        {
            NetworkingManager.Instance.TCPSend(20, "isDamaged;true");
        }
        health -= dDealt;
        return health;
    }

    public void AddHealth(float hlth)
    {
        if (maxHealth > (hlth + health))
        {
            health += hlth;//add health
        }
        else
            health = maxHealth;//if over max set it to max health
    }

    public void AddScore(int nScore)
    {
        score += nScore;
    }
    public void ResetHealth()
    {
        health = maxHealth;
    }



#region Getters
    public int GetLevel()
    {
        return level;
    }
    public int GetXp()
    {
        return xp;
    }
    public float GetHealth()
    {
        return health;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public float Heavy()
    {
        return hDamage;
    }
    public float Light()
    {
        return lDamage;
    }
#endregion
#region Setters
    void SetPclass(int inclass)
    {
        switch (inclass)
        {
            case 1:
                pClass = pClasses.Rogue;
                break;
            case 2:
                pClass = pClasses.Tank;
                break;
            case 3:
                pClass = pClasses.Fighter;
                break;
            case 4:
                pClass = pClasses.Musket;
                break;

        };
    }
    void Setlevel(int newLevel)
    {
        level = newLevel;
    }
    void SetXp(int newXp)
    {
        xp = newXp;
    }
    void SetHealth(int newHealth)
    {
        health = newHealth;
    }
    void SetHeavy(int newDamage)
    {
        hDamage = newDamage;
    }
    void Setlight(int newDamage)
    {
        lDamage = newDamage;
    }
#endregion

    private void OnTriggerEnter(Collider col)
    {
        Enemy_Stats dmg = col.gameObject.GetComponentInParent<Enemy_Stats>();
        if (col.gameObject.tag == "Enemy")
        {
            Damage(dmg.Attack());
            Debug.Log(health);
        }
        if (col.gameObject.tag == "Arrow")
        {
            Damage(2f); //2 damage
            Debug.Log(health);
        }
        if (col.gameObject.tag == "Spikes")
        {
            Damage(2f); //2 damage
            Debug.Log(health);
        }
        if (col.gameObject.tag == "Bullet")
        {
            Damage(col.GetComponent<Bullet>().GetDamage());
        }
    }
   
}
