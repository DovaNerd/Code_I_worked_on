using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class Player_combat : MonoBehaviour
{
    public Collider hHitbox;
    public Collider lHitbox;
    Animator anim;
    [SerializeField] private EventReference swingSound;
    [SerializeField] private EventReference shotSound;
    Player_Stats stats;
    private int pClass;
    public GameObject bulletPre;
    public Transform firePoint;
    AnimatorClipInfo[] clipInfo;
    bool lDown = false;
    bool hDown = false;

    public TrailRenderer trail = null;
    public ParticleSystem smoke = null;

    [SerializeField] float cTime = 0.8f;

    /*
    float cTimeMin = 0.5f; //min you need to watch
    float cTimeMax = 0.8f; //after this takes you to idle
    */

    int lCombo = 0;
    int hCombo = 0;

    [SerializeField] bool hasIn = false;
    [SerializeField] bool reset = false;
    [SerializeField] bool hasHeavy;
    [SerializeField] bool hasLight;
    public bool isShoot;

    string animName;
    private bool mobileLPressed = false;
    private bool mobileHPressed = false;

    // This funcion is created to streamline the process of choosing what attack the player made
    void Combo(int attack)
    {
        FMODUnity.RuntimeManager.PlayOneShot(swingSound);
        if (attack == 0)
        {
            this.GetComponent<Player_movement>().SetStop(true);
            lHitbox.enabled = false;
            lHitbox.enabled = true;

            if(trail != null)
            trail.enabled = true;

            if (lCombo != 0)
            {
                hasIn = true;
            }
            lCombo++;
            anim.SetInteger("lCombo", lCombo);
            if (NetworkingManager.Instance != null)
            {
                NetworkingManager.Instance.TCPSend(21, "lCombo;" + lCombo);
            }
        }
        else if (attack == 1)
        {
            this.GetComponent<Player_movement>().SetStop(true);
            hHitbox.enabled = false;
            hHitbox.enabled = true;

            if (trail != null)
                trail.enabled = true;
            
            if (lCombo != 0)
            {
                hasIn = true;
            }
            hCombo++;
            anim.SetInteger("hCombo", hCombo);
            if (NetworkingManager.Instance != null)
            {
                NetworkingManager.Instance.TCPSend(21, "hCombo;" + hCombo);
            }
        }

    }
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        pClass = PlayerPrefs.GetInt("PlayerClass");
        stats = GetComponent<Player_Stats>();
        clipInfo = clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        animName = clipInfo[0].clip.name;
        Debug.Log(animName);
    }

    public void ManagedUpdate()
    {
        clipInfo = anim.GetCurrentAnimatorClipInfo(0);

#if UNITY_MOBILE
    lDown = mobileLPressed;
    mobileLPressed=false;
    hDown = mobileHPressed;
    mobileHPressed=false;
#else
        lDown = Input.GetMouseButtonDown(0);
        hDown = Input.GetMouseButtonDown(1);
#endif
        //This if statement is here for whether or not the player has pressed left click or right click if he has already done an attack for the current state and whether or not the current state should be allowed to do a heavy attack
        if (lDown && hasIn == false && hasLight == true)
        {
            Combo(0);
        }
        if (hDown && hasIn == false && hasHeavy == true)
        {
            Combo(1);
        }
        
        //Here we end the combo if the player has not done any attack for the time they can continue the combo if they dont we go back to start refer to the animation behavior scripts for additional information on the resetting process
        if (cTime <= 0)
        {
            lCombo = 0;
            hCombo = 0;
            anim.SetInteger("hCombo", hCombo);
            anim.SetInteger("lCombo", lCombo);
            if (NetworkingManager.Instance != null)
            {
                NetworkingManager.Instance.TCPSend(21, "hCombo;" + hCombo);
                NetworkingManager.Instance.TCPSend(21, "lCombo;" + lCombo);
            }
        }

        if (hDown == true && pClass >= 2 && clipInfo[0].clip.name == "Idle")
        {
            anim.SetBool("isAim", true);
            if (NetworkingManager.Instance != null)
            {
                NetworkingManager.Instance.TCPSend(20, "isAim;true");
            }

            isShoot = true;
        }
        else if (Input.GetMouseButtonUp(1) == true && pClass >= 2 && clipInfo[0].clip.name == "Aim" && anim.GetBool("isShoot") == false)
        {
            RuntimeManager.PlayOneShot(shotSound);

            if(smoke != null)
            smoke.Play();

            anim.SetBool("isShoot", true);
            anim.SetBool("isReload", true);
            anim.SetBool("isAim", false);
            if (NetworkingManager.Instance != null)
            {
                NetworkingManager.Instance.TCPSend(20, "isShoot;true");
                NetworkingManager.Instance.TCPSend(20, "isReload;true");
                NetworkingManager.Instance.TCPSend(20, "isAim;false");
            }
            GameObject bullet = Instantiate(bulletPre, firePoint.transform.position, Quaternion.identity);
            Vector3 dir = (this.transform.forward);
            bullet.GetComponentInChildren<Bullet>().Setup(dir, stats.Heavy());
        }
        // This is checking to see if the player is currently doing the reload animation and resets all the variables
        else if (clipInfo[0].clip.name == "Reload")
        {
            anim.SetBool("isShoot", false);
            anim.SetBool("isReload", false);
            anim.SetBool("isAim", false);
            if (NetworkingManager.Instance != null)
            {
                NetworkingManager.Instance.TCPSend(20, "isShoot;false");
                NetworkingManager.Instance.TCPSend(20, "isReload;false");
                NetworkingManager.Instance.TCPSend(20, "isAim;false");
            }
        }

        //This code deducts the timer only if the player has entered the state that way they have some extra time to input an additional attack
        if (lCombo > 0 && anim.IsInTransition(0) == false || hCombo > 0 && anim.IsInTransition(0) == false)
        {
            cTime -= Time.deltaTime;
        }
    }

    public void LButtonPressed()
    {
        mobileLPressed = true;
    }

    public void HButtonPressed()
    {
        mobileHPressed = true;
    }

    //This function serves the purpose of simplifying the process of resetting the combo system since its done so frequently by other code segments refer to behaviors for additional uses
    public void CombatReset()
    {
        lHitbox.enabled = false;
        hHitbox.enabled = false;
        hasIn = false;
        cTime = 0.8f;
    }

    public void ComboReset()
    {
        lCombo = 0;
        hCombo = 0;
        anim.SetInteger("hCombo", hCombo);
        anim.SetInteger("lCombo", lCombo);
    }

    public void FullCombatReset()
    {
        lCombo = 0;
        hCombo = 0;
        anim.SetInteger("hCombo", hCombo);
        anim.SetInteger("lCombo", lCombo);
        if (NetworkingManager.Instance != null)
        {
            NetworkingManager.Instance.TCPSend(21, "hCombo;" + hCombo);
            NetworkingManager.Instance.TCPSend(21, "lCombo;" + lCombo);
        }
        lHitbox.enabled = false;
        hHitbox.enabled = false;
        hasIn = false;
        cTime = 0.8f;
    }
    #region Getters
    /*
    public float GetcTimeMax()
    {
        return cTimeMax;
    }
    */

    public bool GetInput()
    {
        return hasIn;
    }
    #endregion

    #region Setters

    public void SetInput(bool state)
    {
        hasIn = state;
    }

    public void SetHeavy(bool state)
    {
        hasHeavy = state;
    }

    public void SetLight(bool state)
    {
        hasLight = state;
    }

    #endregion
}
