using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Input System 사용

/// <summary>
/// 플레이어 키 입력 감지 스크립트
/// </summary>
public class PlayerKeyInput : MonoBehaviour
{
    // 컴포넌트
    private PlayerInput playerInput;

    // 플레이어 입력 여부 프로퍼티
    // 키보드
    public Vector2 moveInput { get; private set; }
    public bool keyPressed_Sprint { get; private set; }
    public bool keyPressed_Crouch { get; private set; }
    public bool keyPressed_Fire { get; private set; }

    // FixedUpdate에서 GetButtonDown을 검사하는 경우 키가 무시되는 경우가 있음
    // 이 스크립트에서 키가 감지되면 true로 바꾸고, 다른 스크립트에서 처리 후 false로 바꿔주는 방식을 사용
    public bool keyPressed_Jump { get; set; }
    public bool keyPressed_Use { get; set; }

    private void Update()
    {
        DetectKeyInput_MoveInput();
        DetectKeyInput_Sprint();
        DetectKeyInput_Crouch();
        DetectKeyInput_Use();
        DetectKeyInput_Jump();
    }

    /// <summary>
    /// 업데이트 함수들의 호출 순서 차이로 인해 키 입력이 무시되는 문제 해결하기 위해 딜레이를 둠
    /// </summary>
    /// <returns></returns>

    private void DetectKeyInput_MoveInput()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        moveInput = new Vector2(horizontalInput, verticalInput).normalized;
    }

    private void DetectKeyInput_Sprint()
    {
        keyPressed_Sprint = Input.GetButton("Sprint");
    }

    private void DetectKeyInput_Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            keyPressed_Jump = true;
        }
    }

    private void DetectKeyInput_Crouch()
    {
        keyPressed_Crouch = Input.GetButton("Crouch");
    }

    private void DetectKeyInput_Use()
    {
        if (Input.GetButtonDown("Use"))
        {
            keyPressed_Use = true;
        }
    }
}
