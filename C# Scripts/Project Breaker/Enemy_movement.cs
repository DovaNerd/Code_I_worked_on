using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PBServer;

public class Enemy_movement : MonoBehaviour
{
    public float lookRadius = 10f;

    [SerializeField] Transform target;
    [SerializeField] GameObject manager;
    public NavMeshAgent agent;
    private Transform test;
    public GameObject bulletPre;
    public Transform firePoint;
    AnimatorClipInfo[] clipInfo;
    public ParticleSystem smoke = null;
    public GameObject indicator;

    public float attackTime;
    public Collider hitbox;
    public Animator anim;
    private Enemy_Stats stats;
    private bool active;

    private void Start()
    {
        stats = GetComponent<Enemy_Stats>();
        clipInfo = anim.GetCurrentAnimatorClipInfo(0);
    }

    void Update()
    {
        clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        // Getting the position of the player
            if (target == null)
            {
                manager = GameObject.Find("Game_Manager");
                target = manager.GetComponent<Player_Manager>().GetPlayer().transform;
                agent = GetComponent<NavMeshAgent>();
            }

        if (NetworkingManager.Instance != null)
        {
            if (Vector3.Distance(manager.GetComponent<Player_Manager>().GetPlayer().transform.position, transform.position) < Vector3.Distance(target.transform.position, transform.position))
            {
                target = manager.GetComponent<Player_Manager>().GetPlayer().transform;
            }
            foreach (LightPlayerData player in Player_Manager.Instance.otherPlayers.Values)
            {
                if (Vector3.Distance(player.pObject.transform.position, transform.position) < Vector3.Distance(target.position, transform.position))
                {
                    target = player.pObject.transform;
                }

            }
        }


        if (this.tag == "Enemy" || this.tag == "enemy1" || this.tag == "enemy2" || this.tag == "enemy3")
        {
            // Here we calculate the distance from the player to the enemy
            float distance = Vector3.Distance(target.position, transform.position);

            // If they are in the sight bubble we activate the enemy and set their destination to the players location
            if (distance <= lookRadius || active == true)
            {
                agent.SetDestination(target.position);
                anim.SetBool("isWalking", true);

            }
            else
            {
                anim.SetBool("isWalking", false);
            }

            if (anim.GetBool("isDead") == false) //when not dead
            {
                // This is the code that decides when the enemy is supposed to attack
                if (attackTime > 0 && distance <= agent.stoppingDistance + 2f)
                {
                    anim.SetBool("isWalking", false);
                    indicator.SetActive(true);
                    attackTime -= Time.deltaTime;
                    if (attackTime < 2 && attackTime > 1 && hitbox.enabled != true)
                    {
                        hitbox.enabled = true;
                        anim.SetBool("isAttacking", true);

                    }
                }
                // The enemy will attack here if they are within stoping distance to the player and then reset their attack timer
                else if (attackTime <= 0 || distance >= agent.stoppingDistance + 2f)
                {
                    if (hitbox.enabled == true)
                    {
                        hitbox.enabled = false;
                        anim.SetBool("isAttacking", false);
                    }
                    indicator.SetActive(false);
                    attackTime = 3f;
                }
            }
            // This is so the enemy faces the player when they are within range
            FaceTarget();
        }
        // This handles the functionality if the Enemy is of the musket type
        else if (this.tag =="MusketEnemy")
        {
            // Here we calculate the distance from the player to the enemy
            float distance = Vector3.Distance(target.position, transform.position);

            // Begin walking towards the player if they are within our sight bubble
            if (distance <= lookRadius && anim.GetBool("isAim") == false || active == true && anim.GetBool("isAim") == false)
            {
                agent.SetDestination(target.position);
                anim.SetBool("isWalking", true);
            }
            else if (anim.GetBool("isAim") == true)
            {
                agent.SetDestination(this.transform.position);
            }
            else
            {
                anim.SetBool("isWalking", false);
            }

            if (anim.GetBool("isDead") == false) //when not dead
            {
                // This is the code that decides when the enemy is supposed to attack
                if (attackTime > 0 && distance <= agent.stoppingDistance + 4 && anim.GetBool("isShoot") == false)
                {
                    // Begin to aim at the player
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isAim", true);
                    attackTime -= Time.deltaTime;
                    indicator.SetActive(true);
                    // If the enemy is suposed to attack the player
                    if (attackTime <= 2 && attackTime >= 1)
                    {
                        if(smoke != null)
                        smoke.Play();
                        anim.SetBool("isShoot", true);
                        anim.SetBool("isReload", true);
                        anim.SetBool("isAim", false);
                        GameObject bullet = Instantiate(bulletPre, firePoint.transform.position, Quaternion.identity);
                        Vector3 dir = (this.transform.forward);
                        bullet.GetComponentInChildren<Bullet>().Setup(dir, stats.GetBullet());
                    }
                }
                else if (attackTime <= 0 || distance >= agent.stoppingDistance)
                {
                    anim.SetBool("isReload", true);
                }

                if (clipInfo[0].clip.name == "Reload")
                {
                    anim.SetBool("isShoot", false);
                    anim.SetBool("isReload", false);
                    anim.SetBool("isAim", false);
                    indicator.SetActive(false);
                    attackTime = 6f;
                }
            }
            if (anim.GetBool("isAim") == false)
            {
                FaceTarget();
            }
        }
    }
    // This is so that the enemy will try to face the direction the player is in
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion LookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, LookRot, Time.deltaTime * 5);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadius);

    }

    public void SetActive()
    {
        active = true;
    }
}
