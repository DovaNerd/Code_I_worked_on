using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 facing;
    private float damage;

    public void Setup(Vector3 dir, float dam)
    {
        facing = dir;
        damage = dam;
    }

    private void Update()
    {
        float moveSpeed = 50f;
        transform.position += facing * moveSpeed * Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }

    public float GetDamage()
    {
        return damage;
    }
}
