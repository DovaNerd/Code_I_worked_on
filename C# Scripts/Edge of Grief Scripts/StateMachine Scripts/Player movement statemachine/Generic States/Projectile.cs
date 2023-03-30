using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a generic state its inherits directly from base state however does not encapsulate code from other states
public class Projectile : BaseState
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;
    
    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public Projectile(PlayerSM stateMachine) : base("Heal", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        _sm.projectileAction = false;
        _sm.magicReady = false;
        _sm.MagicCoroutine();
        _sm.Projectile();
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_sm.airStates.Contains(_sm.GetPreviousState()))
            stateMachine.ChangeState(_sm.fallingState);
        else
        {
            stateMachine.ChangeState(_sm.idleState);
            _sm.anim.SetTrigger("Idle");
        }
    }
}
