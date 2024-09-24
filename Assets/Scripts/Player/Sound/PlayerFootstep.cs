using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    // 컴포넌트
    private AudioSource audioSource;
    private PlayerStateMachine playerStateMachine;
    private PlayerMovement playerMovement;

    [Header("사운드")]
    private AudioClip currentFootstepSound; // 재질에 따라 정해진 사운드
    public AudioClip[] jumpSound;
    public AudioClip[] landingSound;
    public AudioClip[] defaultSound;
    public AudioClip[] dirtSound;
    public AudioClip[] woodSound;
    public AudioClip[] waterSound;

    [Header("설정")]
    public float walkStepInterval;
    public float sprintStepInterval;
    public float crouchWalkStepInterval;
    public bool onAirStarted = false; // 점프, 착지 사운드 재생 판별 용도
    private enum JumpType { None, KeyPressed, Fall }; 
    private JumpType jumpType = JumpType.None;
    

    private LayerMask surfaceType;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayFootstepSound();
    }

    /// <summary>
    /// 바닥 재질 확인 후 surfaceType 변수 업데이트
    /// </summary>
    private void CheckSurfaceType()
    {
        // 아래 방향으로 Ray 발사
        if (Physics.Raycast(transform.position, Vector3.down, 1f, surfaceType))
        {
            Debug.Log(surfaceType);
        }
    }

    /// <summary>
    /// 플레이어가 착지한 경우, 점프 후 착지인지 점프 없이 떨어진 후 착지인지 결정
    /// </summary>
    private void PlayJumpSound()
    {
        if (!onAirStarted)
        {
            if (playerMovement.isJumping)
            {
                Debug.Log("Play Jump sound");
                //audioSource.PlayOneShot(jumpSound[Random.Range(0, jumpSound.Length)]);
                jumpType = JumpType.KeyPressed;
                onAirStarted = true;
            }
            else if (playerStateMachine.CurrentMoveState == playerStateMachine.onAirState)
            {
                Debug.Log("Fall");
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
            Debug.Log("Play Land sound");
            onAirStarted = false;
            //audioSource.PlayOneShot(landingSound[Random.Range(0, jumpSound.Length)]);
            jumpType = JumpType.None;

        }

        // 이동 시 발소리 재생
        if (cond1 || cond2 || cond3)
        {
            // surfaceType을 확인해 결정
            CheckSurfaceType();

            // 그 재질에 맞는 랜덤 사운드 재생

        }


    }

    IEnumerator PlayFootStep(float interval)
    {
        audioSource.PlayOneShot(currentFootstepSound);
        yield return new WaitForSeconds(interval);
    }
}
