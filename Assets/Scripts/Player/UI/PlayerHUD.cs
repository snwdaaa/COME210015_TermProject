using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Text staminaText;

    [Header("컴포넌트")]
    private PlayerHealth plyHealth;
    private PlayerStamina plyStamina;

    // Start is called before the first frame update
    void Start()
    {
        plyHealth = GetComponent<PlayerHealth>();
        plyStamina = GetComponent<PlayerStamina>();
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
        // 체력/스태미너 바 Fill Amount 설정
        healthBar.fillAmount = plyHealth.currentHealth / plyHealth.currentMaxHealth;
        staminaBar.fillAmount = plyStamina.currentStamina / plyStamina.currentMaxStamina;
    }
}
