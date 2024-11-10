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

    private void Start()
    {
        screenMelt = image.GetComponent<ScreenMelt>();

        ResetScreen();
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

    public void StartScreenFadeIn()
    {
        StartCoroutine("FadeIn");
    }

    public void StartScreenMelt()
    {
        StartCoroutine("ScreenMelt");
    }

    IEnumerator ScreenMelt()
    {
        screenMelt.effectOn = true;
        yield return new WaitForSeconds(2.0f);
        image.gameObject.SetActive(false);
    }

    /// <summary>
    /// 화면 Fade In 시작
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeIn()
    {
        while (image.material.color.a < 1f)
        {
            Color currentColor = image.material.color;
            currentColor.a += Mathf.Min(currentColor.a + fadeInDelta, 1f);
            image.material.color = currentColor;
            yield return new WaitForSeconds(fadeInWaitTime);
        }
    }

    /// <summary>
    /// 플레이 종료할 때마다 쉐이더 Color 리셋
    /// </summary>
    void OnApplicationQuit()
    {
        ResetScreen();
    }
}
