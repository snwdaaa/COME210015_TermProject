using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeltScreenController : MonoBehaviour
{
    private ScreenMelt screenMelt;
    [SerializeField] private Image image;

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
