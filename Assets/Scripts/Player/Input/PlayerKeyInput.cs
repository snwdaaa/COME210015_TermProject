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
    public bool keyPressed_Crouch { get; private set; }
    public bool keyPressed_Fire { get; private set; }

    public bool keyPressed_Use { get; set; }

    private void Update()
    {
        DetectKeyInput_MoveInput();
        DetectKeyInput_Crouch();
        DetectKeyInput_Use();
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
