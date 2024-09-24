using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    // ������Ʈ
    private AudioSource audioSource;
    private PlayerStateMachine playerStateMachine;
    private PlayerMovement playerMovement;

    [Header("����")]
    public AudioClip[] jumpSound;
    public AudioClip[] landingSound;
    public AudioClip[] defaultSound;
    public AudioClip[] dirtSound;
    public AudioClip[] woodSound;
    public AudioClip[] waterSound;
    private AudioClip currentFootstepSound; // ������ ���� ������ ����

    [Header("����")]
    public float walkVolume = 0.2f;
    public float crouchWalkVolume = 0.1f;
    public float sprintVolume = 0.4f;
    public float walkStepInterval;
    public float sprintStepInterval;
    public float crouchWalkStepInterval;
    private bool onAirStarted = false; // ����, ���� ���� ��� �Ǻ� �뵵
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
    /// �ٴ� ���� Ȯ�� �� currentFootstepSound�� �ش� ���� ���� �Ҵ�
    /// </summary>
    private void CheckSurfaceType()
    {
        // �Ʒ� �������� Ray �߻�
        RaycastHit hit;
        Vector3 rayStartPos = new Vector3(transform.position.x, 0.1f, transform.position.z);
        Debug.DrawRay(rayStartPos, Vector3.down * 0.5f, Color.red);
        if (Physics.Raycast(rayStartPos, Vector3.down, out hit, 0.5f))
        {
            surfaceType = hit.transform.gameObject.layer;
        }

        // surfaceType�� ���� ���� ����
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
    /// �÷��̾ ������ ���, ���� �� �������� ���� ���� ������ �� �������� ���� �� ���� ���
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
            onAirStarted = false;
            audioSource.PlayOneShot(landingSound[Random.Range(0, jumpSound.Length)]);
            jumpType = JumpType.None;
        }

        // �̵� �� �߼Ҹ� ���
        if (cond1 || cond2 || cond3)
        {
            float interval = 0f;

            // ���� �� �� ����
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

            // surfaceType�� Ȯ���� ������ �´� ���� ���� ����
            CheckSurfaceType();

            // �� ������ �´� ���� ���� ���
            stepIntervalTimer += Time.deltaTime; // �߼Ҹ� ��� ������ ���� Ÿ�̸�
            if (stepIntervalTimer > interval)
            {
                stepIntervalTimer = 0; // Ÿ�̸� �ʱ�ȭ
                audioSource.PlayOneShot(currentFootstepSound);
            }
        }
    }
}
