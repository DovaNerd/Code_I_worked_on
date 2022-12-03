using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateConc : StateMachineBehaviour
{
    private GameObject manager;
    private GameObject player;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        manager = GameObject.FindGameObjectWithTag("Manage");
        player = manager.GetComponent<Player_Manager>().GetPlayer();
        player.GetComponent<Player_combat>().SetLight(false);
        player.GetComponent<Player_combat>().SetHeavy(false);
        player.GetComponent<Player_combat>().ComboReset();
    }
}
