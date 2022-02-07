using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ShiftingState : State
{
    public ShiftingState(GridManager gridManager, StateMachine stateMachine)
        :base (gridManager, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        shiftingItems.Clear();
        gridManager.SetShiftNullTiles(shiftingItems);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        /*Debug.Log("Shift update");
        foreach(Tuple<int,int> t in shiftingItems)
        {
            Debug.Log(t.ToString());
        }*/
        //gridManager.nullCheck();
        if (!gridManager.CheckShift(shiftingItems))
            return;

        HashSet<Tuple<int, int>> matchingItems = gridManager.CheckMatchingItems();
        if (matchingItems.Count > 0)
        {
            destItems.AddRange(matchingItems);
            stateMachine.ChangeState(gridManager.destroyingState);
        }
        else
        {
            stateMachine.ChangeState(gridManager.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        shiftingItems.Clear();
    }
}
