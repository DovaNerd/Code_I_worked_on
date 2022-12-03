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
        _sm.anim.SetTrigger("Jumping");
        _sm.Jump();
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (Crest())
            stateMachine.ChangeState(_sm.fallingState);
    }

    //This handles the updating of the physics of our gameobject and interactions that should be executed in our current state
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
    }

    //We use this function to determine whether or not the player has begun going down in the jump
    private bool Crest()
    {
        if (_sm.characterbody.velocity.y < 0)
            return true;
        else
            return false;
    }
}
