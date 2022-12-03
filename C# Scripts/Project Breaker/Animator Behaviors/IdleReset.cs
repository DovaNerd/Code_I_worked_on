using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleReset : StateMachineBehaviour
{
    private GameObject manager;
    private GameObject player;

    //This Code uses the players class to determine whether or not the player can even start a heavy or light combo
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        manager = GameObject.FindGameObjectWithTag("Manage");
        player = manager.GetComponent<Player_Manager>().GetPlayer();
        player.GetComponent<Player_combat>().CombatReset();
        player.GetComponent<Player_combat>().isShoot = false;
        if (player.GetComponent<Player_combat>().trail != null)
        player.GetComponent<Player_combat>().trail.enabled = false;

        player.GetComponent<Player_movement>().SetStop(false);
        switch (player.GetComponent<Player_Stats>().classSelect)
        {
            case 0:
                player.GetComponent<Player_combat>().SetHeavy(true);
                player.GetComponent<Player_combat>().SetLight(true);
                break;
            case 1:
                player.GetComponent<Player_combat>().SetHeavy(true);
                player.GetComponent<Player_combat>().SetLight(true);
                break;
            case 2:
                player.GetComponent<Player_combat>().SetLight(true);
                break;
            case 3:
                player.GetComponent<Player_combat>().SetLight(true);
                break;
        }
    }

    //This resets the combat mechanics of the player when they continue the combo it resets the input variable so the player can input another attack as well as resets the hitboxes
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*
        manager = GameObject.FindGameObjectWithTag("Manage");
        player = manager.GetComponent<Player_Manager>().GetPlayer();
        player.GetComponent<Player_combat>().CombatReset();
        */
    }
    
}
