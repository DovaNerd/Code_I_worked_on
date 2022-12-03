using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : BaseState
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;

    //This is a variable the state uses in order to determine when it should transition to a specific state
    bool _grounded;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public Dashing(PlayerSM stateMachine) : base("Dash", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        _sm.AnimationMirror();
        _sm.dashes += 1;
        _sm.anim.SetTrigger("Dashing");
        _sm.DashCoroutine();
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();

        if (_sm.dashed == true)
        {
            if (_sm.airStates.Contains(_sm.GetPreviousState()))
                stateMachine.ChangeState(_sm.fallingState);
            if (_sm.groundedStates.Contains(_sm.GetPreviousState()) && _grounded)
                stateMachine.ChangeState(_sm.idleState);
        }
        
    }

    //This handles the updating of the physics of our gameobject and interactions that should be executed in our current state
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _grounded = _sm.characterbody.velocity.y < Mathf.Epsilon && _sm.dCollider.IsTouchingLayers();
    }
}
