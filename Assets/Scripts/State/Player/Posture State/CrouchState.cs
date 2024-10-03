using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : IState
{
    private PlayerMovement pm;

    // 생성자
    public CrouchState(PlayerMovement playerMovement)
    {
        this.pm = playerMovement;
    }

    public void Enter()
    {
        pm.postureState = PlayerMovement.PostureState.Crouch;
    }

    public void Update()
    {
        if (pm.CheckCrouch())
        {
            pm.StartCrouch();
        }
        else
        {
            pm.EndCrouch();
        }

        if (!pm.isCrouching)
        {
            pm.playerStateMachine.TransferPostureState(pm.playerStateMachine.standState);
        }
    }

    public void Exit()
    {

    }
}
