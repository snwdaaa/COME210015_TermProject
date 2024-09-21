using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandState : IState
{
    private PlayerMovement pm;

    // 생성자
    public StandState(PlayerMovement playerMovement)
    {
        this.pm = playerMovement;
    }

    public void Enter()
    {
        pm.postureState = PlayerMovement.PostureState.Stand;
    }

    public void Update()
    {
        // Crouch 상태 전이 검사
        if (pm.CheckCrouch())
        {
            pm.playerStateMachine.TransferPostureState(pm.playerStateMachine.crouchState);
        }
    }

    public void Exit()
    {

    }
}
