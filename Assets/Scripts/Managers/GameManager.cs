using Assets.Pixelation.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    private GeneratorSpawner generatorSpawner;
    [SerializeField] private PlayerMovement plyMovement;
    private MeltScreenController meltScreen;
    private AudioSource ambientSound;
    private AudioSource bgmSound;

    public enum GameMode
    {
        None,
        Normal,
        Doom
    }
    [Header("GameMode")]
    public static GameMode gameMode = GameMode.Normal; // 현재 게임 모드

    [Header("Normal Mode Properties")]
    public static int repairedGeneratorCount = 0; // 수리한 발전기 개수

    [Header("Doom Mode Properties")]
    [SerializeField] private GameObject directionalLight; // 맵을 밝게 할 Directional Light
    public static int eliminatedCount = 0; // 죽인 적의 수
    [SerializeField] private GameObject doomHUD;
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private AudioClip doomBGM;
    [SerializeField] private float doomMoveSpeed = 5.0f;
    public event Action DoomModeSpawnEvent;

    [Header("Exit Properties")]
    [SerializeField] private ExitArea exitArea;
    [SerializeField] private GameObject clearUI;


    // Start is called before the first frame update
    void Start()
    {
        generatorSpawner = GetComponent<GeneratorSpawner>();
        meltScreen = GetComponent<MeltScreenController>();
        ambientSound = GameObject.Find("Ambient").GetComponent<AudioSource>();
        bgmSound = GameObject.Find("BGM").GetComponent<AudioSource>();

        //
        // StartCoroutine("EnterDoomMode");
    }

    // Update is called once per frame
    void Update()
    {
        CheckNormalModeExitConditions();
        CheckDoomModeExitConditions();
    }

    // --------------- Normal Mode ---------------
    /// <summary>
    /// Normal 모드 종료 조건 확인
    /// </summary>
    private void CheckNormalModeExitConditions()
    {
        if (gameMode != GameMode.Normal) return; // 일반 모드가 아닌 경우 return

        // 발전기를 모두 수리했으면 Doom Mode 진입
        if (repairedGeneratorCount >= generatorSpawner.spawnCount)
        {
            StartCoroutine("EnterDoomMode");
        }
    }

    // ---------------  Doom Mode  ---------------
    IEnumerator EnterDoomMode()
    {
        gameMode = GameMode.Doom; // 모드 변경       

        meltScreen.ShowScreen();   
        meltScreen.StartScreenMelt(); // 화면 Melt   

        DoomModeSpawnEvent?.Invoke(); // 랜덤 위치에서 적 생성

        playerHUD.SetActive(false);
        doomHUD.SetActive(true);

        // 카메라 Pixelation 쉐이더 active
        Camera.main.GetComponent<Pixelation>().enabled = true;
        // Directional Light Active
        directionalLight.SetActive(true);

        // BGM 반복 재생 시작
        bgmSound.clip = doomBGM;
        bgmSound.loop = true;
        bgmSound.Play();

        // 이동 속도 변경
        plyMovement.ChangeSpeed(doomMoveSpeed, doomMoveSpeed);

        yield return new WaitForSeconds(5f);
        meltScreen.HideMeltImage();
    }

    private void CheckDoomModeExitConditions()
    {
        if (gameMode != GameMode.Doom) return; // Doom 모드가 아닌 경우 return

        if (eliminatedCount >= EnemySpawner.enemiesOnMap)
        {       
            exitArea.EnableExitArea(); // 탈출구 오픈
        }
    }

    // ---------------    Exit    ---------------
    public void CompleteGame()
    {
        clearUI.SetActive(true);
    }
}