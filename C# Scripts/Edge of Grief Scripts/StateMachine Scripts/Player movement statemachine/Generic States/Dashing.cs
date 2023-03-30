using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
        _sm.dashed = false;//hasnt dashed yet(needed for bug fix)
        _sm.dashReady = false; ///fix where variables are reset
        _sm.dashAction = false;
        _sm.DashCoroutine();
        Physics2D.IgnoreLayerCollision(7, 3, true);
        _sm.AnimationMirror();
        _sm.AnimationPlay("Dashing");
        AfterImagePool.Instance.GetFromPool(); // Getting a reference to an after image object from the object pool
        _sm.lastImageXpos = _sm.transform.position.x;
        if (_sm.playerStats.GetDashUpgrade())
            _sm.dashCol.enabled = true;
    
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();

        if (_sm.dashed == true)
        {
            _sm.dashCol.enabled = false;
            if (_grounded)
            {
                stateMachine.ChangeState(_sm.idleState);
            }
            else
            {
                stateMachine.ChangeState(_sm.fallingState);
            }
                
        }
        
    }

    //This handles the updating of the physics of our gameobject and interactions that should be executed in our current state
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _grounded = _sm.characterRigidbody.velocity.y < Mathf.Epsilon && _sm.dCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    public override void Exit()
    {
        base.Exit();
        Physics2D.IgnoreLayerCollision(7, 3, false);
    }
}
