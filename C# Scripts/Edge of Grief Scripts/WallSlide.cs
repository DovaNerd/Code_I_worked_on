using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a generic state its inherits directly from the Air general state
public class WallSlide : Air
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public WallSlide (PlayerSM stateMachine) : base ("WallSlide", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        _sm.anim.SetTrigger("WallSlide");
        _sm.characterbody.gravityScale = 0.25f;
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_sm.moveinput.x < 0 && _sm.rCollider.IsTouchingLayers() || _sm.moveinput.x > 0 && _sm.lCollider.IsTouchingLayers())
            stateMachine.ChangeState(_sm.walljumpState);
    }

    //This handles the updating of the physics of our gameobject and interactions that should be executed in our current state
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
    }

    //This is code executed when the state is changing out it runs just before it changes
    public override void Exit()
    {
        base.Exit();
        _sm.characterbody.gravityScale = 2;
    }
}
