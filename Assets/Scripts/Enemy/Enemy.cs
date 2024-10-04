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
    [SerializeField] private CircleQTEUI circleQTEUI;

    // 오브젝트
    private GameObject player;

    // 상태
    [Header("State 설정")]
    [SerializeField] private float updatePeriod = 0.2f; // 업데이트 코루틴 호출 주기 (초 단위)

    [Header("이동 및 NavMesh")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    [SerializeField] private float patrolPointDistance = 50f; // 새롭게 찾을 랜덤한 순찰 지점까지의 최대 거리

    [Header("공격")]
    [SerializeField] private float attackDamage = 30f;
    [SerializeField] private float attackDelay = 3f;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private Transform attackRoot; // 해당 루트를 기준으로 일정 범위 공격
    private float attackTimer;
    [HideInInspector] public bool isAttacking = false;

    [Header("플레이어 탐지")]
    [SerializeField] private float detectionFov = 50f; // 시아갹
    [SerializeField] private float detectionDistance = 30f; // 시야 거리
    [SerializeField] private Transform eyeTransform; // 시야 기준점
    [SerializeField] private float targetLostTime = 10f; // 대상이 시야 범위 내에 없을 때 Chase 상태에서 벗어나는 데 걸리는 시간
    [HideInInspector] public Transform chaseTarget; // 추적 대상
    private bool isInSight = false; // 추적 대상이 시야 범위 내에 있는지 여부
    private Collider sightCol; // 시야 범위 내에서 감지한 추적 대상의 Collider
    private float timeSinceLastSeen; // 대상을 마지막으로 감지한 시점부터 지난 시간

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

        // 상태 머신 초기화
        esm.Initialize(esm.patrolState);

        // 이벤트 구독
        SubscribeEvent();

        // 공격 타이머 초기화
        attackTimer = attackDelay; // 처음에 바로 공격할 수 있도록 타이머를 딜레이만큼 설정
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
        Collider[] colliders = Physics.OverlapSphere(eyeTransform.position, detectionDistance); // 중심이 eyeTransform이고 반지름이 시야 거리인 구 안에 있는 Collider를 모두 가져옴

        foreach (Collider col in colliders)
        {
            if (AIUtil.IsTargetOnSight(col.transform, eyeTransform, detectionFov, detectionDistance, "Player")) // 대상이 시야각 내에 들어오고, 다른 물체에 가려지지 않으면
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
        if (navAgent.destination == null || navAgent.remainingDistance <= 1f) // Waypoint가 없거나, 거의 도달한 상태면
        {
            // NavMesh 상의 임의의 위치를 새로운 Waypoint로 설정
            Vector3 patrolDestination = AIUtil.GetRandomPointOnNavMesh(this.transform.position, patrolPointDistance, NavMesh.AllAreas);
            navAgent.SetDestination(patrolDestination);
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
}
