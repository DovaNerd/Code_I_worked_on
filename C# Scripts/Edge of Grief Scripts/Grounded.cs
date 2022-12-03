using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

//This is a generic state its inherits directly from base state however does not encapsulate code from other states
public class Grounded : BaseState
{
    //This is a reference to the statemachine the state is part of
    protected PlayerSM _sm;

    //This is a variable the state uses in order to determine when it should transition to a specific state
    public Grounded(string name, PlayerSM stateMachine) : base(name, stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_sm.jumpAction)
        {
            stateMachine.ChangeState(_sm.jumpingState);
        }
        else if (_sm.dashAction)
        {
            stateMachine.ChangeState(_sm.dashingState);
        }
        else if (_sm.attackAction)
        {
            stateMachine.ChangeState(_sm.attackState);
        }
        else if (_sm.projectileAction)
        {
            stateMachine.ChangeState(_sm.projectileState);
        }
        else if (_sm.healAction)
        {
            stateMachine.ChangeState(_sm.healState);
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _sm.AnimationMirror();
    }
}
