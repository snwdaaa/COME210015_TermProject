using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 플레이어 이동 관련 부가 기능 처리
/// </summary>
public class PlayerMovementHelper : MonoBehaviour
{
    // 컴포넌트
    private PlayerMovement playerMovement;
    private PlayerStateMachine playerStateMachine;

    // ----------------- 점프, 추락 처리 -----------------
    public bool onAirStarted { get; private set; }
    public bool onAirType_Jump { get; private set; }
    public bool onAirType_Fall { get; private set; }
    public event Action PlayerJumpStartEvent;
    public event Action PlayerFallStartEvent;
    public event Action PlayerLandingEvent;

    public float fallHeight { get; private set; }
    private float playerMaxHeight; // 플레이어의 최대 위치 높이
    private float playerLandHeight; // 플레이어의 착지 위치 높이
    // ----------------- 점프, 추락 처리 -----------------

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 초기화
        onAirStarted = false;
        onAirType_Fall = false;
        onAirType_Jump = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckOnAirState();
    }

    // ----------------- 점프, 추락 처리 -----------------
    /// <summary>
    /// 현재 플레이어가 공중에 떠있는 상태가 점프에 의한 상태인지 점프 없이 떨어진 상태인지 체크
    /// </summary>
    private void CheckOnAirState()
    {
        if (!onAirStarted)
        {
            StartOnAirState();
        }
        else
        {
            EndOnAirState();
        }
    }

    /// <summary>
    /// 플레이어가 점프 or 추락을 시작할 때 호출
    /// </summary>
    private void StartOnAirState()
    {
        if (playerMovement.isJumping)
        {
            onAirStarted = true;
            onAirType_Jump = true;
            PlayerJumpStartEvent?.Invoke();

            // 높이 계산
            CalcFallStartHeight();
        }
        else if (playerStateMachine.CurrentMoveState == playerStateMachine.onAirState)
        {
            onAirStarted = true;
            onAirType_Fall = true;

            // 높이 계산
            CalcFallStartHeight();
      
            PlayerFallStartEvent?.Invoke(); // 이벤트 실행
        }
    }

    /// <summary>
    /// 플레이어가 착지할 때 호출
    /// </summary>
    private void EndOnAirState()
    {
        bool cond1 = playerStateMachine.CurrentMoveState == playerStateMachine.walkState;
        bool cond2 = playerStateMachine.CurrentMoveState == playerStateMachine.sprintState;
        bool cond3 = playerStateMachine.CurrentMoveState == playerStateMachine.crouchWalkState;
        bool cond4 = playerStateMachine.CurrentMoveState == playerStateMachine.idleState;

        if (onAirStarted && (cond1 || cond2 || cond3 || cond4))
        {
            onAirStarted = false;
            if (onAirType_Jump) onAirType_Jump = false;
            else if (onAirType_Fall) onAirType_Fall = false;

            // 높이 계산
            CalcFallEndHeight();

            PlayerLandingEvent?.Invoke(); // 이벤트 실행
        }
    }

    /// <summary>
    /// 플레이어가 추락을 시작하는 최대 높이를 계산 후 저장
    /// </summary>
    private void CalcFallStartHeight()
    {
        float startYPos = this.gameObject.transform.position.y;

        if (onAirType_Jump)
        {
            Vector3 playerMoveDir = playerMovement.moveVelocityWithGravity; // 이동 방향 벡터
            Vector3 playerMoveDirAxis = Vector3.zero; // 이동 방향을 지표면에 투사한 벡터와 같은 방향을 가지는 벡터 (이하 루트 벡터)

            // 이동 방향 벡터로 루트 벡터의 방향을 계산
            playerMoveDirAxis += transform.InverseTransformDirection(this.transform.forward) * playerMoveDir.z;
            playerMoveDirAxis += transform.InverseTransformDirection(this.transform.right) * playerMoveDir.x;

            // 중력의 영향을 받는 최대 점프 높이 (포물선 운동 최고점 높이 공식)
            // = (점프 시작시 이동 속도)^2 * sin^2(이동 방향 루트 벡터와 점프 이동 방향 사이 각) / 2 * gravity
            float angle = Vector3.Angle(playerMoveDir.normalized, playerMoveDirAxis.normalized);
            float sinVal = Mathf.Sin(angle * Mathf.Deg2Rad); // 라디안으로 변환
            sinVal = (sinVal > 0) ? sinVal : 1; // 제자리 점프 보정 -> 수직 방향 이동은 Sin(pi/2(90도)) = 1
            float jumpMaxHeight = (Mathf.Pow(playerMoveDir.magnitude, 2) * Mathf.Pow(sinVal, 2)) / (2 * Mathf.Abs(Physics.gravity.y)); // 중력값 항상 양수여야 함
            playerMaxHeight = startYPos + jumpMaxHeight;
        }
        else if (onAirType_Fall)
        {
            playerMaxHeight = startYPos;
        }
    }

    /// <summary>
    /// 플레이어가 지면에 착지한 위치의 높이와 최종 낙하 높이를 계산 후 저장
    /// </summary>
    private void CalcFallEndHeight()
    {
        playerLandHeight = this.gameObject.transform.position.y;
        fallHeight = playerMaxHeight - playerLandHeight;
    }
    // ----------------- 점프, 추락 처리 -----------------
}