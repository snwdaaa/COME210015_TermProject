using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 체력 관리 스크립트
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    // 컴포넌트
    private PlayerMovementHelper playerMovementHelper;
    private AudioSource audioSource;

    [Header("사운드")]
    [SerializeField] private AudioClip[] fallDamageSound; // 낙하 대미지 사운드

    [Header("체력 설정")]
    [SerializeField] private float maxHealth = 100f; // 최대 체력 초기 설정값
    public float currentMaxHealth { get; private set; } // 현재 최대 체력
    [SerializeField] private bool enableAutoRecover = false; // 자동 회복 기능 활성화 여부
    [SerializeField] private bool isDamageResetRecoverTimer = false; // 대미지 받을 때 회복 타이머 초기화 여부
    [SerializeField] private float autoRecoverStartDelay = 10f; // 자동 회복 기능 활성화 딜레이
    [SerializeField] private float autoRecoverPerSec = 3f; // 초당 회복량

    [Header("추락 대미지")]
    [SerializeField] private float damagableHeight = 3f; // 대미지를 받을 수 있는 최소 높이
    [SerializeField] private float deathHeight = 8f; // 즉사 높이
    [SerializeField] private float baseFallDamage = 20f; // 최소 대미지
    [SerializeField] private float fallDamageMultiplier = 1.5f;
    
    // 기타 변수
    public float currentHealth { get; private set; } // 현재 체력
    public bool isDied { get; private set; } // 사망 여부
    private float recoverTimer = 0f; // 회복 타이머

    private void Start()
    {
        playerMovementHelper = GetComponent<PlayerMovementHelper>();
        audioSource = GetComponent<AudioSource>();

        currentMaxHealth = maxHealth;
        currentHealth = currentMaxHealth;

        // 이벤트 구독
        SubscribeEvent();
    }

    private void Update()
    {
        CheckHealthCondition();
    }

    /// <summary>
    /// 이벤트 구독 메서드
    /// </summary>
    private void SubscribeEvent()
    {
        // Landing Event
        playerMovementHelper.PlayerLandingEvent += () =>
        {
            ApplyFallDamage(playerMovementHelper.fallHeight);
        };
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

    /// <summary>
    /// 플레이어 사망 처리
    /// </summary>
    public void Die()
    {
        Debug.Log("Player Died");
        isDied = true;
    }

    /// <summary>
    /// 추락 높이에 따라 추락 데미지 계산 후 적용
    /// </summary>
    /// <param name="fallHeight">추락 높이</param>
    private void ApplyFallDamage(float fallHeight)
    {
        if (fallHeight >= damagableHeight)
        {
            float fallDamage = 0;
            if (fallHeight >= deathHeight) // 즉사 대미지 적용
            {
                fallDamage = maxHealth;
            }
            else // 높이에 비례한 대미지 적용
            {
                // 대미지 = 기본 추락 대미지 + (추락 높이 * 대미지 배수) 
                fallDamage = baseFallDamage + (fallHeight * fallDamageMultiplier);
            } 
            
            fallDamage = Mathf.Clamp(fallDamage, baseFallDamage, maxHealth); // baseFallDamage ~ 최대 체력
            ApplyDamage(fallDamage);

            // 추락 사운드 재생
            audioSource.volume = 1f;
            audioSource.PlayOneShot(fallDamageSound[UnityEngine.Random.Range(0, fallDamageSound.Length)]);
        }
    }
}
