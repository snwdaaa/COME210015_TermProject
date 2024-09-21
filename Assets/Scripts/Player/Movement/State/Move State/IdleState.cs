using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private PlayerMovement pm;

    // 생성자
    public IdleState(PlayerMovement playerMovement)
    {
        this.pm = playerMovement;
    }

    public void Enter()
    {
        pm.moveState = PlayerMovement.MoveState.Idle;
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
        if (pm.CheckJump() && pm.characterController.isGrounded)
        {
            pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.onAirState);
        }
        
        if (pm.currentSpeed > 0)
        {
            if (pm.isCrouching) // CrouchWalk 상태 전이 검사
            {
                pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.crouchWalkState);
            }
            else // Walk 상태 전이 검사
            {
                pm.playerStateMachine.TransferMoveState(pm.playerStateMachine.walkState);
            }
        }

    }

    public void Exit()
    {

    }
}
