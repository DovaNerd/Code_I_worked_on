using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Player_movement : MonoBehaviour
{
    [SerializeField] private EventReference DashSound;

    public CharacterController controller;
    [SerializeField] private Camera mainCamera;
    [SerializeField] GameObject cam;
    [SerializeField] private GameObject vCam;
    [SerializeField] private Player_combat pCom;
    private Player_Stats pStats;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    private float dTime;
    private bool stop;

    private Joystick mobileStick = null;

    private Vector3 camToP2DV;
    private Vector3 camToP2DH;
    private bool camDirDirty = true;

    private bool camMoving = false;
    private Vector3 startAngle;
    private Vector3 endAngle;
    private float camCounter;
    [SerializeField] private float camMoveTime;

    private float mouseX;
    private float mouseZ;



    Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        vCam = GameObject.Find("CM vcam1");
        cam= GameObject.Find("Camera");
        mainCamera = cam.GetComponent<Camera>();
        pCom = GetComponent<Player_combat>();
        pStats = GetComponent<Player_Stats>();
        dTime = pStats.dashcooldown;
    }


    // Update is called once per frame
    public void ManagedUpdate()
    {

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        if (camDirDirty)
        {
            camToP2DV = Vector3.Normalize(new Vector3(mainCamera.transform.forward.x, 0.0f, mainCamera.transform.forward.z));
            camToP2DH = Quaternion.AngleAxis(90.0f, Vector3.up) * camToP2DV;
            camDirDirty = false;
        }
        Vector3 move = new Vector3(0.0f, 0.0f, 0.0f);
        //This is for moving the character
#if UNITY_MOBILE
        move = camToP2DH * mobileStick.Horizontal + camToP2DV * mobileStick.Vertical;
#else
        move = camToP2DH * Input.GetAxis("Horizontal") + camToP2DV * Input.GetAxis("Vertical");
#endif
        if (Mathf.Abs(move.sqrMagnitude) > 0.1f)
        {
            anim.SetBool("isWalking", true);          
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        if (stop == false)
        {
            Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            float rayLength;
            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);

                transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
            }

            if (pCom.isShoot == false)
            controller.Move(move * Time.deltaTime * pStats.speed);

        }

        if (dTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) && pCom.isShoot == false)
            {
                StartCoroutine(Dash(move.normalized));
            }
        }
        if (Input.GetButtonDown("CameraLeft") && !camMoving)
        {
            Transform vCamTrans = vCam.transform;
            startAngle = vCamTrans.rotation.eulerAngles;
            endAngle = new Vector3(startAngle.x, startAngle.y - 90.0f, startAngle.z);
            camCounter = 0.0f;
            camMoving = true;
        }
        if (Input.GetButtonDown("CameraRight") && !camMoving)
        {
            Transform vCamTrans = vCam.transform;
            startAngle = vCamTrans.rotation.eulerAngles;
            endAngle = new Vector3(startAngle.x, startAngle.y + 90.0f, startAngle.z);
            camCounter = 0.0f;
            camMoving = true;
        }
        if (camMoving)
        {
            camCounter += Time.deltaTime;
            Transform vCamTrans = vCam.transform;
            float camT = camCounter / camMoveTime;
            vCam.transform.eulerAngles = Vector3.Slerp(startAngle, endAngle, camT);
            if (camCounter >= camMoveTime)
            {
                vCam.transform.eulerAngles = endAngle;
                camMoving = false;
            }
            camDirDirty = true;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        dTime -= Time.deltaTime;
    }

    public void SetJoyStick(GameObject newStick)
    {
        mobileStick = newStick.GetComponent<Joystick>();
    }
    
    private IEnumerator Dash(Vector3 dir)
    {
        GameObject particle = GameObject.FindGameObjectWithTag("Dash");
        particle.transform.position = this.transform.position;
        particle.GetComponent<ParticleSystem>().Play();

        FMODUnity.RuntimeManager.PlayOneShot(DashSound);

        float startTime = Time.time;
        dTime = pStats.dashcooldown;
        pCom.FullCombatReset();

        while(Time.time < startTime + pStats.dashTime)
        {
            controller.Move(dir * pStats.dashSpeed * Time.deltaTime);
            yield return null;
        }

    }

    public void SetStop(bool state)
    {
        stop = state;
    }
    
}
