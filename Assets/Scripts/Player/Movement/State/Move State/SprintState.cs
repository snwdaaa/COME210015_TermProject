using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintState : IState
{
    private PlayerMovement pm;

    // 생성자
    public SprintState(PlayerMovement playerMovement)
    {
        this.pm = playerMovement;
    }

    public void Enter()
    {
        pm.moveState = PlayerMovement.MoveState.Sprint;
        pm.StartSprint();
    }

    public void Update()
    {
        pm.Move();
        pm.CheckJump();

        // ------- 전이 검사 -------

        // Idle 상태 전이 검사
        if (pm.currentSpeed == 0)
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.idleState);
        }

        // Slide 상태 전이 검사
        if (PhysicsUtil.IsOnSteepSlope(pm.gameObject, ref pm.slopeHit))
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.slideState);
        }

        // Walk 상태 전이 검사
        if (pm.currentSpeed > 0 && !pm.CheckSprint())
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.walkState);
        }

        // OnAir 상태 전이 검사
        if (pm.CheckJump() || !pm.characterController.isGrounded)
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.onAirState);
        }
    }

    public void Exit()
    {
        pm.EndSprint();
    }
}
