using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾� ü�� ���� ��ũ��Ʈ
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    // ������Ʈ
    private PlayerMovementHelper playerMovementHelper;
    private AudioSource audioSource;

    [Header("����")]
    [SerializeField] private AudioClip[] fallDamageSound; // ���� ����� ����

    [Header("ü�� ����")]
    [SerializeField] private float maxHealth = 100f; // �ִ� ü��
    [SerializeField] private bool enableAutoRecover = false; // �ڵ� ȸ�� ��� Ȱ��ȭ ����
    [SerializeField] private bool isDamageResetRecoverTimer = false; // ����� ���� �� ȸ�� Ÿ�̸� �ʱ�ȭ ����
    [SerializeField] private float autoRecoverStartDelay = 10f; // �ڵ� ȸ�� ��� Ȱ��ȭ ������
    [SerializeField] private float autoRecoverPerSec = 3f; // �ʴ� ȸ����

    [Header("�߶� �����")]
    [SerializeField] private float damagableHeight = 3f; // ������� ���� �� �ִ� �ּ� ����
    [SerializeField] private float deathHeight = 8f; // ��� ����
    [SerializeField] private float baseFallDamage = 20f; // �ּ� �����
    [SerializeField] private float fallDamageMultiplier = 1.5f;
    
    // ��Ÿ ����
    public float currentHealth { get; private set; } // ���� ü��
    public bool isDied { get; private set; } // ��� ����
    private float recoverTimer = 0f; // ȸ�� Ÿ�̸�

    private void Start()
    {
        playerMovementHelper = GetComponent<PlayerMovementHelper>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;

        // �̺�Ʈ ����
        SubscribeEvent();
    }

    private void Update()
    {
        CheckHealthCondition();
    }

    /// <summary>
    /// �̺�Ʈ ���� �޼���
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
    /// �÷��̾�� ����� ����
    /// </summary>
    /// <param name="amount">����� ��</param>
    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // ���� ����

        if (isDamageResetRecoverTimer)
        {
            recoverTimer = 0; // ���� ������ ȸ�� Ÿ�̸� �ʱ�ȭ
        }
    }
    
    /// <summary>
    /// �÷��̾� ü�� ���� Ȯ��
    /// </summary>
    public void CheckHealthCondition()
    {
        if (currentHealth <= 0) // 0 ������ ��� �÷��̾� ���
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
    /// �÷��̾� ü�� ȸ��
    /// </summary>
    /// <param name="amount">ȸ���� ü���� ��</param>
    /// <param name="isAtOnce">�� ���� ü���� ȸ���� �� ����</param>
    public void RecoverHealth(float amount, bool isAtOnce)
    {
        if (isAtOnce)
        {
            currentHealth += amount;         
        }
        else
        {
            currentHealth += amount * Time.deltaTime; // �ʴ� amount��ŭ ȸ��
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // ���� ����
    }

    /// <summary>
    /// �÷��̾� ��� ó��
    /// </summary>
    public void Die()
    {
        Debug.Log("Player Died");
        isDied = true;
    }

    /// <summary>
    /// �߶� ���̿� ���� �߶� ������ ��� �� ����
    /// </summary>
    /// <param name="fallHeight">�߶� ����</param>
    private void ApplyFallDamage(float fallHeight)
    {
        if (fallHeight >= damagableHeight)
        {
            float fallDamage = 0;
            if (fallHeight >= deathHeight) // ��� ����� ����
            {
                fallDamage = maxHealth;
            }
            else // ���̿� ����� ����� ����
            {
                // ����� = �⺻ �߶� ����� + (�߶� ���� * ����� ���) 
                fallDamage = baseFallDamage + (fallHeight * fallDamageMultiplier);
            } 
            
            fallDamage = Mathf.Clamp(fallDamage, baseFallDamage, maxHealth); // baseFallDamage ~ �ִ� ü��
            ApplyDamage(fallDamage);

            // �߶� ���� ���
            audioSource.volume = 1f;
            audioSource.PlayOneShot(fallDamageSound[UnityEngine.Random.Range(0, fallDamageSound.Length)]);
        }
    }
}
