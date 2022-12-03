using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

//This is a generic state its inherits directly from the grounded general state
public class Idle : Grounded
{
    //This is a reference to the statemachine the state is part of
    private PlayerSM _sm;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public Idle(PlayerSM stateMachine) : base("Idle", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        if(_sm.GetPreviousState() != null)
            _sm.anim.SetTrigger("Idle");


        _sm.dashed = false;
        _sm.healAction = false;
        _sm.projectileAction = false;

        _sm.dashes = 0;
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_sm.moveinput.x != 0)
        {
            stateMachine.ChangeState(_sm.movingState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
