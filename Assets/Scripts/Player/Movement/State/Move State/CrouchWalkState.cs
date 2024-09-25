using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchWalkState : IState
{
    private PlayerMovement pm;

    // 생성자
    public CrouchWalkState(PlayerMovement playerMovement)
    {
        this.pm = playerMovement;
    }

    public void Enter()
    {
        pm.moveState = PlayerMovement.MoveState.CrouchWalk;
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

        if (!pm.isCrouching)
        {
            // Walk 상태 전이 검사
            if (pm.currentSpeed > 0)
            {
                pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.walkState);
            }
        }

        // Slide 상태 전이 검사
        if (PhysicsUtil.IsOnSteepSlope(pm.gameObject, ref pm.slopeHit))
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.slideState);
        }

        // OnAir 상태 전이 검사
        if ((pm.enableDuckJump && pm.CheckJump()) || PhysicsUtil.IsGrounded(pm.gameObject))
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.onAirState);
        }

    }

    public void Exit()
    {

    }
}
