using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾� ���¹̳� ���� ��ũ��Ʈ
/// </summary>
public class PlayerStamina : MonoBehaviour
{
    // ������Ʈ
    private PlayerStateMachine psm;
    private PlayerMovement pm;

    [Header("���¹̳� ����")]
    public float maxStamina = 100f; // �ִ� ���¹̳�
    public float currentStamina { get; private set; } // ���� ���¹̳�
    public float sprintStaminaDrainPerSec = 10f; // �޸��� �ʴ� ���¹̳� �Ҹ�
    public float jumpStaminaDrain = 20f; // ���� �� �Ҹ�Ǵ� ���¹̳�
    public float staminaRegenDelay = 2f; // ���¹̳� ȸ�� ���� �ð�
    public float staminaRegenPerSec = 5f; // �ʴ� ȸ���Ǵ� ���¹̳� ��

    private float regenTimer; // ȸ�� ��� �ð� Ÿ�̸�
    public bool hasEnoughStamina_Jump { get; private set; }
    public bool hasEnoughStamina_Sprint { get; private set; }

    

    // Start is called before the first frame update
    void Start()
    {
        psm = GetComponent<PlayerStateMachine>();
        pm = GetComponent<PlayerMovement>();

        currentStamina = maxStamina;

        // �̺�Ʈ ����
        pm.StartJumpAction += () => DrainStamina(jumpStaminaDrain, true); // ���� �̺�Ʈ ����� ���¹̳� ����
    }

    // Update is called once per frame
    void Update()
    {
        CheckDrainCondition();
        CheckRecoverCondition();
        CheckEnoughStaminaCondition();
    }

    /// <summary>
    /// Ư�� �ൿ�� �� �� �ִ� ���¹̳ʰ� ����� �ִ� �� ��� ������Ʈ
    /// </summary>
    private void CheckEnoughStaminaCondition()
    {
        // �޸���
        if (currentStamina > 0)
        {
            hasEnoughStamina_Sprint = true;
        }
        else
        {
            hasEnoughStamina_Sprint = false;
        }

        // ����
        if (currentStamina >= jumpStaminaDrain)
        {
            hasEnoughStamina_Jump = true;
        }
        else
        {
            hasEnoughStamina_Jump = false;
        }
    }

    /// <summary>
    /// ���¹̳� ���� ���� Ȯ��
    /// </summary>
    private void CheckDrainCondition()
    {
        if (psm.CurrentMoveState == psm.sprintState) // �޸���
        {
            if (currentStamina > 0f)
            {
                DrainStamina(sprintStaminaDrainPerSec, false);
            }
        }
    }

    /// <summary>
    /// ���¹̳ʸ� �ʴ� amount��ŭ ���ҽ�Ŵ
    /// </summary>
    /// <param name="amount">���ҽ�ų ��</param>
    /// <param name="isAtOnce">���¹̳ʸ� �ʴ��� �ƴ� �� ���� ���ҽ�ų�� ����</param>
    private void DrainStamina(float amount, bool isAtOnce)
    {
        if (isAtOnce)
        {
            currentStamina -= amount;
        }
        else
        {
            currentStamina -= amount * Time.deltaTime;
        }
    }

    /// <summary>
    /// ���¹̳� ȸ�� ���� �˻�
    /// </summary>
    private void CheckRecoverCondition()
    {
        bool cond1 = !(psm.CurrentMoveState == psm.sprintState) && !pm.isJumping; // �޸���� ������ ���� �ʰ� ����
        bool cond2 = currentStamina < maxStamina; // ȸ���� ���¹̳ʰ� �ִ� ���

        if (cond1 && cond2)
        {
            regenTimer += Time.deltaTime; // ȸ�� ���� Ÿ�̸� ���
            if (regenTimer >= staminaRegenDelay) // �����̸�ŭ �ð��� ���� ���
            {
                RecoverStamina(staminaRegenPerSec, false);
            }
        }
        else
        {
            regenTimer = 0; // �ൿ �߿��� Ÿ�̸� �ʱ�ȭ
        }
    }

    /// <summary>
    /// ���¹̳� ȸ��
    /// </summary>
    /// <param name="amount">ȸ���� ���¹̳� ��</param>
    /// <param name="isAtOnce">�� ���� ȸ���� �� ����</param>
    private void RecoverStamina(float amount, bool isAtOnce)
    {
        if (isAtOnce)
        {
            currentStamina += amount;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // ���� ����
        }
        else
        {
            currentStamina += amount * Time.deltaTime; // �ʴ� amount��ŭ ȸ��
        }    
    }
}
