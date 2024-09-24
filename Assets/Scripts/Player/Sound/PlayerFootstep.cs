using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    // 컴포넌트
    private AudioSource audioSource;
    private PlayerStateMachine playerStateMachine;
    private PlayerMovement playerMovement;

    [Header("사운드")]
    public AudioClip[] jumpSound;
    public AudioClip[] landingSound;
    public AudioClip[] defaultSound;
    public AudioClip[] dirtSound;
    public AudioClip[] woodSound;
    public AudioClip[] waterSound;
    private AudioClip currentFootstepSound; // 재질에 따라 정해진 사운드

    [Header("설정")]
    public float walkVolume = 0.2f;
    public float crouchWalkVolume = 0.1f;
    public float sprintVolume = 0.4f;
    public float walkStepInterval;
    public float sprintStepInterval;
    public float crouchWalkStepInterval;
    private bool onAirStarted = false; // 점프, 착지 사운드 재생 판별 용도
    private enum JumpType { None, KeyPressed, Fall }; 
    private JumpType jumpType = JumpType.None;

    private int surfaceType;
    private float stepIntervalTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = walkVolume;
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayFootstepSound();
    }

    /// <summary>
    /// 바닥 재질 확인 후 currentFootstepSound에 해당 재질 사운드 할당
    /// </summary>
    private void CheckSurfaceType()
    {
        // 아래 방향으로 Ray 발사
        RaycastHit hit;
        Vector3 rayStartPos = new Vector3(transform.position.x, 0.1f, transform.position.z);
        Debug.DrawRay(rayStartPos, Vector3.down * 0.5f, Color.red);
        if (Physics.Raycast(rayStartPos, Vector3.down, out hit, 0.5f))
        {
            surfaceType = hit.transform.gameObject.layer;
        }

        // surfaceType에 따라 사운드 결정
        if (surfaceType == LayerMask.NameToLayer("SURFACE_DIRT"))
        {
            currentFootstepSound = dirtSound[Random.Range(0, dirtSound.Length)];
        }
        else if (surfaceType == LayerMask.NameToLayer("SURFACE_WOOD"))
        {
            currentFootstepSound = woodSound[Random.Range(0, woodSound.Length)];
        }
        else if (surfaceType == LayerMask.NameToLayer("SURFACE_WATER"))
        {
            currentFootstepSound = waterSound[Random.Range(0, waterSound.Length)];
        }
        else
        {
            currentFootstepSound = defaultSound[Random.Range(0, defaultSound.Length)];
        }
    }

    /// <summary>
    /// 플레이어가 착지한 경우, 점프 후 착지인지 점프 없이 떨어진 후 착지인지 결정 후 사운드 재생
    /// </summary>
    private void PlayJumpSound()
    {
        if (!onAirStarted)
        {
            if (playerMovement.isJumping)
            {
                audioSource.PlayOneShot(jumpSound[Random.Range(0, jumpSound.Length)]);
                jumpType = JumpType.KeyPressed;
                onAirStarted = true;
            }
            else if (playerStateMachine.CurrentMoveState == playerStateMachine.onAirState)
            {
                jumpType = JumpType.Fall;
                onAirStarted = true;
            }
        }
    }

    /// <summary>
    /// surfaceType에 따라 다른 사운드 재생
    /// </summary>
    private void PlayFootstepSound()
    {
        bool cond1 = playerStateMachine.CurrentMoveState == playerStateMachine.walkState;
        bool cond2 = playerStateMachine.CurrentMoveState == playerStateMachine.sprintState;
        bool cond3 = playerStateMachine.CurrentMoveState == playerStateMachine.crouchWalkState;
        bool cond4 = playerStateMachine.CurrentMoveState == playerStateMachine.idleState;

        // 점프, 착지 사운드 재생
        PlayJumpSound();

        if (onAirStarted && (cond1 || cond2 || cond3 || cond4))
        {
            onAirStarted = false;
            audioSource.PlayOneShot(landingSound[Random.Range(0, jumpSound.Length)]);
            jumpType = JumpType.None;
        }

        // 이동 시 발소리 재생
        if (cond1 || cond2 || cond3)
        {
            float interval = 0f;

            // 상태 별 값 설정
            if (cond1)
            {
                audioSource.volume = walkVolume;
                interval = walkStepInterval;
            }
            else if (cond2)
            {
                audioSource.volume = sprintVolume;
                interval = sprintStepInterval;
            }
            else if (cond3)
            {
                audioSource.volume = crouchWalkVolume;
                interval = crouchWalkStepInterval;
            }

            // surfaceType을 확인해 재질에 맞는 랜덤 사운드 결정
            CheckSurfaceType();

            // 그 재질에 맞는 랜덤 사운드 재생
            stepIntervalTimer += Time.deltaTime; // 발소리 재생 간격을 위한 타이머
            if (stepIntervalTimer > interval)
            {
                stepIntervalTimer = 0; // 타이머 초기화
                audioSource.PlayOneShot(currentFootstepSound);
            }
        }
    }
}
