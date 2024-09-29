using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("ü�� ����")]
    [SerializeField] private float maxHealth = 100f; // �ִ� ü��
    [SerializeField] private bool enableAutoRecover = false; // �ڵ� ȸ�� ��� Ȱ��ȭ ����
    [SerializeField] private bool isDamageResetRecoverTimer = false; // ����� ���� �� ȸ�� Ÿ�̸� �ʱ�ȭ ����
    [SerializeField] private float autoRecoverStartDelay = 10f; // �ڵ� ȸ�� ��� Ȱ��ȭ ������
    [SerializeField] private float autoRecoverPerSec = 3f; // �ʴ� ȸ����
    public float currentHealth { get; private set; } // ���� ü��
    private float recoverTimer = 0f; // ȸ�� Ÿ�̸�

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        CheckHealthCondition();
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

    public void Die()
    {
        Debug.Log("Player Died");
    }
}
