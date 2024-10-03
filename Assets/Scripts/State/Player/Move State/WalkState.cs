using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : IState
{
    private PlayerMovement pm;

    // 생성자
    public WalkState(PlayerMovement playerMovement)
    {
        this.pm = playerMovement;
    }

    public void Enter()
    {
        pm.moveState = PlayerMovement.MoveState.Walk;
        pm.StartWalk();
    }

    public void Update()
    {
        pm.Move();

        // ------- 전이 검사 -------

        // Slide 상태 전이 검사
        if (PhysicsUtil.IsOnSteepSlope(pm.gameObject, ref pm.slopeHit))
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.slideState);
        }

        // OnAir 상태 전이 검사
        if (pm.isJumping || !PhysicsUtil.IsGrounded(pm.gameObject))
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.onAirState);
        }

        // Sprint 상태 전이 검사
        if (pm.CheckSprint())
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.sprintState);
        }

        // CrouchWalk 상태 전이 검사
        if (pm.isCrouching && pm.currentSpeed == pm.crouchSpeed) // 앉은 상태 조건 추가
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.crouchWalkState);
        }

        // Idle 상태 전이 검사
        if (pm.currentSpeed == 0)
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.idleState);
        }
    }

    public void Exit()
    {

    }
}
