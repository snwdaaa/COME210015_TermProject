using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeltScreenController : MonoBehaviour
{
    private ScreenMelt screenMelt;
    [SerializeField] private Image image;
    [SerializeField] private float fadeInDelta = 0.01f; // 화면 암전 단위
    [SerializeField] private float fadeInWaitTime = 0.01f; // 화면 암전 속도

    private void Awake()
    {
        ResetScreen();
    }

    private void Start()
    {
        screenMelt = image.GetComponent<ScreenMelt>();
    }

    /// <summary>
    /// 화면 초기화
    /// </summary>
    private void ResetScreen()
    {
        Color currentColor = image.material.color;
        currentColor.a = 0f;
        image.material.color = currentColor;
    }

    public void StartScreenMelt()
    {
        image.gameObject.SetActive(true);
        screenMelt.effectOn = true;
    }

    public void HideMeltImage()
    {
        image.gameObject.SetActive(false);
    }

    /// <summary>
    /// 플레이 종료할 때마다 쉐이더 Color 리셋
    /// </summary>
    void OnApplicationQuit()
    {
        ResetScreen();
    }
}
