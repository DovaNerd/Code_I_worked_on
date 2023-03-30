using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a generic state its inherits directly from the Air general state
public class WallSlide : Air
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;

    //This is to slow the player down on a wall
    float slideSpeed = 5.0f;

    // Time spent on the wall
    float timer;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public WallSlide (PlayerSM stateMachine) : base ("WallSlide", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        _sm.AnimationMirror();
        _sm.AnimationPlay("WallSlide");
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_sm.moveinput.x < 0 && _sm.rCollider.IsTouchingLayers(LayerMask.GetMask("Wall")) || _sm.moveinput.x > 0 && _sm.lCollider.IsTouchingLayers(LayerMask.GetMask("Wall")))
            stateMachine.ChangeState(_sm.walljumpState);
        else if (!_sm.rCollider.IsTouchingLayers(LayerMask.GetMask("Wall")) && !_sm.lCollider.IsTouchingLayers(LayerMask.GetMask("Wall")))
            stateMachine.ChangeState(_sm.fallingState);
    }

    //This handles the updating of the physics of our gameobject and interactions that should be executed in our current state
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        if (_sm.holdWall == false)
        {
            _sm.characterRigidbody.gravityScale = 2;
             if (timer <= 1)
                 timer += Time.deltaTime;

            if ((_sm.moveinput.x < 0 && _sm.lCollider.IsTouchingLayers(LayerMask.GetMask("Wall"))) || (_sm.moveinput.x > 0 && _sm.rCollider.IsTouchingLayers(LayerMask.GetMask("Wall"))))
            {
                _sm.characterRigidbody.velocity = new Vector2(0, -slideSpeed * timer);
            }
            else
            {
                _sm.Move();
                _sm.characterRigidbody.velocity = new Vector2(_sm.characterRigidbody.velocity.x, -slideSpeed * timer);
            }
        }
        else
        {
            timer = 0;
            _sm.characterRigidbody.gravityScale = 0;
            _sm.characterRigidbody.velocity = new Vector2(_sm.characterRigidbody.velocity.x, 0);
        }
    }

    //This is code executed when the state is changing out it runs just before it changes
    public override void Exit()
    {
        base.Exit();
        _sm.characterRigidbody.gravityScale = 2;
        timer = 0;
    }
}
