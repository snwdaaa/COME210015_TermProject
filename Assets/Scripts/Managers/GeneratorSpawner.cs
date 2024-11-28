using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject generatorPrefab;
    [Header("Properties")]
    [SerializeField] private Transform[] spawnPoints;
    public int spawnCount; // 생성할 발전기의 개수
    [Header("Game Objects")]
    public Slider progressBar;
    public GameObject qteUIObject; // CircleQTEUI 컴포넌트를 가지는 오브젝트
    public GameObject progressUIObject;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.gameMode == GameManager.GameMode.Doom) 
            this.enabled = false;

        spawnCount = Mathf.Clamp(spawnCount, 1, spawnPoints.Length); // 예외 범위 보정
        StartCoroutine("SpawnGeneratorCoroutine");
    }

    IEnumerator SpawnGeneratorCoroutine()
    {
        yield return new WaitForSeconds(0.01f);

        if (GameManager.gameMode == GameManager.GameMode.Normal)
        {
            SpawnGenerator();
        }
    }

    private void SpawnGenerator()
    {
        Transform[] randomPoints = GetRandomSpawnpoints();

        foreach (Transform point in randomPoints)
        {
            Instantiate(generatorPrefab, point.position, point.rotation); // 발전기 spawn
        }
    }

    /// <summary>
    /// Spawnpoint 배열에서 랜덤으로 spawnCount개 만큼 뽑아 randomPoints에 저장 후 리턴
    /// </summary>
    /// <returns></returns>
    private Transform[] GetRandomSpawnpoints()
    {
        List<Transform> shuffledPoints = spawnPoints.ToList();
        
        for (int i = 0; i < shuffledPoints.Count; i++) // 랜덤하게 셔플
        {
            int randIdx = Random.Range(i, shuffledPoints.Count);
            (shuffledPoints[i], shuffledPoints[randIdx]) = (shuffledPoints[randIdx], shuffledPoints[i]);
        }

        // 상위 spawnCount개의 포인트 반환
        return shuffledPoints.Take(spawnCount).ToArray();
    }

}
