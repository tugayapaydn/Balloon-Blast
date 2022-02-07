using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SwapTileBackState : State
{
    public SwapTileBackState(GridManager gridManager, StateMachine stateMachine)
        : base(gridManager, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        gridManager.MakeMove(selected1, selected2);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!gridManager.CheckSwap(selected1, selected2))
            return;

        stateMachine.ChangeState(gridManager.idleState);
    }
}

