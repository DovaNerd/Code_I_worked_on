using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : BaseState
{
    //This is a reference to the statemachine the state is part of
    PlayerSM _sm;

    public Death(PlayerSM stateMachine) : base("Damage", stateMachine)
    {
        _sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        _sm.AnimationPlay("Death");
        LevelLoader.Instance.StartCoroutine(LevelLoader.Instance.Close());
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (LevelLoader.Instance.closeFinished)
        {
            LevelLoader.Instance.closeFinished = false;
            _sm.playerStats.Respawn();
        }
    }

    public override void Exit() 
    {
        base.Exit();
        LevelLoader.Instance.StartCoroutine(LevelLoader.Instance.Open());
    }
}
