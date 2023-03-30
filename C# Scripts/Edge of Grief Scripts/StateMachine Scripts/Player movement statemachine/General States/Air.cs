using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This is a general state used for common code shared between states
public class Air : BaseState
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;
    //This is a variable the state uses in order to determine when it should transition to a specific state
    bool _grounded;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public Air (string name, PlayerSM stateMachine) : base (name, stateMachine)
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
        //Debug.Log(_sm.GetCurrentState());
        if (_grounded)
            stateMachine.ChangeState(_sm.idleState);
        else if (_sm.dashAction)
        {
            stateMachine.ChangeState(_sm.dashingState);
        }
        else if (_sm.attackAction)
        {
            stateMachine.ChangeState(_sm.attackState);
        }
        else if (_sm.menuAction)
        {
            stateMachine.ChangeState(_sm.menuState);
        }
        else if (_sm.projectileAction)
        {
            stateMachine.ChangeState(_sm.projectileState);
        }
    }

    //This handles the updating of the physics of our gameobject and interactions that should be executed in our current state
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _grounded = _sm.dCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }
}
