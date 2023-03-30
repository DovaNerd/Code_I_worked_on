using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a generic state its inherits directly from base state however does not encapsulate code from other states
public class Falling : Air
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;

    //This is a variable the state uses in order to determine when it should transition to a specific state
    bool _grounded;

    //The variable we use to amplifiy gravity for the player when they enter the state
    float lowJumpMultiplier = 5f;
    float fallMultiplier = 2.5f;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public Falling (PlayerSM stateMachine) : base ("Falling", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        
        if (_sm.GetPreviousState().GetType() != typeof(Attack))
            _sm.AnimationPlay("Falling");
    }
    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (WallTouch() && _sm.characterRigidbody.velocity.y < 0)
            stateMachine.ChangeState(_sm.wallslideState);

        if (_sm.dashReady == true && _sm.dashAction == true)
        {
            stateMachine.ChangeState(_sm.dashingState);
        }
    }

    //This handles the updating of the physics of our gameobject and interactions that should be executed in our current state
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _sm.AnimationMirror();
        _sm.Move();

        if (_sm.GetPreviousState().GetType() == typeof(Jumping))
        {
            if (_sm.characterRigidbody.velocity.y > 0 && !_sm.jumpAction)
            {
                _sm.characterRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            else
            {
                _sm.characterRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
        }
        _grounded = _sm.characterRigidbody.velocity.y < Mathf.Epsilon && _sm.dCollider.IsTouchingLayers();
    }

    //We use this function to determine whether or not the player has touched a wall on either the left or right side
    private bool WallTouch()
    {
        if (_sm.lCollider.IsTouchingLayers(LayerMask.GetMask("Wall")) || _sm.rCollider.IsTouchingLayers(LayerMask.GetMask("Wall")))
            return true;
        else
            return false;
    }
}
