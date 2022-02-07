using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class IdleState : State
{
    private Vector3 mouseDown;

    public IdleState(GridManager gridManager, StateMachine stateMachine)
        :base(gridManager, stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        selected1 = null;
        selected2 = null;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (Input.GetMouseButtonDown(0))
        {
            this.mouseDown = mouse;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Tuple<int, int> direction = gridManager.GetMouseDirection(mouseDown, mouse);
            selected1 = gridManager.GetGridItem(mouseDown);
            if (selected1 != null)
            {
                selected2 = new Tuple<int, int>(selected1.Item1 + direction.Item1, selected1.Item2 + direction.Item2);

                if (gridManager.MakeMove(selected1, selected2))
                    stateMachine.ChangeState(gridManager.swapTileState);
            }
        }
    }
}