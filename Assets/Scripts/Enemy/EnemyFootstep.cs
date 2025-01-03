using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFootstep : MonoBehaviour
{
    // 컴포넌트
    private AudioSource audioSource;
    private Enemy enemy;
    private CapsuleCollider capCol;

    [Header("사운드")]
    [SerializeField] private AudioClip[] defaultSound;
    [SerializeField] private AudioClip[] dirtSound;
    [SerializeField] private AudioClip[] woodSound;
    [SerializeField] private AudioClip[] waterSound;
    private AudioClip currentFootstepSound; // 재질에 따라 정해진 사운드

    [Header("설정")]
    [SerializeField] private float walkVolume = 0.2f;
    [SerializeField] private float sprintVolume = 0.4f;
    [SerializeField] private float walkStepInterval;
    [SerializeField] private float sprintStepInterval;

    private int surfaceType;
    private float stepIntervalTimer = 0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        enemy = GetComponent<Enemy>();
        capCol = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {       
        audioSource.volume = walkVolume;
    }

    // Update is called once per frame
    void Update()
    {
        PlayFootstepSound();
    }

    /// <summary>
    /// 바닥 재질 확인 후 currentFootstepSound에 해당 재질 사운드 할당
    /// </summary>
    private void CheckSurfaceType()
    {
        float yDownHeight = transform.position.y - (capCol.height / 2f - capCol.center.y);

        // 아래 방향으로 Ray 발사
        RaycastHit hit;
        Vector3 rayStartPos = new Vector3(transform.position.x, yDownHeight, transform.position.z);

        Debug.DrawRay(rayStartPos, Vector3.down, Color.red);

        if (Physics.Raycast(rayStartPos, Vector3.down, out hit, 0.5f))
        {
            surfaceType = hit.transform.gameObject.layer;
        }

        // surfaceType에 따라 사운드 결정
        if (surfaceType == LayerMask.NameToLayer("SURFACE_DIRT"))
        {
            currentFootstepSound = dirtSound[Random.Range(0, dirtSound.Length)];
        }
        else if (surfaceType == LayerMask.NameToLayer("SURFACE_WOOD"))
        {
            currentFootstepSound = woodSound[Random.Range(0, woodSound.Length)];
        }
        else if (surfaceType == LayerMask.NameToLayer("SURFACE_WATER"))
        {
            currentFootstepSound = waterSound[Random.Range(0, waterSound.Length)];
        }
        else
        {
            currentFootstepSound = defaultSound[Random.Range(0, defaultSound.Length)];
        }
    }

    /// <summary>
    /// surfaceType에 따라 다른 사운드 재생
    /// </summary>
    private void PlayFootstepSound()
    {
        bool cond1 = enemy.navAgent.speed == enemy.patrolSpeed; // 걷기
        bool cond2 = enemy.navAgent.speed == enemy.chaseSpeed; // 달리기

        // 이동 시 발소리 재생
        if (cond1 || cond2)
        {
            float interval = 0f;

            // 상태 별 값 설정
            if (cond1)
            {
                audioSource.volume = walkVolume;
                interval = walkStepInterval;
            }
            else if (cond2)
            {
                audioSource.volume = sprintVolume;
                interval = sprintStepInterval;
            }

            // surfaceType을 확인해 재질에 맞는 랜덤 사운드 결정
            CheckSurfaceType();

            // 그 재질에 맞는 랜덤 사운드 재생
            stepIntervalTimer += Time.deltaTime; // 발소리 재생 간격을 위한 타이머
            if (stepIntervalTimer > interval)
            {
                stepIntervalTimer = 0; // 타이머 초기화
                audioSource.PlayOneShot(currentFootstepSound);
            }
        }
    }
}
