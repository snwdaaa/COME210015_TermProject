using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 스태미너 관리 스크립트
/// </summary>
public class PlayerStamina : MonoBehaviour
{
    // 컴포넌트
    private PlayerStateMachine psm;
    private PlayerMovement pm;

    [Header("스태미너 설정")]
    public float maxStamina = 100f; // 최대 스태미너
    public float currentStamina { get; private set; } // 현재 스태미너
    public float sprintStaminaDrainPerSec = 10f; // 달리기 초당 스태미너 소모량
    public float jumpStaminaDrain = 20f; // 점프 시 소모되는 스태미너
    public float staminaRegenDelay = 2f; // 스태미너 회복 지연 시간
    public float staminaRegenPerSec = 5f; // 초당 회복되는 스태미너 양

    private float regenTimer; // 회복 대기 시간 타이머
    public bool hasEnoughStamina_Jump { get; private set; }
    public bool hasEnoughStamina_Sprint { get; private set; }

    

    // Start is called before the first frame update
    void Start()
    {
        psm = GetComponent<PlayerStateMachine>();
        pm = GetComponent<PlayerMovement>();

        currentStamina = maxStamina;

        // 이벤트 구독
        pm.StartJumpAction += () => DrainStamina(jumpStaminaDrain, true); // 점프 이벤트 실행시 스태미너 감소
    }

    // Update is called once per frame
    void Update()
    {
        CheckDrainCondition();
        CheckRecoverCondition();
        CheckEnoughStaminaCondition();
    }

    /// <summary>
    /// 특정 행동을 할 수 있는 스태미너가 충분히 있는 지 계속 업데이트
    /// </summary>
    private void CheckEnoughStaminaCondition()
    {
        // 달리기
        if (currentStamina > 0)
        {
            hasEnoughStamina_Sprint = true;
        }
        else
        {
            hasEnoughStamina_Sprint = false;
        }

        // 점프
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
    /// 스태미너 감소 조건 확인
    /// </summary>
    private void CheckDrainCondition()
    {
        if (psm.CurrentMoveState == psm.sprintState) // 달리기
        {
            if (currentStamina > 0f)
            {
                DrainStamina(sprintStaminaDrainPerSec, false);
            }
        }
    }

    /// <summary>
    /// 스태미너를 초당 amount만큼 감소시킴
    /// </summary>
    /// <param name="amount">감소시킬 양</param>
    /// <param name="isAtOnce">스태미너를 초당이 아닌 한 번에 감소시킬지 여부</param>
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
    /// 스태미너 회복 조건 검사
    /// </summary>
    private void CheckRecoverCondition()
    {
        bool cond1 = !(psm.CurrentMoveState == psm.sprintState) && !pm.isJumping; // 달리기와 점프를 하지 않고 있음
        bool cond2 = currentStamina < maxStamina; // 회복할 스태미너가 있는 경우

        if (cond1 && cond2)
        {
            regenTimer += Time.deltaTime; // 회복 시작 타이머 계산
            if (regenTimer >= staminaRegenDelay) // 딜레이만큼 시간이 지난 경우
            {
                RecoverStamina(staminaRegenPerSec, false);
            }
        }
        else
        {
            regenTimer = 0; // 행동 중에는 타이머 초기화
        }
    }

    /// <summary>
    /// 스태미너 회복
    /// </summary>
    /// <param name="amount">회복할 스태미너 양</param>
    /// <param name="isAtOnce">한 번에 회복할 지 여부</param>
    private void RecoverStamina(float amount, bool isAtOnce)
    {
        if (isAtOnce)
        {
            currentStamina += amount;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // 범위 제한
        }
        else
        {
            currentStamina += amount * Time.deltaTime; // 초당 amount만큼 회복
        }    
    }
}
