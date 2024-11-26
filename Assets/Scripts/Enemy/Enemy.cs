using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // 컴포넌트
    [Header("컴포넌트")]
    private AudioSource audioSource;
    [HideInInspector] public NavMeshAgent navAgent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public EnemyStateMachine esm;
    private GameObject managers;
    private CircleQTEUI circleQTEUI;
    private PlayerFlashlight plyFlashlight;

    // 오브젝트
    private GameObject player;

    // 상태
    [Header("State 설정")]
    [SerializeField] private float updatePeriod = 0.2f; // 업데이트 코루틴 호출 주기 (초 단위)

    [Header("이동 및 NavMesh")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    [SerializeField] private bool isSamplingPatrolPoint = false; // 순찰 지점을 랜덤으로 샘플링?
    [SerializeField] private float patrolPointDistance = 50f; // 새롭게 찾을 랜덤한 순찰 지점까지의 최대 거리
    [SerializeField] private Transform[] patrolPoints;
    private int lastPatrolPointIdx = 0;
    private Transform lastPatrolPoint = null;
    private bool isArrived = false;
    [SerializeField] private bool isDestPlayerLastPos = false;

    [Header("공격")]
    [SerializeField] private float attackDamage = 30f;
    [SerializeField] private float attackDelay = 3f;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private Transform attackRoot; // 해당 루트를 기준으로 일정 범위 공격
    private float attackTimer;
    [HideInInspector] public bool isAttacking = false;

    [Header("플레이어 탐지")]
    [SerializeField] private float detectionFov = 50f; // 시아갹
    [SerializeField] private float detectionDistance = 25f; // 시야 거리
    [SerializeField] private float flashlightDetectionDistance = 35f; // 시야 거리
    [SerializeField] private Transform eyeTransform; // 시야 기준점
    [SerializeField] private float targetLostTime = 10f; // 대상이 시야 범위 내에 없을 때 Chase 상태에서 벗어나는 데 걸리는 시간
    [HideInInspector] public Transform chaseTarget; // 추적 대상
    private bool isInSight = false; // 추적 대상이 시야 범위 내에 있는지 여부
    private Collider sightCol; // 시야 범위 내에서 감지한 추적 대상의 Collider
    private float timeSinceLastSeen; // 대상을 마지막으로 감지한 시점부터 지난 시간

    [Header("체력")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [HideInInspector] public bool isDead = false;

    [Header("사운드")]
    [SerializeField] private AudioClip attackSound;

    // 유니티 에디터 내에서만 동작
#if UNITY_EDITOR

    // Enemy 스크립트를 가진 오브젝트가 선택됐을 때 매 프레임마다 실행
    // 여기에서는 시야와 공격 범위를 그림
    private void OnDrawGizmosSelected()
    {
        if (attackRoot != null) // 공격 범위
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(attackRoot.position, attackRadius);
        }

        if (eyeTransform != null)
        {
            Quaternion leftEyeRotation = Quaternion.AngleAxis(-detectionFov * 0.5f, Vector3.up); // 호의 왼쪽 끝 지점을 향하는 각도
            Vector3 leftRayDirection = leftEyeRotation * transform.forward; // 앞쪽 방향에서 leftEyeRotation만큼 왼쪽으로 회전
            Handles.color = new Color(1f, 1f, 1f, 0.2f);
            Handles.DrawSolidArc(eyeTransform.position, Vector3.up, leftRayDirection, detectionFov, detectionDistance); // 호 그리기. from부터 angle만큼 원을 그리면서 이동
        }
    }

#endif

    private void Start()
    {
        // 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        esm = GetComponent<EnemyStateMachine>();
        managers = GameObject.Find("Managers");
        circleQTEUI = managers.GetComponent<UIManager>().circleQTEUI;
        plyFlashlight = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerFlashlight>();

        // 상태 머신 초기화
        esm.Initialize(esm.patrolState);

        // 이벤트 구독
        SubscribeEvent();

        // 공격 타이머 초기화
        attackTimer = attackDelay; // 처음에 바로 공격할 수 있도록 타이머를 딜레이만큼 설정

        // 체력 설정
        currentHealth = maxHealth;
    }
    private void SubscribeEvent()
    {
        circleQTEUI.OnQTEFail += () =>
        {
            MoveToQTEFailPos();
        };
    }

    /// <summary>
    /// 시야 범위 안에 들어오는지 여부
    /// </summary>
    /// <returns></returns>
    private bool IsCaughtInSight()
    {
        // 먼저 구 안에 있는 모든 콜라이더를 가져온 후, 시야 범위 내있는 대상 중 Ray에 맞은 콜라이더만 구분해 다시 가져옴
        // 중심이 eyeTransform이고 반지름이 시야 거리인 구 안에 있는 Collider를 모두 가져옴
        Collider[] colliders = Physics.OverlapSphere(eyeTransform.position, 
            plyFlashlight.isLightOn ? flashlightDetectionDistance : detectionDistance); // 플래시가 켜져 있으면 시야 거리 늘림

        foreach (Collider col in colliders)
        {
            // 대상이 시야각 내에 들어오고, 다른 물체에 가려지지 않으면
            if (AIUtil.IsTargetOnSight(col.transform, eyeTransform, detectionFov,
                plyFlashlight.isLightOn ? flashlightDetectionDistance : detectionDistance, "Player"))
            {
                sightCol = col;
                return true;
            }
        }

        return false;
    }

    private void CheckSight()
    {
        isInSight = IsCaughtInSight(); // 시야 범위 검사
    }

    // ----------------------------- Patrol -----------------------------

    /// <summary>
    /// updatePeriod 초마다 Chase 상태에 필요한 업데이트 기능 실행
    /// </summary>
    public void UpdatePatrolStatus()
    {
        StartCoroutine("UpdatePatrolStatusCoroutine");
    }

    public IEnumerator UpdatePatrolStatusCoroutine()
    {
        CheckSight(); // 시야 범위 검사
        SetPatrolWaypoint(); // Patrol Waypoint 설정
        SetChaseTargetInSight(); // 추적 대상 검사

        yield return new WaitForSeconds(updatePeriod);
    }

    /// <summary>
    /// 이동할 Waypoint 설정
    /// </summary>
    private void SetPatrolWaypoint()
    {
        if (isSamplingPatrolPoint)
        {
            if (navAgent.destination == null || navAgent.remainingDistance <= 2f) // Waypoint가 없거나, 거의 도달한 상태면
            {
                // NavMesh 상의 임의의 위치를 새로운 Waypoint로 설정
                Vector3 patrolDestination = AIUtil.GetRandomPointOnNavMesh(this.transform.position, patrolPointDistance, NavMesh.AllAreas);
                navAgent.SetDestination(patrolDestination);
            }
        }
        else
        {
            // 순찰 경로 따라서 순차적으로 움직임
            // 1. partrolPoints 배열 (크기 고정)
            // 2. modulo 연산으로 사이즈만큼 계속 돌면서 웨이포인트 설정
            // 3. 추적하다가 다시 순찰로 돌아갈 때 마지막으로 이동하던 웨이포인트로 이동
            if (navAgent.destination == null || navAgent.destination == transform.position) // Waypoint가 없으면
            {
                lastPatrolPoint = patrolPoints[lastPatrolPointIdx];
                navAgent.SetDestination(lastPatrolPoint.position);
                isArrived = false;
            }
            else if (navAgent.remainingDistance <= 2f && !isArrived) // 거의 도달한 상태면 웨이포인트 인덱스 증가
            {
                isArrived = true;

                // 만약 도착한 지점이 플레이어의 마지막 위치라면
                // 마지막으로 이동하던 웨이포인트로 이동하기 위해 return
                if (isDestPlayerLastPos) // 정지 거리 포함
                {
                    isDestPlayerLastPos = false;
                    return;
                }

                lastPatrolPointIdx = (lastPatrolPointIdx + 1) % patrolPoints.Length;    
            }
        }


    }

    /// <summary>
    /// 시야에 들어오는 적을 감지해 추적 대상으로 설정
    /// </summary>
    private void SetChaseTargetInSight()
    {
        if (isInSight)
        {
            chaseTarget = sightCol.transform;
        }
    }

    /// <summary>
    /// QTE 실패시 플레이어의 위치를 추적 목표로 설정
    /// </summary>
    private void MoveToQTEFailPos()
    {
        chaseTarget = GameObject.FindWithTag("Player").transform;
    }

    // ----------------------------- Chase -----------------------------

    /// <summary>
    /// updatePeriod 초마다 Chase 상태에 필요한 업데이트 기능 실행
    /// </summary>
    public void UpdateChaseStatus()
    {
        StartCoroutine("UpdateChaseStatusCoroutine");
    }

    public IEnumerator UpdateChaseStatusCoroutine()
    {
        CheckSight(); // 시야 범위 검사
        UpdateChaseTargetPosition(); // 추적 대상 위치 갱신
        CheckDistanceToTarget(); // 추적 대상과의 거리 확인
        CheckLostTarget(); // 추적 대상 놓쳤는 지 확인

        yield return new WaitForSeconds(updatePeriod);
    }

    /// <summary>
    /// NavAgent의 destination을 계속 업데이트
    /// </summary>
    private void UpdateChaseTargetPosition()
    {
        if (chaseTarget != null)
        {
            navAgent.SetDestination(chaseTarget.position);
            isDestPlayerLastPos = true;
        }
    }

    /// <summary>
    /// 플레이어를 일정 시간 동안 찾지 못한 상태를 체크
    /// </summary>
    private void CheckLostTarget()
    {
        if (!isInSight)
        {
            timeSinceLastSeen += Time.deltaTime;

            if (timeSinceLastSeen >= targetLostTime)
            {
                chaseTarget = null; // 목표를 놓친 경우 null로 설정
            }
        }
        else
        {
            timeSinceLastSeen = 0;
        }
    }

    /// <summary>
    /// 타겟과 일정한 거리를 유지하기 위해 일정 거리 내로 들어오면 멈춤
    /// </summary>
    private void CheckDistanceToTarget()
    {
        if (chaseTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, chaseTarget.position);

            if (distanceToTarget <= 1.0f)
            {
                navAgent.isStopped = true;
            }
            else
            {
                navAgent.isStopped = false;
            }
        }
    }

    // ----------------------------- Attack -----------------------------
    /// <summary>
    /// updatePeriod 초마다 Chase 상태에 필요한 업데이트 기능 실행
    /// </summary>
    public void UpdateAttackStatus()
    {
        StartCoroutine("UpdateAttackStatusCoroutine");
    }

    public IEnumerator UpdateAttackStatusCoroutine()
    {
        CheckAttackCondition(); // 공격 조건 확인

        yield return new WaitForSeconds(updatePeriod);
    }

    private bool IsPlayerInAttackRange(ref GameObject playerObj)
    {
        // 공격 범위(구) 내에 있는 모든 Collider를 가져옴
        Collider[] colliders = Physics.OverlapSphere(attackRoot.position, attackRadius); // 중심이 attackRoot이고 반지름이 attackRadius인 구 안에 있는 Collider를 모두 가져옴

        foreach (Collider col in colliders)
        {
            if (col.gameObject.tag == "Player") // 플레이어인 경우
            {
                playerObj = col.gameObject;
                return true;
            }
        }

        playerObj = null;
        return false;
    }

    private void CheckAttackCondition()
    {
        if (isAttacking) return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackDelay)
        {
            if (IsPlayerInAttackRange(ref player)) // Player가 범위 내에 있는 경우 레퍼런스로 컴포넌트 받아옴
            {
                navAgent.speed = 0f; // 공격 범위 내에 있는 경우 멈춤
                isAttacking = true;
                animator.SetTrigger("Attack");
            }
        }
    }

    private void AttackStart()
    {

    }

    private void Attack()
    {
        audioSource.volume = 0.2f;
        audioSource.PlayOneShot(attackSound);
        player.GetComponent<PlayerHealth>().ApplyDamage(attackDamage);
    }

    private void AttackEnd()
    {
        isAttacking = false;
        attackTimer = 0; // 타이머 초기화
    }

    /// <summary>
    /// 적에게 대미지 적용
    /// </summary>
    /// <param name="amount">대미지 양</param>
    public void ApplyDamage(float amount, Transform attacker)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 범위 설정

        // 공격자 추적 시작
        chaseTarget = attacker;

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// 적 사망시 호출
    /// </summary>
    public void Death()
    {
        isDead = true;

        // 래그돌 생성      
        GetComponent<CapsuleCollider>().enabled = false;
        audioSource.enabled = false;
        navAgent.enabled = false;
        esm.enabled = false;
        GetComponent<EnemyFootstep>().enabled = false;
        animator.enabled = false;
        this.enabled = false;

        managers.GetComponent<GameManager>().eliminatedCount += 1;
    }
}
