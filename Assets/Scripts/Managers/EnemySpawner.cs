using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform enemyPrefab; // 적 프리팹
    [SerializeField] private int spawnCountInArea = 5; // Box Collider 구역 안에 생성할 적의 수
    public static int enemiesOnMap = 0; // 맵에 있는 전체 적의 수
    private BoxCollider boxCollider;  // 프리팹을 생성할 Box Collider
    private GameManager gameManager;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        gameManager = GameObject.Find("Managers").GetComponent<GameManager>();

        gameManager.DoomModeSpawnEvent += SpawnEnemy; // 이벤트 구독
    }

    void SpawnEnemy()
    {
        if (boxCollider == null)
        {
            Debug.LogError("Box Collider가 할당되지 않았습니다.");
            return;
        }

        // Box Collider의 Bounds 가져오기
        Bounds bounds = boxCollider.bounds;

        for (int i = 0; i < spawnCountInArea; i++)
        {
            // Bounds 내에서 랜덤 위치 생성
            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.min.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            // 프리팹 인스턴스 생성
            Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        }

        enemiesOnMap += spawnCountInArea;
    }
}
