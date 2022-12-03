using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using PBServer;

public class Player_Manager : MonoBehaviour
{
    #region Singleton

    public GameObject player;


    public Player_Stats pS;
    public Player_movement pM;
    public Player_combat pC;
    public TextAsset json;

    public static Player_Manager Instance;

    [SerializeField] private float udpFrequency=30;
    private float timer;
    public Dictionary<Guid, LightPlayerData> otherPlayers = new Dictionary<Guid, LightPlayerData>();

    private int dummyPacks = 0;
    private TcpPBPacket tPack=null;
    private UdpPBPacket uPack=null;

    List<string> namesList = new List<string>();
    List<int> scoresList = new List<int>();


    Vector3 posLast = new Vector3();

    bool showingScore = false;
    float[] tempPos = new float[6];

    public Vector3 startPos;
    
    private void Update()
    {
        if (player == null)
            return;

        pS.ManagedUpdate();
        pM.ManagedUpdate();
        pC.ManagedUpdate();

        if(NetworkingManager.Instance!=null)
        {
            timer += Time.deltaTime;
            if (timer > (1 / udpFrequency))
            {
                if(player.transform.position!=posLast)
                {
                    NetworkingManager.Instance.UDPSend(player.transform);
                    posLast = player.transform.position;
                    dummyPacks = 0;
                }
                else if(dummyPacks<10)
                {
                    NetworkingManager.Instance.UDPSend(player.transform);
                    posLast = player.transform.position;
                    dummyPacks++;
                }
                timer = 0.0f;
            }
            if(NetworkingManager.Instance.ReceiveUDP())
            {
                uPack = NetworkingManager.Instance.GetUdpPacket();
                if (otherPlayers.ContainsKey(uPack.GetGuid()))
                {
                    tempPos = uPack.GetPos();
                    otherPlayers[uPack.GetGuid()].pObject.transform.position = new Vector3(tempPos[0], tempPos[1], tempPos[2]);
                    otherPlayers[uPack.GetGuid()].pObject.transform.eulerAngles = new Vector3(tempPos[3], tempPos[4], tempPos[5]);
                    otherPlayers[uPack.GetGuid()].received = true;
                    otherPlayers[uPack.GetGuid()].posCache.RemoveAt(0);
                    otherPlayers[uPack.GetGuid()].posCache.Add(otherPlayers[uPack.GetGuid()].pObject.transform.position);
                }

            }
            if(NetworkingManager.Instance.ReceiveTCP())
            {
                tPack = NetworkingManager.Instance.GetTcpPacket();
                switch(tPack.GetOp())
                {
                    case 14:
                        //damage enemy from remote player
                        Debug.Log("Received remote damage");
                        string[] temp = tPack.GetMes().Split(';');
                        GameObject.Find(temp[1]).GetComponent<Enemy_Stats>().NetworkedDamage(Convert.ToInt32(temp[0]));
                        NetworkingManager.Instance.scoresStorage[tPack.GetGuid()] += Convert.ToInt32(temp[0]);
                        break;
                    case 15:
                        //end level
                        GameObject.Find("Exit").GetComponent<LevelExit>().NetworkedExit();
                        break;
                    case 16:
                        //damage crate from remote player
                        try
                        {
                            GameObject.Find(tPack.GetMes()).GetComponentInChildren<Destructable>().NetworkedBreak();

                        }
                        catch
                        {

                        }
                        break;
                    case 17:
                        //break health crate
                        GameObject.Find(tPack.GetMes()).GetComponentInChildren<HealthCrate>().NetworkedBreak();
                        break;
                    case 18:
                        GameObject.Find(tPack.GetMes()).GetComponentInChildren<TrapLaunch>().NetworkedLaunch();
                        break;
                    case 19:
                        GameObject.Find(tPack.GetMes()).GetComponentInChildren<SpikeTrap>().NetworkedOn();
                        break;
                    case 20:
                        string[] tempBool = tPack.GetMes().Split(';');
                        otherPlayers[tPack.GetGuid()].pObject.GetComponentInChildren<Animator>().SetBool(tempBool[0], Convert.ToBoolean(tempBool[1]));
                        break;
                    case 21:
                        string[] tempInt = tPack.GetMes().Split(';');
                        otherPlayers[tPack.GetGuid()].pObject.GetComponentInChildren<Animator>().SetInteger(tempInt[0], Convert.ToInt32(tempInt[1]));
                        break;
                }
            }
            foreach(LightPlayerData player in otherPlayers.Values)
            {
                if(player.received)
                {
                    player.received = false;
                }
                else
                {
                    player.pObject.transform.position += (player.posCache[player.posCache.Count-1] - player.posCache[0]).normalized * pS.classSpeeds[player.pClass]* Time.deltaTime;
                    if(player.posCache[player.posCache.Count - 1] == player.posCache[0])
                    {
                        player.pObject.GetComponentInChildren<Animator>().SetBool("isWalking", false);
                    }
                    else
                    {
                        player.pObject.GetComponentInChildren<Animator>().SetBool("isWalking", false);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showingScore = !showingScore;
            //display the scoreboard
            if(showingScore==true)
            {
                NetworkingManager.Instance.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                namesList.Clear();
                scoresList.Clear();

                foreach (LightPlayerData player in otherPlayers.Values)
                {
                    namesList.Add(player.pName);
                }
                foreach(int score in NetworkingManager.Instance.scoresStorage.Values)
                {
                    scoresList.Add(score);
                }
                NetworkingManager.Instance.namesText[0].text = NetworkingManager.Instance.name;
                NetworkingManager.Instance.scoreText[0].text = Convert.ToString(NetworkingManager.Instance.playerTotalDamage);
                for (int i = 0; i < namesList.Count; i++)
                {
                    NetworkingManager.Instance.namesText[i+1].text = namesList[i];
                    NetworkingManager.Instance.scoreText[i+1].text = Convert.ToString(scoresList[i]);
                }
                for (int i = namesList.Count; i < 3; i++)
                {
                    NetworkingManager.Instance.namesText[i+1].text = "";
                    NetworkingManager.Instance.scoreText[i+1].text = "";
                }
            }
            else
            {
                NetworkingManager.Instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }

        }
    }
    private void Start()
    {
        pS = player.GetComponent<Player_Stats>();
        pM = player.GetComponent<Player_movement>();
        pC = player.GetComponent<Player_combat>();

    }
#endregion



public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
        pS = player.GetComponent<Player_Stats>();
        pM = player.GetComponent<Player_movement>();
        pC = player.GetComponent<Player_combat>();
    }

    public GameObject GetPlayer()
    {
        return player;
    }
    public void SetComponents()
    {
        if (player != null)
        {
            pS = player.GetComponent<Player_Stats>();
            pM = player.GetComponent<Player_movement>();
            pC = player.GetComponent<Player_combat>();
        }
    }
    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
