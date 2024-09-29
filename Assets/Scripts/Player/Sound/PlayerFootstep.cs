using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �÷��̾� �߼Ҹ� ���� ��ũ��Ʈ
/// </summary>
public class PlayerFootstep : MonoBehaviour
{
    // ������Ʈ
    private AudioSource audioSource;
    private PlayerStateMachine playerStateMachine;
    private PlayerMovement playerMovement;
    private PlayerMovementHelper playerMovementHelper;

    [Header("����")]
    [SerializeField] private AudioClip[] jumpSound;
    [SerializeField] private AudioClip[] landingSound;
    [SerializeField] private AudioClip[] defaultSound;
    [SerializeField] private AudioClip[] dirtSound;
    [SerializeField] private AudioClip[] woodSound;
    [SerializeField] private AudioClip[] waterSound;
    private AudioClip currentFootstepSound; // ������ ���� ������ ����

    [Header("����")]
    [SerializeField] private float walkVolume = 0.2f;
    [SerializeField] private float crouchWalkVolume = 0.1f;
    [SerializeField] private float sprintVolume = 0.4f;
    [SerializeField] private float walkStepInterval;
    [SerializeField] private float sprintStepInterval;
    [SerializeField] private float crouchWalkStepInterval;

    private int surfaceType;
    private float stepIntervalTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = walkVolume;
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
        playerMovementHelper = GetComponent<PlayerMovementHelper>();

        SubscribeEvent();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayFootstepSound();
    }

    /// <summary>
    /// �ʿ��� �̺�Ʈ ����
    /// </summary>
    private void SubscribeEvent()
    {
        // Jump Event
        playerMovementHelper.PlayerJumpStartEvent += () =>
        {
            audioSource.PlayOneShot(jumpSound[Random.Range(0, jumpSound.Length)]);
        };

        // Landing Event
        playerMovementHelper.PlayerLandingEvent += () =>
        {
            if (playerMovementHelper.fallHeight > 0.1f) // ���̰� ��� ���� �ִ� ��쿡�� ���� ���
            {
                audioSource.PlayOneShot(landingSound[Random.Range(0, jumpSound.Length)]);
            }         
        };
    }

    /// <summary>
    /// �ٴ� ���� Ȯ�� �� currentFootstepSound�� �ش� ���� ���� �Ҵ�
    /// </summary>
    private void CheckSurfaceType()
    {
        // �Ʒ� �������� Ray �߻�
        RaycastHit hit;
        Vector3 rayStartPos = new Vector3(transform.position.x, 0.1f, transform.position.z);
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
        if (playerMovementHelper.onAirStarted)
        {
            if (playerMovementHelper.onAirType_Jump)
            {
                audioSource.PlayOneShot(jumpSound[Random.Range(0, jumpSound.Length)]);
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
