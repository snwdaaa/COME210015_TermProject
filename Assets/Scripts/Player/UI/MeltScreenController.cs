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
    }

    public void ShowScreen()
    {
        image.gameObject.SetActive(true);
    }

    public void StartScreenMelt()
    {
        screenMelt.effectOn = true;
    }

    public void HideMeltImage()
    {
        image.gameObject.SetActive(false);
    }
}
