using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using PBServer;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] Shader unlit;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject vcam;
    [SerializeField] GameObject mapVCam;
    [SerializeField] GameObject mapCam;
    public GameObject dashPre;


    [SerializeField] GameObject playerManager;
    private Player_Manager manager;
    [SerializeField] GameObject playerRef;
    public GameObject musketPre;
    public GameObject zweihanderPre;
    public GameObject gunnerPre;
    public GameObject TankPre;

    public GameObject muskDummy;
    public GameObject zweiDummy;
    public GameObject gunDummy;
    public GameObject tankDummy;


    private CinemachineVirtualCamera camref;

    public int classSelect;

    
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("CM vcam1");
        mapVCam = GameObject.Find("CM vcam2");
        mapCam = GameObject.Find("MapCam");
        vcam = GameObject.Find("Camera");

        GameObject dash = Instantiate(dashPre, new Vector3(1000f, 1000f, 1000f), Quaternion.identity);

        // Setting the refernce in the player manager
        playerManager = GameObject.Find("Game_Manager");
        manager = playerManager.GetComponent<Player_Manager>();
        classSelect = PlayerPrefs.GetInt("PlayerClass");
        if(NetworkingManager.Instance!=null)
        {
            zweiDummy = NetworkingManager.Instance.zweiDummy;
            muskDummy = NetworkingManager.Instance.muskDummy;
            gunDummy = NetworkingManager.Instance.gunDummy;
            tankDummy = NetworkingManager.Instance.tankDummy;
            Debug.Log("this is a networked game session");
            Debug.Log(NetworkingManager.Instance.storage);
            string[] temp = NetworkingManager.Instance.storage.Split(';');
            for(int i = 0; i<temp.Length-2;i+=3)
            {
                Guid tempID = new Guid(temp[i]);
                if(tempID!=NetworkingManager.Instance.id)
                {
                    Debug.Log("Spawning New Player Right here");
                    switch (Convert.ToInt32(temp[i + 2]))
                    {
                        case 0:
                            manager.otherPlayers.Add(tempID,new LightPlayerData(Instantiate(zweiDummy),temp[i+1],0));
                            break;

                        case 1:
                            manager.otherPlayers.Add(tempID,new LightPlayerData(Instantiate(tankDummy),temp[i+1],1));
                            break;
                        case 2:
                            manager.otherPlayers.Add(tempID,new LightPlayerData(Instantiate(gunDummy),temp[i+1],2));
                            break;
                        case 3:
                            manager.otherPlayers.Add(tempID,new LightPlayerData(Instantiate(muskDummy),temp[i+i],3));
                            break;
                    }
                    if(!NetworkingManager.Instance.scoresStorage.ContainsKey(tempID))
                    {
                        NetworkingManager.Instance.scoresStorage.Add(tempID, 0);
                    }
                    manager.otherPlayers[tempID].posCache.Add(transform.position);
                    manager.otherPlayers[tempID].posCache.Add(transform.position);
                    manager.otherPlayers[tempID].posCache.Add(transform.position);
                    manager.otherPlayers[tempID].posCache.Add(transform.position);

                }
            }
        }

        // Creating the player prefab based on what class they chose
        switch (classSelect)
        {
            case 0:
                {
                    playerRef = Instantiate(zweihanderPre, this.transform.position, Quaternion.identity);
                    break;
                }
            case 1:
                {
                    playerRef = Instantiate(TankPre, this.transform.position, Quaternion.identity);
                    break;
                }
            case 2:
                {
                    playerRef = Instantiate(gunnerPre, this.transform.position, Quaternion.identity);
                    break;
                }
            case 3:
                {
                    playerRef = Instantiate(musketPre, this.transform.position, Quaternion.identity);
                    break;
                }
        }

        GameObject.FindGameObjectWithTag("UIObject").GetComponent<UIScript>().player = playerRef;
        GameObject.FindGameObjectWithTag("UIObject").GetComponent<UIScript>().playerStats = playerRef.GetComponent<Player_Stats>();


        cam.GetComponent<CinemachineVirtualCamera>().LookAt = playerRef.transform;
        cam.GetComponent<CinemachineVirtualCamera>().Follow = playerRef.transform;
        mapVCam.GetComponent<CinemachineVirtualCamera>().Follow = playerRef.transform;
        playerManager.GetComponent<Player_Manager>().SetPlayer(playerRef);
        Player_Manager.Instance.startPos = playerRef.transform.position;
    }
}
