using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Enemy_Stats : MonoBehaviour
{
    [SerializeField] private EventReference damageSound;
    [SerializeField] private EventReference deadSound;
    [SerializeField] private EventReference hitSound;

    [SerializeField] float health;
    private float damage;
    private int bulletDamage;
    bool deadSoundPlayed = false;
    private SkinnedMeshRenderer mr;
    Animator anim;
    private int score;

    //Materials
    private Color matDef;

    void Start()
    {
        health = 10;
        damage = 2.0f;
        bulletDamage = 4;
        score = 10;
        mr = GetComponentInChildren<SkinnedMeshRenderer>();
        matDef = mr.material.color;
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
       //if (anim.GetBool("isDead") == true && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        //{
         //   Destroy(this.gameObject);
        //}
        if (health <= 0)
        {
            anim.SetBool("isDead", true);
            if (!deadSoundPlayed)
            {
                RuntimeManager.PlayOneShot(deadSound);
                deadSoundPlayed = true;
            }
            Invoke("DestroyE", 1.0f);
        }

        if (anim.GetBool("isDamaged") == true && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            anim.SetBool("isDamaged", false);
        }
    }
    public void DestroyE()
    {

        Destroy(this.gameObject);
    }

    //This is for dealing damage to the enemy and returning whether or not the enemy has died
    public float Damage(float dDealt)
    {
        if(NetworkingManager.Instance!=null)
        {
            NetworkingManager.Instance.playerTotalDamage += dDealt;
            NetworkingManager.Instance.SendDamage((int)dDealt, gameObject.name);
        }
        mr.material.color = Color.red;
        Invoke("Flashstop", 0.1f);

        if (health > 0)
        {
            RuntimeManager.PlayOneShot(hitSound);
            RuntimeManager.PlayOneShot(damageSound);
        }

        health -= dDealt;
        
        DamNum.instance.SpawnDamNum(transform.position, (float)dDealt);
        //gameObject.GetComponentInChildren<ParticleSystem>().Play();
        //currently there is no particle system needs fixed
        anim.SetBool("isDamaged", true);
        return health;
    }

    public float NetworkedDamage(int dDealt)
    {
        mr.material.color = Color.red;
        Invoke("Flashstop", 0.1f);

        if (health > 0)
        {
            RuntimeManager.PlayOneShot(hitSound);
            RuntimeManager.PlayOneShot(damageSound);
        }

        health -= dDealt;

        DamNum.instance.SpawnDamNum(transform.position, (float)dDealt);
        //gameObject.GetComponentInChildren<ParticleSystem>().Play();
        //currently there is no particle system needs fixed
        anim.SetBool("isDamaged", true);
        return health;
    }


    void Flashstop()
    {
        mr.material.color = matDef;
    }

    #region Getters
    public float Attack()
    {
        return damage;
    }
    public float GetHealth()
    {
        return health;
    }

    public float GetBullet()
    {
        return bulletDamage;
    }
    #endregion
    #region Setters
    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
    #endregion

    private void OnTriggerEnter(Collider col)
    {
        Player_Stats dmg = col.gameObject.GetComponentInParent<Player_Stats>();
        if (col.gameObject.tag == "Lbox")
        {
            Damage(Convert.ToInt32(dmg.Light()));
        }
        if (col.gameObject.tag == "Hbox")
        {
            Damage(Convert.ToInt32(dmg.Heavy()));
        }
        if (col.gameObject.tag == "Bullet")
        {
            Damage(col.gameObject.GetComponent<Bullet>().GetDamage());
            this.GetComponent<Enemy_movement>().SetActive();
        }
    }
}
