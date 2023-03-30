using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is base stateMachine class that all statemachines we create will inheirt from
public class StateMachine : MonoBehaviour
{
    //These memeber variables are used to store both previous and currentState for the statemachines
    protected BaseState currentState;
    protected BaseState previousState;

    //This function allows our created state machine to set its initial state on startup
    void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    //We are using both the update and LateUpdate functions from mono behavior in order run the updates for our current state
    void Update()
    {
        if (currentState != null)
            currentState.UpdateLogic();
    }

    void LateUpdate()
    {
        if (currentState != null)
            currentState.UpdatePhysics();
    }

    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    //This function handles the transitions between states by calling the current states exit function and the new states enter function
    public void ChangeState(BaseState newState)
    {
        currentState.Exit();
        previousState = currentState;
        currentState = newState;
        newState.Enter();
    }

    //This let's us get what our current state is, if we need it in another script
    public BaseState GetCurrentState()
    {
        return currentState;
    }
}
