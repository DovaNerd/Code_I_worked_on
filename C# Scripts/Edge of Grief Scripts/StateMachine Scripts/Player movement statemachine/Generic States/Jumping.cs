using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a generic state its inherits directly from the Air general state
public class Jumping : Air
{
    //This is a reference to the statemachine the state is part of
    private PlayerSM _sm;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public Jumping(PlayerSM stateMachine) : base ("Jumping", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        _sm.Jump();
        _sm.AnimationPlay("Jumping");
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (Crest() || !_sm.jumpAction)
            stateMachine.ChangeState(_sm.fallingState);
        else if (WallTouch())
            stateMachine.ChangeState(_sm.wallslideState);
    }

    //This handles the updating of the physics of our gameobject and interactions that should be executed in our current state
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _sm.AnimationMirror();
        _sm.Move();
    }

    //We use this function to determine whether or not the player has begun going down in the jump
    private bool Crest()
    {
        if (_sm.characterRigidbody.velocity.y < 0)
            return true;
        else
            return false;
    }

    private bool WallTouch()
    {
        if (_sm.lCollider.IsTouchingLayers(LayerMask.GetMask("Wall")) || _sm.rCollider.IsTouchingLayers(LayerMask.GetMask("Wall")))
            return true;
        else
            return false;
    }
}
