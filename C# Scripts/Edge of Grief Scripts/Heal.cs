using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a generic state its inherits directly from base state however does not encapsulate code from other states
public class Heal : BaseState
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public Heal (PlayerSM stateMachine) : base ("Heal", stateMachine)
    {
        _sm = stateMachine;
    }

    //This executes code when the statemachine is entered in the statemachine and runs it once
    public override void Enter()
    {
        base.Enter();
        _sm.anim.SetTrigger("Healing");
    }

    //This update logic function is used for determining whether the current state needs to change and when it should change
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _sm._pStats.heal();
        stateMachine.ChangeState(_sm.idleState);
    }
}
