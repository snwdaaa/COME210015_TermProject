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
    private CharacterController characterController;
    private PlayerKeyInput playerKeyInput;

    [Header("이동 속도 설정")]
    public float moveSpeed;
    public float walkSpeed = 3f;
    public float moveSpeedSmoothTime = 0.2f;
    public float sprintSpeed = 6f;
    private float moveSpeedRef; // 변화량 저장 레퍼런스

    [Header("점프 속도 설정")]
    public float jumpSpeed = 10f;
    [Range(0.01f, 1f)] public float airControlPercentage = 1f; // 점프 중 조작 가능한 정도
    private float currentYSpeed; // 현재 y 방향 속도

    // 현재 이동 속력 -> 캐릭터 컨트롤러의 속도 벡터의 크기
    public float currentSpeed => new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerKeyInput = GetComponent<PlayerKeyInput>();
    }

    private void FixedUpdate()
    {
        Move(playerKeyInput.moveInput);

        // 점프
        if (playerKeyInput.keyPressed_Jump)
            Jump();

        Sprint(playerKeyInput.moveInput); // 이동 속도

    }

    /// <summary>
    /// 입력받은 방향으로 플레이어 이동.
    /// 바로 목표 속도로 바뀌지 않고 가속도가 붙게 함
    /// </summary>
    /// <param name="moveInput">키 입력 벡터</param>
    private void Move(Vector2 moveInput)
    {
        // 이동 속력
        float targetSpeed = moveSpeed * moveInput.magnitude; // 목표 속력 = 최대 속력 * 입력 벡터 크기
        // 가속도 smoothTime 설정. 떠있는 동안에는 airControlPercentage에 의해 조작 속도 조절
        float smoothTime = characterController.isGrounded ? moveSpeedSmoothTime : moveSpeedSmoothTime / airControlPercentage;
        targetSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref moveSpeedRef, smoothTime); // 이동 속력 점진적으로 증가
        currentYSpeed += Physics.gravity.y * Time.deltaTime; // 아래에 물체가 없는 경우에 y 속력이 중력에 영향을 받게 함

        // 방향
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x; // 이동 방향

        // 최종 속도
        Vector3 moveVelocity = moveDirection * targetSpeed + transform.up * currentYSpeed; // 중력 영향 받게 함

        characterController.Move(moveVelocity * Time.deltaTime); // 정해진 속도로 캐릭터 이동

        if (characterController.isGrounded)
            currentYSpeed = 0; // 물체 위에 있는 경우에는 중력 영향 X
    }

    private void Jump()
    {
        if (!characterController.isGrounded) return; // 공중에 떠있는 경우에는 점프 X

        currentYSpeed = jumpSpeed; // y 속력 변경해 점프
    }

    private void Sprint(Vector2 moveInput)
    {
        if (playerKeyInput.keyPressed_Sprint) // 앞으로 이동하는 경우에만 달릴 수 있음
        {
            if (moveInput.y > 0)
                moveSpeed = sprintSpeed;
        }
        else
            moveSpeed = walkSpeed;
    }
}