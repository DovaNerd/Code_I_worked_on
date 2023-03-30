using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : BaseState
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;

    //This is a variable the state uses in order to determine when it should transition to a specific state
    bool _grounded;

    public Damage(PlayerSM stateMachine) : base("Damage", stateMachine) 
    {
        _sm = stateMachine;
    }

    // Code ran when we enter the state
    public override void Enter()
    {
        base.Enter();
        _sm.StartCoroutine(_sm.Damage());
        _sm.AnimationPlay("Damage");
        //_sm.characterRigidbody.velocity = Vector3.zero;
        _sm.characterRigidbody.AddForce(new Vector2(10 * (_sm.playerTpose.transform.localScale.z * -1), 9), ForceMode2D.Impulse);
        CinemachineShake.Instance.ShakeCamera(3f, 0.25f);
    }

    // Checking when we should change states
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Debug.Log("Damage");
        if (_grounded)
        {
            // Allowing the character to move if they have completed the knockback animation
            _sm.Move();
            if (_sm.damageStop)
            {
                stateMachine.ChangeState(_sm.idleState); 
            }
        }
    }

    // Handling the physics while we are in the state
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _grounded = _sm.characterRigidbody.velocity.y < Mathf.Epsilon && _sm.dCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    // Code ran when we exit the state
    public override void Exit()
    {
        _sm.damageStop= false;
        base.Exit();
    }
}
