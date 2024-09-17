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
    public Vector2 moveInput { get; private set; }
    public bool keyPressed_Sprint { get; private set; }
    public bool keyPressed_Crouch { get; private set; }
    public bool keyPressed_Fire { get; private set; }
    public bool keyPressed_Jump { get; private set; }

    // Input System에 할당된 키 입력시 실행될 콜백함수
    public void OnMoveCallback(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>(); // context -> 레퍼런스

        if (input != null)
            moveInput = input;
    }

    // 콜백 호출되면 이벤트 실행
    public void OnSprintCallback(InputAction.CallbackContext context)
    {
        keyPressed_Sprint = context.ReadValueAsButton();
    }

    public void OnCrouchCallback(InputAction.CallbackContext context)
    {
        keyPressed_Crouch = context.ReadValueAsButton();
    }

    public void OnFireCallback(InputAction.CallbackContext context)
    {
        keyPressed_Fire = context.ReadValueAsButton();
    }

    public void OnJumpCallback(InputAction.CallbackContext context)
    {
        keyPressed_Jump = context.ReadValueAsButton();
    }
}
