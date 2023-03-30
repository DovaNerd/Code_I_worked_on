using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : BaseState
{
    //This is a reference to the statemachine the state is part of
    private PlayerSM _sm;

    //This is the constructor for the state which assigns the reference to the overall statemachine and then passes the reference to its base state
    public Menu(PlayerSM stateMachine) : base("Menu", stateMachine)
    {
        _sm = stateMachine;
    }

    public override void Enter()
    {
        _sm.anim.SetTrigger("Idle");
        base.Enter();
    }

    public override void Exit() 
    { 
        base.Exit();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_sm.menuObj == null)
        {
            stateMachine.ChangeState(_sm.idleState);
        }
    }
}
