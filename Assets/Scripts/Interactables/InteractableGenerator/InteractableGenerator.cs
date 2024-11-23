using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableGenerator : MonoBehaviour
{
    // 컴포넌트
    private CircleQTEUI circleQTEUI;
    private AudioSource audioSource;
    [SerializeField] private AudioSource generatorSound;
    private PlayerKeyInput keyInput;
    private PlayerMovement playerMovement;
    private PlayerCameraMovement cameraMovement;
    private GeneratorSpawner spawner;
    private UIManager uiManager;

    [Header("사운드")]
    [SerializeField] private AudioClip generatorSound_Start;
    [SerializeField] private AudioClip generatorSound_Working;
    [SerializeField] private AudioClip qteSound_Fail;
    [SerializeField] private AudioClip qteSound_Normal;
    [SerializeField] private AudioClip qteSound_Success;

    [Header("UI")]
    private Slider progressBar;

    [Header("발전기 속성")]
    [SerializeField] private bool isPlayerOperating = false;
    [SerializeField] private bool isRepaired = false;
    private bool isStarted = false;

    [Header("진행도 설정")]
    [SerializeField] private float currentProgress = 0f; // 현재 진행도
    private float completeProgress = 100f; // 최대 진행도
    [SerializeField] private float progressionPerSec = 2f; // 초당 증가할 진행도
    [SerializeField] private float decreasePerSec = 1f; // 아무 것도 진행하지 않을 때 초당 감소할 진행도
    [SerializeField] private float failDecrease = 20f; // 빨간 구역 감소량
    [SerializeField] private float normalDecrease = 5f; // 노란 구역 감소량
    [SerializeField] private float successIncrease = 10f; // 초록 구역 증가량

    [Header("필요 오브젝트")]
    private GameObject qteUIObject; // CircleQTEUI 컴포넌트를 가지는 오브젝트
    private GameObject progressUIObject;

    private float neededTime; // 현재 진행도로부터 끝날 때까지 걸리는 시간
    private float currentTime = 0;
    private float nextTime;
    private List<int> randomTimes;

    // Start is called before the first frame update
    void Start()
    {
        GameObject managers = GameObject.Find("Managers");
        spawner = managers.GetComponent<GeneratorSpawner>();
        audioSource = GetComponent<AudioSource>();
        uiManager = managers.GetComponent<UIManager>();

        progressBar = spawner.progressBar;
        qteUIObject = spawner.qteUIObject;
        progressUIObject = spawner.progressUIObject;

        circleQTEUI = qteUIObject.GetComponent<CircleQTEUI>();

        SubscribeEvent();
    }

    // Update is called once per frame
    void Update()
    {
        ProgressionAutoDecrease();
    }

    private void SubscribeEvent()
    {
        circleQTEUI.OnQTEFail += () =>
        {
            this.OnQTEFail(this.gameObject);
        };

        circleQTEUI.OnQTENormal += () =>
        {
            this.OnQTENormal(this.gameObject);
        };

        circleQTEUI.OnQTESuccess += () =>
        {
            this.OnQTESuccess(this.gameObject);
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            uiManager.keyNotifierUI.Show("E", "발전기 수리");
        }        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isRepaired)
        {
            if (other.gameObject.tag == "Player")
            {
                keyInput = other.gameObject.GetComponent<PlayerKeyInput>();
                playerMovement = other.gameObject.GetComponent<PlayerMovement>();
                cameraMovement = other.gameObject.GetComponent<PlayerCameraMovement>();

                if (!isPlayerOperating)
                {
                    if (keyInput.keyPressed_Use)
                    {
                        EnterFixing();

                        uiManager.keyNotifierUI.Hide(); // 키 도움말 숨기기
                        StartCoroutine("PlayGeneratorSound"); // 사운드 재생
                        ToggleProgressUI(); // 진행도 UI 표시
                        InitQuickTimeEvent(); // QTE 시작 전 설정
                    }
                }
                else
                {
                    if (keyInput.keyPressed_Use)
                    {
                        // QTE 도중에 취소하는 경우 UI 숨기고 실패 처리
                        if (qteUIObject.activeInHierarchy)
                        {
                            OnQTEFail(this.gameObject);
                        }

                        ExitFixing();
                        ToggleProgressUI(); // 진행도 UI 표시

                        uiManager.keyNotifierUI.Show("E", "발전기 수리");

                    }
                    else
                    {
                        FixGenerator(); // 발전기 수리
                        QuickTimeEvent(); // QTE 이벤트 실행
                    }
                }
            }
        }
        else
        {
            uiManager.keyNotifierUI.Hide(); // 키 도움말 숨기기
        }
    }

    private void OnTriggerExit(Collider other)
    {
        uiManager.keyNotifierUI.Hide(); // 키 도움말 숨기기
    }

    /// <summary>
    /// 진행도 UI를 끄고 켬
    /// </summary>
    private void ToggleProgressUI()
    {
        if (progressUIObject.activeInHierarchy)
        {
            progressUIObject.SetActive(false);
        }
        else
        {
            progressUIObject.SetActive(true);
        }
    }

    private IEnumerator PlayGeneratorSound()
    {
        if (!isStarted || currentProgress <= 0f)
        {
            isStarted = true;
            generatorSound.PlayOneShot(generatorSound_Start);
            yield return new WaitForSeconds(generatorSound_Start.length);
            generatorSound.clip = generatorSound_Working;
            generatorSound.loop = true;
            generatorSound.Play();
        }
    }

    private void InitQuickTimeEvent()
    {    
        neededTime = (completeProgress - currentProgress) / progressionPerSec; // 끝날 때까지 걸리는 시간 계산
        randomTimes = GetRandomQTEStartTimes(1f, neededTime, Mathf.Clamp((int)(neededTime / 5f), 1, 6)); // 0초 ~ neededTime까지 QTE를 시작할 랜덤한 시간을 결정
        nextTime = randomTimes[0];
        currentTime = 0;

        // 무작위로 뽑은 시간을 출력
        foreach (float time in randomTimes)
        {
            Debug.Log(time);
        }
    }

    /// <summary>
    /// 랜덤으로 뽑힌 시간이 되면 QTE 실행
    /// </summary>
    private void QuickTimeEvent()
    {
        // 처음 시작했거나 랜덤 시간이 없는 경우
        if (!isStarted || randomTimes.Count <= 0)
        {
            InitQuickTimeEvent();
        }

        if (currentTime <= neededTime)
        {
            if (!qteUIObject.activeInHierarchy)
            {
                currentTime += Time.deltaTime;

                if ((int)currentTime == nextTime)
                {
                    nextTime = randomTimes[0]; // 다음 QTE때 사용할 시간 업데이트
                    randomTimes.RemoveAt(0);
                    circleQTEUI.ToggleQTEUI(); // QTE UI 표시             
                }
            }
        }
    }

    /// <summary>
    /// startTime부터 finishTime까지 count개의 랜덤한 시점을 뽑음
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="finishTime"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<int> GetRandomQTEStartTimes(float startTime, float finishTime, int count)
    {
        List<int> times = new List<int>();

        for (int i = 0; i < count; i++)
        {
            int randomTime = UnityEngine.Random.Range((int)startTime, (int)finishTime);

            if (!times.Contains(randomTime))
            {
                times.Add(randomTime);
            }
        }

        times.Sort(); // 시간 순으로 정렬

        return times;
    }

    private void EnterFixing()
    {
        keyInput.keyPressed_Use = false;
        playerMovement.DisableMovement();
        cameraMovement.enabled = false;
        isPlayerOperating = true;
        CircleQTEUI.currentGenerator = this.gameObject;
    }

    private void ExitFixing()
    {
        keyInput.keyPressed_Use = false;
        playerMovement.EnableMovement();
        cameraMovement.enabled = true;
        isPlayerOperating = false;
        CircleQTEUI.currentGenerator = null;
    }

    /// <summary>
    /// 발전기 수리
    /// </summary>
    private void FixGenerator()
    {
        if (!isRepaired)
        {
            if (currentProgress < completeProgress)
            {
                if (!qteUIObject.activeInHierarchy)
                {
                    currentProgress += progressionPerSec * Time.deltaTime;
                    progressBar.value = currentProgress;
                }
            }
            else // 수리가 끝났을 때
            {
                isRepaired = true;
                ToggleProgressUI();
                ExitFixing();

                GameManager.repairedGeneratorCount++; // 게임 매니저 수리한 발전기 개수 증가
            }
        }
    }

    private void ProgressionAutoDecrease()
    {
        if (!isRepaired && isStarted && !isPlayerOperating)
        {
            if (currentProgress <= 0)
            {
                generatorSound.Stop();
            }

            currentProgress -= decreasePerSec * Time.deltaTime;
            currentProgress = Mathf.Clamp(currentProgress, 0, completeProgress);
        }
    }

        /// <summary>
        /// QTE 실패 시 호출되는 함수
        /// </summary>
        private void OnQTEFail(GameObject target)
        {
            if (target != CircleQTEUI.currentGenerator) return; // Invoke로 Broadcast된 메시지가 자신의 것이 아니면 무시

            currentProgress -= failDecrease;
            currentProgress = Mathf.Clamp(currentProgress, 0, completeProgress);

            audioSource.PlayOneShot(qteSound_Fail);
        }

        /// <summary>
        /// QTE 보통 성공 시 호출되는 함수
        /// </summary>
        private void OnQTENormal(GameObject target)
        {
            if (target != CircleQTEUI.currentGenerator) return; // Invoke로 Broadcast된 메시지가 자신의 것이 아니면 무시

            currentProgress -= normalDecrease;
            currentProgress = Mathf.Clamp(currentProgress, 0, completeProgress);

            audioSource.PlayOneShot(qteSound_Normal);
        }

        /// <summary>
        /// QTE 성공 시 호출되는 함수
        /// </summary>
        private void OnQTESuccess(GameObject target)
        {
            if (target != CircleQTEUI.currentGenerator) return; // Invoke로 Broadcast된 메시지가 자신의 것이 아니면 무시

            currentProgress += successIncrease;
            currentProgress = Mathf.Clamp(currentProgress, 0, completeProgress);

            audioSource.PlayOneShot(qteSound_Success);
        }
}