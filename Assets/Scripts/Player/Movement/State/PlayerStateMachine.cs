using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State 패턴으로 플레이어 상태 관리
/// </summary>
public class PlayerStateMachine : MonoBehaviour
{
    private PlayerMovement playerMovement;

    public IState CurrentMoveState { get; private set; } // 현재 움직임 상태
    public IState CurrentPostureState { get; private set; } // 현재 자세 상태

    // MoveState
    public IdleState idleState;
    public WalkState walkState;
    public SprintState sprintState;
    public CrouchWalkState crouchWalkState;
    public SlideState slideState;
    public OnAirState onAirState;

    // PostureState
    public StandState standState;
    public CrouchState crouchState;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        // 모든 State 생성자 호출
        this.idleState = new IdleState(playerMovement);
        this.walkState = new WalkState(playerMovement);
        this.sprintState = new SprintState(playerMovement);
        this.crouchWalkState = new CrouchWalkState(playerMovement);
        this.slideState = new SlideState(playerMovement);
        this.onAirState = new OnAirState(playerMovement);

        this.standState = new StandState(playerMovement);
        this.crouchState = new CrouchState(playerMovement);
    }

    /// <summary>
    /// 시작 State 초기화
    /// </summary>
    /// <param name="startingMoveState">시작 Move State</param>
    /// <param name="startingPostureState">시작 Posture State</param>
    public void Initialize(IState startingMoveState, IState startingPostureState)
    {
        CurrentMoveState = startingMoveState;
        startingMoveState.Enter();
        CurrentPostureState = startingPostureState;
        startingPostureState.Enter();
    }

    public void FixedUpdate()
    {
        if (CurrentMoveState != null)
        {
            CurrentMoveState.Update();
        }

        if (CurrentPostureState != null)
        {
            CurrentPostureState.Update();
        }
    }

    /// <summary>
    /// Move State 전이
    /// </summary>
    /// <param name="nextMoveState">전이할 Move State</param>
    public void TransferMoveState(IState nextMoveState)
    {
        CurrentMoveState.Exit(); // 현재 상태 Exit
        CurrentMoveState = nextMoveState; // 상태 변수 Update
        nextMoveState.Enter(); // 다음 상태 Enter
    }

    /// <summary>
    /// Posture State 전이
    /// </summary>
    /// <param name="nextPostureState">전이할 Posture State</param>
    public void TransferPostureState(IState nextPostureState)
    {
        CurrentPostureState.Exit(); // 현재 상태 Exit
        CurrentPostureState = nextPostureState; // 상태 변수 Update
        nextPostureState.Enter(); // 다음 상태 Enter
    }
}
