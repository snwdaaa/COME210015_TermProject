using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 마우스 커서 관리 스크립트
/// </summary>
public class CursorManager : MonoBehaviour
{
    private void Start()
    {
        HideCursor();
    }

    public static void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
