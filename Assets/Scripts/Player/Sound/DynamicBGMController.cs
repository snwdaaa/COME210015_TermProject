using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBGMController : MonoBehaviour
{
    public EnemyStateMachine esm;

    public AudioSource bgmSource;       // 기본 BGM AudioSource
    public AudioSource chaseSource;    // 긴급 상황 BGM AudioSource

    public AudioClip detectedSound;

    public float transitionSpeed = 2f; // 볼륨 전환 속도 (초 단위)

    public bool isChasing = false;    // 적에게 쫓기고 있는 상태
    public bool isDetectSoundPlayed = false;

    void Start()
    {
        esm = GameObject.FindWithTag("Enemy").GetComponent<EnemyStateMachine>();

        // 시작 시 기본 BGM 활성화
        bgmSource.volume = 1f;
        chaseSource.volume = 0f;

        bgmSource.Play();
        chaseSource.Play();
    }

    void Update()
    {
        if (GameManager.gameMode == GameManager.GameMode.Doom)
        {
            //if (bgmSource.enabled) bgmSource.enabled = false;
            if (chaseSource.enabled) bgmSource.enabled = false;
            return;
        }

        // 추적을 시작해야 공격을 할 수 있으므로 공격 상태도 추적 상태로 간주
        isChasing = esm.CurrentState == esm.chaseState || esm.CurrentState == esm.attackState;

        // 상태에 따라 볼륨을 전환
        if (isChasing)
        {       
            if (!isDetectSoundPlayed)
            {
                chaseSource.PlayOneShot(detectedSound);
                isDetectSoundPlayed = true;
            }
            bgmSource.volume = Mathf.Lerp(bgmSource.volume, 0f, Time.deltaTime * transitionSpeed);
            chaseSource.volume = Mathf.Lerp(chaseSource.volume, 0.05f, Time.deltaTime * transitionSpeed);
        }
        else
        {
            if (isDetectSoundPlayed)
            {
                isDetectSoundPlayed = false;
            }

            bgmSource.volume = Mathf.Lerp(bgmSource.volume, 0.05f, Time.deltaTime * transitionSpeed);
            chaseSource.volume = Mathf.Lerp(chaseSource.volume, 0f, Time.deltaTime * transitionSpeed);
        }
    }
}

