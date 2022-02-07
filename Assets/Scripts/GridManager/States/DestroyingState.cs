using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DestroyingState : State
{
    public DestroyingState(GridManager gridManager, StateMachine stateMachine)
        :base(gridManager, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        gridManager.SetDestroyItems(destItems);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!gridManager.CheckDestroying(destItems))
            return;

        gridManager.DestroyItems(destItems);
        gridManager.soundManager.PlayClip(gridManager.soundManager.baloonPopClipList[UnityEngine.Random.Range(0, gridManager.soundManager.baloonPopClipList.Count)]);
        stateMachine.ChangeState(gridManager.shiftingState);
    }

    public override void Exit()
    {
        base.Exit();
        destItems.Clear();
    }

}
