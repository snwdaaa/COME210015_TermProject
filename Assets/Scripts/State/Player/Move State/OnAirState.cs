using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAirState : IState
{
    private PlayerMovement pm;

    // 생성자
    public OnAirState(PlayerMovement playerMovement)
    {
        this.pm = playerMovement;
    }

    public void Enter()
    {
        pm.moveState = PlayerMovement.MoveState.OnAir;
    }

    public void Update()
    {
        pm.Move();

        // Idle 상태 전이 검사
        if (PhysicsUtil.IsGrounded(pm.gameObject))
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.idleState);
        }

        // Slide 상태 전이 검사
        if (PhysicsUtil.IsOnSteepSlope(pm.gameObject, ref pm.slopeHit))
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.slideState);
        }
    }

    public void Exit()
    {

    }
}
