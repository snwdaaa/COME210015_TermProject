using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healthBar;
    private PlayerHealth plyHealth;

    [Header("Stamina")]
    [SerializeField] private Image staminaBar;
    private PlayerStamina plyStamina;

    [Header("Flashlight")]
    [SerializeField] private PlayerFlashlight plyFlashlight;
    [SerializeField] private Image batteryBar;

    private void Awake()
    {
        plyHealth = GetComponent<PlayerHealth>();
        plyStamina = GetComponent<PlayerStamina>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnGUI()
    {
        UpdateHUD();
    }

    /// <summary>
    /// HUD 관련 정보 업데이트
    /// </summary>
    private void UpdateHUD()
    {
        healthBar.fillAmount = plyHealth.currentHealth / plyHealth.currentMaxHealth; // 체력
        staminaBar.fillAmount = plyStamina.currentStamina / plyStamina.currentMaxStamina; // 스태미너
        batteryBar.fillAmount = plyFlashlight.currentBatteryAmount / plyFlashlight.currentMaxBatteryAmount;
    }
}
