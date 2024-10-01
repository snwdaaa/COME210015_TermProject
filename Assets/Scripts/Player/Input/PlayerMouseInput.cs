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
    public bool mouseButtonPressed_LeftButton { get; private set; }
    public bool mouseButtonPressed_RightButton { get; private set; }

    private void Update()
    {
        DetectMouseInput_MouseInput();
        DetectMouseInput_LeftButton();
        DetectMouseInput_RightButton();
    }

    private void DetectMouseInput_MouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        mouseInput = new Vector2(mouseX, mouseY);
    }

    private void DetectMouseInput_LeftButton()
    {
        mouseButtonPressed_LeftButton = Input.GetButton("LeftMouseButton");
    }

    private void DetectMouseInput_RightButton()
    {
        mouseButtonPressed_RightButton = Input.GetButton("RightMouseButton");
    }
}
