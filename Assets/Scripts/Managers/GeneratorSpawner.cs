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
        SpawnGenerator();
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
        int cnt = 0;
        bool[] isUsed = new bool[spawnPoints.Length]; // 사용 여부 저장할 배열
        Transform[] randomPoints = new Transform[spawnCount];

        while (cnt < spawnCount)
        {
            int randIdx = Random.Range(0, spawnPoints.Length);
            if (!isUsed[randIdx])
            {
                isUsed[randIdx] = true;
                randomPoints[cnt] = spawnPoints[randIdx];
                cnt++;
            }          
        }

        return randomPoints;
    }
}
