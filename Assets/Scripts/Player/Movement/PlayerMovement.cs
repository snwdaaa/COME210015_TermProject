using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// 플레이어 이동 스크립트
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // 컴포넌트
    public CharacterController characterController { get; private set; }
    public PlayerKeyInput playerKeyInput { get; private set; }
    public PlayerStateMachine playerStateMachine { get; private set; }
    public PlayerStamina playerStamina { get; private set; }
    public PlayerHealth playerHealth { get; private set; }

    public enum MoveState { Idle, Walk, Sprint, CrouchWalk, Slide, OnAir };
    public enum PostureState { Stand, Crouch };
    [Header("상태")] // 인스펙터 확인용
    public MoveState moveState = MoveState.Idle;
    public PostureState postureState = PostureState.Stand;

    [Header("이동 속도 설정")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    public float crouchSpeed = 2f;
    [SerializeField] private float moveSpeedSmoothTime = 0.2f;

    [Header("점프 설정")]
    [SerializeField] private float jumpSpeed = 10f;
    [Range(0.01f, 1f)] public float airControlPercentage = 1f; // 점프 중 조작 가능한 정도
    public bool enableDuckJump = false;
    private float currentYSpeed; // 현재 y 방향 속도
    public bool isJumping { get; private set; }
    public event Action StartJumpAction;
    private bool keyPressed_Jump;

    [Header("앉기 설정")]
    [SerializeField] private float standHeight = 1.7f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchSmoothTime = 1f;
    [SerializeField] private float standSmoothTime = 1f;
    [SerializeField] private Vector3 standCenter = new Vector3(0, -0.4f, 0);
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, -0.2f, 0);
    public bool isCrouching { get; set; }

    [Header("경사면 설정")]
    [SerializeField] private float slopeSlidingSpeed = 3f;
    [SerializeField] private float slopeDownForce = 1f;
    [SerializeField] [Range(0f, 1f)] private float slopeControlRatio = 0.5f; // 경사면에서 적용 가능한 조작 비율
    [HideInInspector] public RaycastHit slopeHit;
    private float slopeForceTmp;

    // 레퍼런스 & temp 변수
    private float moveSpeedRef;
    private Vector3 crouchCenterRef;
    private float currentHeightRef;
    private Vector3 groundedCheckRayStartPos;
    public Vector3 moveVelocity { get; private set; }
    public Vector3 moveVelocityWithGravity { get; private set; }

    // 현재 이동 속력 -> 캐릭터 컨트롤러의 속도 벡터의 크기
    public float currentSpeed => new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
    public float currentHeight => characterController.height;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerKeyInput = GetComponent<PlayerKeyInput>();
        playerStamina = GetComponent<PlayerStamina>();
        playerHealth = GetComponent<PlayerHealth>();

        slopeForceTmp = slopeDownForce;

        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerStateMachine.Initialize(playerStateMachine.idleState, playerStateMachine.standState); // State 초기화
    }

    private void Update()
    {
        CheckKeyInput();
    }

    private void FixedUpdate()
    {
        CheckJump();
        CalcMoveVelocity();

        if (playerHealth.isDied) // 플레이어가 사망한 경우 못 움직이게
        {
            moveVelocity = Vector3.zero;
            moveVelocityWithGravity = Vector3.zero;
            return;
        }
    }

    public void ChangeSpeed(float newWalkSpeed, float newSprintSpeed)
    {
        walkSpeed = newWalkSpeed;
        sprintSpeed = newSprintSpeed;
        moveSpeed = newWalkSpeed;
    }

    private void CheckKeyInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            keyPressed_Jump = true;
        }
    }

    /// <summary>
    /// Movement 기능 활성화
    /// </summary>
    public void EnableMovement()
    {
        moveSpeed = walkSpeed;
        this.enabled = true;
    }

    /// <summary>
    /// Movement 기능 비활성화
    /// </summary>
    public void DisableMovement()
    {
        moveVelocity *= 0;
        moveVelocityWithGravity *= 0;
        this.enabled = false;
    }

    /// <summary>
    /// 입력받은 방향으로 플레이어 이동시킬 값을 매 프레임마다 계산
    /// </summary>
    /// <param name="moveInput">키 입력 벡터</param>
    private void CalcMoveVelocity()
    {
        Vector2 moveInput = playerKeyInput.moveInput;

        // 이동 속력
        float targetSpeed = moveSpeed * moveInput.magnitude; // 목표 속력 = 최대 속력 * 입력 벡터 크기
        // 가속도 smoothTime 설정. 떠있는 동안에는 airControlPercentage에 의해 조작 속도 조절
        float smoothTime = PhysicsUtil.IsGrounded(this.gameObject) ? moveSpeedSmoothTime : moveSpeedSmoothTime / airControlPercentage;
        targetSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref moveSpeedRef, smoothTime); // 이동 속력 점진적으로 증가
        currentYSpeed += Physics.gravity.y * Time.deltaTime; // 아래에 물체가 없는 경우에 y 속력이 중력에 영향을 받게 함

        // 방향
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x; // 이동 방향

        // 최종 속도 계산 결과
        moveVelocity = moveDirection * targetSpeed;
        moveVelocityWithGravity = moveDirection * targetSpeed + transform.up * currentYSpeed; // 중력 영향 받게 함
    }

    /// <summary>
    /// 가파른 경사면에서 슬라이딩하게 함
    /// </summary>
    public void SlideSlope()
    {
        Vector3 slideDir = Vector3.ProjectOnPlane(Vector3.down, Vector3.Normalize(slopeHit.normal)); // 슬라이딩 방향 = Vector3.down을 법선벡터가 slopeHit.normal인 표면에 투영해서 생긴 벡터
        Vector3 slideVelocity = slideDir * slopeSlidingSpeed + Vector3.down * slopeDownForce; // 경사면 내려갈 때 떨림 현상 방지하기 위해 아래쪽으로 힘 적용
        characterController.Move((slideVelocity + moveVelocity * slopeControlRatio) * Time.deltaTime); // 슬라이딩 방향으로 슬라이드. slopeControlRatio에 따라 경사면에서 조작 가능한 정도 조절
    }

    /// <summary>
    /// 걷기 시작
    /// </summary>
    public void StartWalk()
    {
        moveSpeed = walkSpeed;
    }

    /// <summary>
    /// 지형 상태에 따라 다르게 움직임
    /// </summary>
    public void Move()
    {
        if (PhysicsUtil.IsOnSlope(gameObject, ref slopeHit)) // 만약 플레이어가 이동 가능한 경사면 위에 있다면
        {
            MoveOnSlope();
        }
        else // 평지 또는 공중에 있으면
        {
            MoveOnPlain();
        }

        if (PhysicsUtil.IsGrounded(this.gameObject)) currentYSpeed = 0.0f; // 물체 위에 있는 경우에는 중력 영향 X
    }

    /// <summary>
    /// 플레이어가 이동 가능한 경사면에서 움직이게 함
    /// </summary>
    private void MoveOnSlope()
    {
        characterController.Move((moveVelocity + (Vector3.down * slopeDownForce)) * Time.deltaTime); // 경사면 내려갈 때 떨림 현상 방지하기 위해 아래쪽으로 힘 적용
    }

    /// <summary>
    /// 플레이어가 경사가 없는 지형에서 움직이게 함
    /// </summary>
    private void MoveOnPlain()
    {
        characterController.Move(moveVelocityWithGravity * Time.deltaTime); // 정해진 속도로 캐릭터 이동
    }

    public bool CheckJump()
    {
        if (!PhysicsUtil.IsGrounded(this.gameObject)) return false; // 공중에 떠있는 경우에는 점프 X
        // if (PhysicsUtil.IsOnSlope(this.gameObject, ref slopeHit)) return false; // 경사면 위에 있는 경우 점프 X
        
        if (!isJumping && keyPressed_Jump) // 점프
        {
            if (isCrouching && !enableDuckJump) // enableDuckJump가 false인 경우 앉을 상태에서 점프 X
            {
                keyPressed_Jump = false;
                return false; 
            }
            if (!playerStamina.hasEnoughStamina_Jump)
            {
                keyPressed_Jump = false;
                return false;
            }

            StartJump();
            return true;
        }
        else if (isJumping && PhysicsUtil.IsGrounded(this.gameObject)) // 착지
        {
            EndJump();
            return true;
        }

        return false;
    }

    private void StartJump()
    {       
        slopeDownForce = 0;
        isJumping = true;
        currentYSpeed = jumpSpeed; // y 속력 변경해 점프
        StartJumpAction(); // 점프 액션 실행
    }

    private void EndJump()
    {
        keyPressed_Jump = false;
        isJumping = false;
        slopeDownForce = slopeForceTmp;
    }

    /// <summary>
    /// 달리기가 가능한지 여부를 반환
    /// </summary>
    /// <returns>달리기 가능 여부</returns>
    public bool CheckSprint()
    {
        if (!playerStamina.hasEnoughStamina_Sprint) return false;

        Vector2 moveInput = playerKeyInput.moveInput;

        if (Input.GetButton("Sprint")) // 앞으로 이동하는 경우에만 달릴 수 있음
        {
            if(moveInput.y > 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 달리기 시작
    /// </summary>
    public void StartSprint()
    {
        moveSpeed = sprintSpeed;
    }

    /// <summary>
    /// 달리기 종료
    /// </summary>
    public void EndSprint()
    {
        moveSpeed = walkSpeed;
    }

    /// <summary>
    /// 앉기가 가능한지 여부를 반환
    /// </summary>
    /// <returns>앉기 가능 여부</returns>
    public bool CheckCrouch()
    {
        if (playerKeyInput.keyPressed_Crouch)
        {
            return true;
        }

        return false;
    }

    public void StartCrouch()
    {
        moveSpeed = crouchSpeed;
        isCrouching = true;

        characterController.height = Mathf.SmoothDamp(currentHeight, crouchHeight, ref currentHeightRef, crouchSmoothTime); 
        characterController.center = Vector3.SmoothDamp(characterController.center, crouchCenter, ref crouchCenterRef, crouchSmoothTime);
    }

    public void EndCrouch()
    {
        // 일어섰을 때의 높이만큼 충분한 공간이 없으면 일어나지 않음
        // 캐릭터의 상하좌우에서 위쪽으로 레이 발사해서 검사
        float dist = standHeight - crouchHeight + characterController.height - transform.position.y;
        bool cond1 = PhysicsUtil.CheckUpperSpace(transform.position + transform.forward * characterController.radius, dist); // 앞쪽
        bool cond2 = PhysicsUtil.CheckUpperSpace(transform.position - transform.forward * characterController.radius, dist); // 뒤쪽
        bool cond3 = PhysicsUtil.CheckUpperSpace(transform.position + transform.right * characterController.radius, dist); // 오른쪽
        bool cond4 = PhysicsUtil.CheckUpperSpace(transform.position - transform.right * characterController.radius, dist); // 왼쪽
        if (cond1 || cond2 || cond3 || cond4) return;

        // ------- 이 위에 있는 코드는 일어나지 못하는 상황에 대한 코드 -------

        if (standHeight - currentHeight > 0.01f)
        {
            characterController.height = Mathf.SmoothDamp(currentHeight, standHeight, ref currentHeightRef, standSmoothTime);
            characterController.center = Vector3.SmoothDamp(characterController.center, standCenter, ref crouchCenterRef, crouchSmoothTime);
        }
        else
        {
            isCrouching = false;
        }

    }
}