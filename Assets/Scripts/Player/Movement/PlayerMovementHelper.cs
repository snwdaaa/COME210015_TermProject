using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �÷��̾� �̵� ���� �ΰ� ��� ó��
/// </summary>
public class PlayerMovementHelper : MonoBehaviour
{
    // ������Ʈ
    private PlayerMovement playerMovement;
    private PlayerStateMachine playerStateMachine;

    // ----------------- ����, �߶� ó�� -----------------
    public bool onAirStarted { get; private set; }
    public bool onAirType_Jump { get; private set; }
    public bool onAirType_Fall { get; private set; }
    public event Action PlayerJumpStartEvent;
    public event Action PlayerFallStartEvent;
    public event Action PlayerLandingEvent;

    public float fallHeight { get; private set; }
    private float playerMaxHeight; // �÷��̾��� �ִ� ��ġ ����
    private float playerLandHeight; // �÷��̾��� ���� ��ġ ����
    // ----------------- ����, �߶� ó�� -----------------

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStateMachine = GetComponent<PlayerStateMachine>();

        // �ʱ�ȭ
        onAirStarted = false;
        onAirType_Fall = false;
        onAirType_Jump = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckOnAirState();
    }

    // ----------------- ����, �߶� ó�� -----------------
    /// <summary>
    /// ���� �÷��̾ ���߿� ���ִ� ���°� ������ ���� �������� ���� ���� ������ �������� üũ
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
    /// �÷��̾ ���� or �߶��� ������ �� ȣ��
    /// </summary>
    private void StartOnAirState()
    {
        if (playerMovement.isJumping)
        {
            onAirStarted = true;
            onAirType_Jump = true;
            PlayerJumpStartEvent?.Invoke();

            // ���� ���
            CalcFallStartHeight();
        }
        else if (playerStateMachine.CurrentMoveState == playerStateMachine.onAirState)
        {
            onAirStarted = true;
            onAirType_Fall = true;
            PlayerFallStartEvent?.Invoke();

            // ���� ���
            CalcFallStartHeight();
        }
    }

    /// <summary>
    /// �÷��̾ ������ �� ȣ��
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
            PlayerLandingEvent?.Invoke(); // �̺�Ʈ ����
            if (onAirType_Jump) onAirType_Jump = false;
            else if (onAirType_Fall) onAirType_Fall = false;

            // ���� ���
            CalcFallEndHeight();
        }
    }

    /// <summary>
    /// �÷��̾ �߶��� �����ϴ� �ִ� ���̸� ��� �� ����
    /// </summary>
    private void CalcFallStartHeight()
    {
        float startYPos = this.gameObject.transform.position.y;

        if (onAirType_Jump)
        {
            Vector3 playerMoveDir = playerMovement.moveVelocityWithGravity; // �̵� ���� ����
            Vector3 playerMoveDirAxis = Vector3.zero; // �̵� ������ ��ǥ�鿡 ������ ���Ϳ� ���� ������ ������ ���� (���� ��Ʈ ����)

            // �̵� ���� ���ͷ� ��Ʈ ������ ������ ���
            playerMoveDirAxis += transform.InverseTransformDirection(this.transform.forward) * playerMoveDir.z;
            playerMoveDirAxis += transform.InverseTransformDirection(this.transform.right) * playerMoveDir.x;

            // �߷��� ������ �޴� �ִ� ���� ���� (������ � �ְ��� ���� ����)
            // = (���� ���۽� �̵� �ӵ�)^2 * sin^2(�̵� ���� ��Ʈ ���Ϳ� ���� �̵� ���� ���� ��) / 2 * gravity
            float angle = Vector3.Angle(playerMoveDir.normalized, playerMoveDirAxis.normalized);
            float sinVal = Mathf.Sin(angle * Mathf.Deg2Rad); // �������� ��ȯ
            float jumpMaxHeight = (Mathf.Pow(playerMoveDir.magnitude, 2) * Mathf.Pow(sinVal, 2)) / (2 * Mathf.Abs(Physics.gravity.y)); // �߷°� �׻� ������� ��
            playerMaxHeight = startYPos + jumpMaxHeight; 
        }
        else if (onAirType_Fall)
        {
            playerMaxHeight = startYPos;
        }
    }

    /// <summary>
    /// �÷��̾ ���鿡 ������ ��ġ�� ���̿� ���� ���� ���̸� ��� �� ����
    /// </summary>
    private void CalcFallEndHeight()
    {
        playerLandHeight = this.gameObject.transform.position.y;
        fallHeight = playerMaxHeight - playerLandHeight;
    }
    // ----------------- ����, �߶� ó�� -----------------
}