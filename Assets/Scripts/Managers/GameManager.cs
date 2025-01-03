using Assets.Pixelation.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    private GeneratorSpawner generatorSpawner;
    [SerializeField] private PlayerMovement plyMovement;
    [SerializeField] private PlayerCameraMovement plyCamMovement;
    
    private MeltScreenController meltScreen;
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
    public int eliminatedCount = 0; // 죽인 적의 수
    [SerializeField] private GameObject doomHUD;
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private AudioClip doomBGM;
    [SerializeField] private float doomMoveSpeed = 5.0f;
    [SerializeField] private GameObject doomShotgunSprite;
    [SerializeField] private DoomShotgun doomShotgun;
    public event Action DoomModeSpawnEvent;

    [Header("Exit Properties")]
    [SerializeField] private ExitArea exitArea;
    [SerializeField] private GameObject clearUI;
    [SerializeField] private Transform exitCamPos;
    [SerializeField] private Light exitLight;
    [SerializeField] private AudioClip unlockSound;
    private bool cutscenePlayed = false;

    private void Awake()
    {
        generatorSpawner = GetComponent<GeneratorSpawner>();
        meltScreen = GetComponent<MeltScreenController>();

        // 맵에 따라 모드 설정
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "MazeMap")
        {
            gameMode = GameMode.Doom;
            StartCoroutine("EnterDoomMode"); // 둠 모드 진입
        }
        else if (currentScene.name == "HouseMap")
        {
            gameMode = GameMode.Normal;
            repairedGeneratorCount = 0;
        }
        else
        {
            gameMode = GameMode.None;
        }      
    }

    // Start is called before the first frame update
    void Start()
    {
        bgmSound = GameObject.Find("DoomBGM").GetComponent<AudioSource>();   
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
            //StartCoroutine("EnterDoomMode");

            if (!cutscenePlayed)
            {
                StartCoroutine("ShowExitArea");
                exitArea.EnableExitArea(); // 탈출구 오픈
            }
        }
    }

    /// <summary>
    /// 탈출 조건 만족하면 잠시 탈출 지점 보여줌
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowExitArea()
    {
        cutscenePlayed = true;

        Vector3 camOriginPos = Camera.main.transform.position;
        Quaternion camOriginRot = Camera.main.transform.rotation;

        plyMovement.disabled = true;
        plyCamMovement.disabled = true;

        // 둠 모드인 경우 삿건 숨기기
        if (gameMode == GameMode.Doom)
        {
            doomShotgunSprite.SetActive(false);
            doomShotgun.enabled = false;
        }

        Camera.main.transform.position = exitCamPos.position;
        Camera.main.transform.rotation = exitCamPos.rotation;
        yield return new WaitForSeconds(1f);
        exitLight.color = Color.green;
        AudioSource.PlayClipAtPoint(unlockSound, transform.position);
        yield return new WaitForSeconds(3f);
        Camera.main.transform.position = camOriginPos;
        Camera.main.transform.rotation = camOriginRot;

        plyMovement.disabled = false;
        plyCamMovement.disabled = false;

        // 둠 모드인 경우 삿건 다시 표시
        if (gameMode == GameMode.Doom)
        {
            doomShotgunSprite.SetActive(true);
            doomShotgun.enabled = true;
        }
    }

    // ---------------  Doom Mode  ---------------
    IEnumerator EnterDoomMode()
    {
        meltScreen.ShowScreen();

        yield return new WaitForSeconds(2.0f);

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
            if (eliminatedCount == 0 && EnemySpawner.enemiesOnMap == 0) return;

            if (!cutscenePlayed)
            {
                StartCoroutine("ShowExitArea");
                exitArea.EnableExitArea(); // 탈출구 오픈
            }
        }
    }

    // ---------------    Exit    ---------------
    public void CompleteGame()
    {
        clearUI.SetActive(true);
    }
}