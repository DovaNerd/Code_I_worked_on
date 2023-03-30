using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

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
        switch (GetInputDirection())
        {
            case 0:
                _sm.AnimationPlay("Attack");
                break;
                case 1:
                _sm.AnimationPlay("Attack up");
                break;
            case -1:
                _sm.AnimationPlay("Attack down");
                break;
        }
        Collider2D[] enemys = Physics2D.OverlapCircleAll(_sm._attackPoint.position, 2, _sm._enemyLayer);
        if (enemys.Length != 0)
        {
            CinemachineShake.Instance.ShakeCamera(3f, 0.25f);
        }
        foreach (var enemy in enemys)
        {
            Enemy_Stats eneStats = enemy.GetComponent<Enemy_Stats>();
            eneStats.Damage(_sm._pStats.GetPlayerDamage());
            eneStats.enemyRigidbody.AddForce(new Vector2(2 * _sm.playerAnimation.localScale.z, 0), ForceMode2D.Impulse);
        }
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        /*
        Collider2D[] walls = Physics2D.OverlapCircleAll(_sm._attackPoint.position, 2);
        foreach (var wall in walls)
        {
            wall.GetComponent<Wall_Stats>().Damage(_sm._pStats.GetPlayerDamage());
        }
        */
        if (_sm.airStates.Contains(_sm.GetPreviousState()))
        {
            stateMachine.ChangeState(_sm.fallingState);
            _sm.anim.SetTrigger("Falling");
        }
        else
        {
            if (_sm.GetPreviousState().GetType() == typeof(Idle))
            {
                stateMachine.ChangeState(_sm.idleState);
                _sm.anim.SetTrigger("Idle");
            }
            else
            {
                stateMachine.ChangeState(_sm.movingState);
                _sm.anim.SetTrigger("Moving");
            }
        }
        
    }

    public float GetInputDirection()
    {
        switch (_sm.moveinput.y)
        {
            case 0:
                _sm._attackPoint.localPosition = new Vector3(2 * _sm.moveinput.x, 0, 0);
                return 0;
            case 1:
                _sm._attackPoint.localPosition = new Vector3(0, 2 * _sm.moveinput.y, 0);
                return 1;
            case -1:
                _sm._attackPoint.localPosition = new Vector3(0, 2 * _sm.moveinput.y, 0);
                return -1;
            default:
                _sm._attackPoint.localPosition = new Vector3(2 * 1, 0, 0);
                return _sm.moveinput.x;
        }
    }
}
