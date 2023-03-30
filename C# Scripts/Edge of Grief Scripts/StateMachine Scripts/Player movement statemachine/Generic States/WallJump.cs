using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a generic state its inherits directly from the Air general state
public class WallJump : Air
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public WallJump (PlayerSM stateMachine) : base ("WallJump", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        _sm.AnimationPlay("Jumping");
        _sm.characterRigidbody.velocity = Vector2.zero;
        _sm.characterRigidbody.AddForce(new Vector2(7 * _sm.moveinput.x, 13), ForceMode2D.Impulse);
        stateMachine.ChangeState(_sm.fallingState);
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _sm.AnimationMirror();
        _sm.Move();
    }
}
