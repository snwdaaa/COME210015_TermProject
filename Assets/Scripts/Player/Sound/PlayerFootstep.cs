using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    // ������Ʈ
    private AudioSource audioSource;
    private PlayerStateMachine playerStateMachine;
    private PlayerMovement playerMovement;

    [Header("����")]
    private AudioClip currentFootstepSound; // ������ ���� ������ ����
    public AudioClip[] jumpSound;
    public AudioClip[] landingSound;
    public AudioClip[] defaultSound;
    public AudioClip[] dirtSound;
    public AudioClip[] woodSound;
    public AudioClip[] waterSound;

    [Header("����")]
    public float walkStepInterval;
    public float sprintStepInterval;
    public float crouchWalkStepInterval;
    public bool onAirStarted = false; // ����, ���� ���� ��� �Ǻ� �뵵
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
    /// �ٴ� ���� Ȯ�� �� surfaceType ���� ������Ʈ
    /// </summary>
    private void CheckSurfaceType()
    {
        // �Ʒ� �������� Ray �߻�
        if (Physics.Raycast(transform.position, Vector3.down, 1f, surfaceType))
        {
            Debug.Log(surfaceType);
        }
    }

    /// <summary>
    /// �÷��̾ ������ ���, ���� �� �������� ���� ���� ������ �� �������� ����
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
    /// surfaceType�� ���� �ٸ� ���� ���
    /// </summary>
    private void PlayFootstepSound()
    {
        bool cond1 = playerStateMachine.CurrentMoveState == playerStateMachine.walkState;
        bool cond2 = playerStateMachine.CurrentMoveState == playerStateMachine.sprintState;
        bool cond3 = playerStateMachine.CurrentMoveState == playerStateMachine.crouchWalkState;
        bool cond4 = playerStateMachine.CurrentMoveState == playerStateMachine.idleState;

        // ����, ���� ���� ���
        PlayJumpSound();

        if (onAirStarted && (cond1 || cond2 || cond3 || cond4))
        {
            Debug.Log("Play Land sound");
            onAirStarted = false;
            //audioSource.PlayOneShot(landingSound[Random.Range(0, jumpSound.Length)]);
            jumpType = JumpType.None;

        }

        // �̵� �� �߼Ҹ� ���
        if (cond1 || cond2 || cond3)
        {
            // surfaceType�� Ȯ���� ����
            CheckSurfaceType();

            // �� ������ �´� ���� ���� ���

        }


    }

    IEnumerator PlayFootStep(float interval)
    {
        audioSource.PlayOneShot(currentFootstepSound);
        yield return new WaitForSeconds(interval);
    }
}
