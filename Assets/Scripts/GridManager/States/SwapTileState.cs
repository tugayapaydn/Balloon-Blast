using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SwapTileState : State
{

    public SwapTileState(GridManager gridManager, StateMachine stateMachine)
        :base (gridManager, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        destItems.Clear();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!gridManager.CheckSwap(selected1, selected2))
            return;

        List<Tuple<int, int>> items = gridManager.CheckMatchingItems(selected1);
        List<Tuple<int, int>> items2 = gridManager.CheckMatchingItems(selected2);
        Debug.Log(items.Count.ToString() + ", " + items2.Count.ToString());
       
        if (items.Count < 3 && items2.Count < 3)
        {
            stateMachine.ChangeState(gridManager.swapTileBackState);
        }
        else
        {
            if (items.Count >= 3)
                destItems.AddRange(items);
            if (items2.Count >= 3)
                destItems.AddRange(items2);

            stateMachine.ChangeState(gridManager.destroyingState);
        }
    }
}

