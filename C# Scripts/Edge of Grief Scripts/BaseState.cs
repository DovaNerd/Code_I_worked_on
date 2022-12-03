using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This is the base state that all states will be inheriting from (unless they inherit from a general state which still inheirits from it)
public class BaseState
{
    //The name of the state
    public string name;

    //This is a reference the state machine the state is apart of
    protected StateMachine stateMachine;

    //This is a constructor that sets the name of the current state machine as well as assigns the reference to the respective statemachine
    public BaseState(string name, StateMachine stateMachine)
    {
        this.name = name;
        this.stateMachine = stateMachine;
    }

    //These are a base functions that are used by inherited states in order to provide ease in creating functions for the state
    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }
}
