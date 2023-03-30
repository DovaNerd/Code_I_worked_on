using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 30f; // How long the after image is going to stay active for
    private float timeActivated; // How long its been activated 
    private float alpha; // This alters the opacity of the material and is used as an in-between for variables
    [SerializeField]
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.85f;


    public Transform player;

    //References to objects on the player
    public SpriteRenderer SR;
    public SpriteRenderer PlayerSR;
    public GameObject Tpose;

    private Color color;


    private void OnEnable()
    {
        alpha = alphaSet;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Tpose = player.GetChild(4).gameObject;

        PlayerSR = player.GetComponentInChildren<SpriteRenderer>();

        if (Tpose.transform.localScale.z == -1)
            SR.flipX = true;
        else
            SR.flipX = false;

        SR.sprite = PlayerSR.sprite;
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
        transform.localScale = PlayerSR.transform.localScale;

        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);


        SR.color = color;

        if (Time.time >= (timeActivated + activeTime))
        {
            // Disable the game object and re-add it to the pool
            AfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
