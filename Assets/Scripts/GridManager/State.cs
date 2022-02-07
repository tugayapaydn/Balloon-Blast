using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class State
{
    protected GridManager gridManager;
    protected StateMachine stateMachine;

    protected static Tuple<int, int> selected1;
    protected static Tuple<int, int> selected2;
    protected static List<Tuple<int, int>> destItems = new List<Tuple<int, int>>();
    protected static List<Tuple<int, int>> shiftingItems = new List<Tuple<int, int>>();

    protected State(GridManager gridManager, StateMachine stateMachine)
    {
        this.gridManager = gridManager;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        Debug.Log("Current State: " + stateMachine.CurrentState.GetType().ToString());
    }

    public virtual void HandleInput()
    {
    }
    public virtual void LogicUpdate()
    {
    }
    public virtual void PhysicsUpdate()
    {
    }
    public virtual void Exit()
    {
    }
}