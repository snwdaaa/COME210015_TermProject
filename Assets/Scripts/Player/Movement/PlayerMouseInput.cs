using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Input System 사용

/// <summary>
/// 플레이어 마우스 입력 감지 스크립트
/// </summary>
public class PlayerMouseInput : MonoBehaviour
{
    // 컴포넌트
    private PlayerInput playerInput;

    // 카메라 관련
    public Vector2 mouseInput { get; private set; }

    public void OnLookCallback(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }
}
