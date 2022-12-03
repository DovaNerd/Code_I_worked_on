using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HCombo : StateMachineBehaviour
{
    private GameObject manager;
    private GameObject player;


    //This resets the combat mechanics of the player when they continue the combo it resets the input variable so the player can input another attack as well as resets the hitboxes
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        manager = GameObject.FindGameObjectWithTag("Manage");
        player = manager.GetComponent<Player_Manager>().GetPlayer();
        player.GetComponent<Player_combat>().CombatReset();
    }

    //This sets it so the Player_combat script knows what attack the player can chain into to prevent players dealing damage with no animation
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        manager = GameObject.FindGameObjectWithTag("Manage");
        player = manager.GetComponent<Player_Manager>().GetPlayer();
        player.GetComponent<Player_combat>().SetHeavy(true);
        player.GetComponent<Player_combat>().SetLight(false);
    }
}
