using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class StateMachine
{
    public State CurrentState { get; private set; }

    public void Initialize(State startState)
    {
        CurrentState = startState;
        startState.Enter();
    }

    public void ChangeState(State newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        newState.Enter();
    }
}
