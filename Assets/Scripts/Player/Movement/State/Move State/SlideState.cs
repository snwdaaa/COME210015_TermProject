using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideState : IState
{
    private PlayerMovement pm;

    // 생성자
    public SlideState(PlayerMovement playerMovement)
    {
        this.pm = playerMovement;
    }

    public void Enter()
    {
        pm.moveState = PlayerMovement.MoveState.Slide;
    }

    public void Update()
    {
        // Idle 상태 전이 검사
        if (!PhysicsUtil.IsOnSteepSlope(pm.gameObject, ref pm.slopeHit))
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.idleState);
        }
        else
        {
            pm.SlideSlope();
        }
    }

    public void Exit()
    {

    }
}
