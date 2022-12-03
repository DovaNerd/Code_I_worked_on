using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a generic state its inherits directly from base state however does not encapsulate code from other states
public class Attack : BaseState
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public Attack (PlayerSM stateMachine) : base ("Attack", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        _sm.anim.SetTrigger("Attacking");
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        switch (_sm.moveinput.y)
        {
            case 0:
                _sm._attackPoint.localPosition = new Vector3(2 * _sm.moveinput.x, 0, 0);
                break;
            case 1:
                _sm._attackPoint.localPosition = new Vector3(0, 2 * _sm.moveinput.y, 0);
                break;
            case -1:
                _sm._attackPoint.localPosition = new Vector3(0, 2 * _sm.moveinput.y, 0);
                break;
            default:
                _sm._attackPoint.localPosition = new Vector3(2 * 1, 0, 0);
                break;
        }
        Collider2D[] enemys = Physics2D.OverlapCircleAll(_sm._attackPoint.position, 2, _sm._enemyLayer);
        foreach (var enemy in enemys)
        {
            enemy.GetComponent<Enemy_Stats>().Damage(_sm._pStats.GetPlayerDamage());
        }
        /*
        Collider2D[] walls = Physics2D.OverlapCircleAll(_sm._attackPoint.position, 2);
        foreach (var wall in walls)
        {
            wall.GetComponent<Wall_Stats>().Damage(_sm._pStats.GetPlayerDamage());
        }
        */
        stateMachine.ChangeState(_sm.GetPreviousState());
    }
}
