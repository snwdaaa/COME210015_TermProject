using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    [SerializeField] private float maxHealth = 100f; // 최대 체력
    [SerializeField] private bool enableAutoRecover = false; // 자동 회복 기능 활성화 여부
    [SerializeField] private bool isDamageResetRecoverTimer = false; // 대미지 받을 때 회복 타이머 초기화 여부
    [SerializeField] private float autoRecoverStartDelay = 10f; // 자동 회복 기능 활성화 딜레이
    [SerializeField] private float autoRecoverPerSec = 3f; // 초당 회복량
    public float currentHealth { get; private set; } // 현재 체력
    private float recoverTimer = 0f; // 회복 타이머

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        CheckHealthCondition();
    }

    /// <summary>
    /// 플레이어에게 대미지 적용
    /// </summary>
    /// <param name="amount">대미지 양</param>
    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 범위 설정

        if (isDamageResetRecoverTimer)
        {
            recoverTimer = 0; // 공격 받으면 회복 타이머 초기화
        }
    }
    
    /// <summary>
    /// 플레이어 체력 상태 확인
    /// </summary>
    public void CheckHealthCondition()
    {
        if (currentHealth <= 0) // 0 이하인 경우 플레이어 사망
        {
            Die();
        }
        else if (currentHealth < maxHealth) // 0 < currentHealth < maxHealth
        {
            if (enableAutoRecover)
            {
                recoverTimer += Time.deltaTime;
                if (recoverTimer >= autoRecoverStartDelay)
                {
                    RecoverHealth(autoRecoverPerSec, false);
                }
            }
        }    
    }

    /// <summary>
    /// 플레이어 체력 회복
    /// </summary>
    /// <param name="amount">회복할 체력의 양</param>
    /// <param name="isAtOnce">한 번에 체력을 회복할 지 여부</param>
    public void RecoverHealth(float amount, bool isAtOnce)
    {
        if (isAtOnce)
        {
            currentHealth += amount;         
        }
        else
        {
            currentHealth += amount * Time.deltaTime; // 초당 amount만큼 회복
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 범위 설정
    }

    public void Die()
    {
        Debug.Log("Player Died");
    }
}
