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

    public Vector2 mouseInput { get; private set; }

    private void Update()
    {
        if (!PauseMenu.isMenuOpened) // 메뉴에서 시점 움직이는 것 방지
        {
            DetectMouseInput_MouseInput();
        }
        else
        {
            // 시야 돌리는 순간에 일시 정지 메뉴 활성화 할 때
            // 마지막 속도 그대로 계속 움직이는 문제 해결
            if (mouseInput != Vector2.zero)
            {
                mouseInput = Vector2.zero;
            }
        }
    }

    private void DetectMouseInput_MouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        mouseInput = new Vector2(mouseX, mouseY);
    }
}
