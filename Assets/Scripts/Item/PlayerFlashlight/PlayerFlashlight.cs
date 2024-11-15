using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [Header("Components")]
    private new Light light;
    private AudioSource audioSource;
    private PlayerHealth plyHealth;

    [Header("Sounds")]
    [SerializeField] private AudioClip toggleSound;

    [Header("Flashlight")]
    [SerializeField] private float batteryDrainPerSec = 3.0f;
    [SerializeField] private float batteryRecoverPerSec = 2.0f;
    private float maxBatteryAmount = 100.0f; // 최대 배터리 양
    public bool isLightOn { get; private set; } // 라이트 켜짐, 꺼짐 여부
    [SerializeField] float lightIntensity = 1.0f;

    [SerializeField] private float toggleDelay = 1.0f; // 토글 딜레이 (연타 방지)
    private float enabledTime = 0.0f; // 켜진 후 지난 시간

    // Properties
    public float currentMaxBatteryAmount { get; private set; }
    public float currentBatteryAmount { get; private set; } // 현재 배터리 양


    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
        plyHealth = GetComponentInParent<PlayerHealth>();

        currentMaxBatteryAmount = maxBatteryAmount;
        currentBatteryAmount = maxBatteryAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (plyHealth.isDied) return;

        if (IsToggleable() && Input.GetButtonDown("Flashlight"))
        {
            ToggleFlashlight();
        }

        CalcBattery();
    }

    private void ToggleFlashlight()
    {
        if (isLightOn) // 켜져있으면
        {
            light.intensity = 0f;
            isLightOn = false;
        }
        else
        {
            light.intensity = lightIntensity;
            isLightOn = true;
        }

        audioSource.PlayOneShot(toggleSound);
        enabledTime = 0.0f; // 타이머 초기화
    }

    private bool IsToggleable()
    {
        if (enabledTime <= toggleDelay)
        {
            enabledTime += Time.deltaTime;
            return false;
        }
        else
        {
            return true;
        }
    }

    private void CalcBattery()
    {
        if (isLightOn)
        {
            if (currentBatteryAmount <= 0)
            {
                ToggleFlashlight();
                return;
            }

            currentBatteryAmount -= batteryDrainPerSec * Time.deltaTime;
        }
        else
        {
            if (currentBatteryAmount < maxBatteryAmount)
            {
                currentBatteryAmount += batteryRecoverPerSec * Time.deltaTime;
            }            
        }
    }
}
