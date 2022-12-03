using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

//This is a generic state its inherits directly from the grounded general state
public class Moving : Grounded
{
    //This is a reference to the statemachine the state is part of
    private PlayerSM _sm;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public Moving(PlayerSM stateMachine) : base("Moving", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        _sm.anim.SetTrigger("Moving");
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_sm.moveinput.x == 0)
        {
            stateMachine.ChangeState(_sm.idleState);
        }
    }

    //This handles the updating of the physics of our gameobject and interactions that should be executed in our current state
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _sm.Move();
    }
}
