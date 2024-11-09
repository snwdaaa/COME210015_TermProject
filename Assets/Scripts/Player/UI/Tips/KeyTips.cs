using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// F1 키를 누르면 키 도움말 UI 표시
/// </summary>
public class KeyTips : MonoBehaviour
{
    [SerializeField] private GameObject keyTipsUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("ToggleKeyTips"))
        {
            keyTipsUI.SetActive(!keyTipsUI.activeSelf); // Toggle
        }
    }
}
