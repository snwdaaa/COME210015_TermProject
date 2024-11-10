using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoomShotgun : MonoBehaviour
{
    [Header("Components")]
    private AudioSource audioSource;

    [Header("Sprite")]
    public Image shotgunImage; // 샷건 이미지
    public Sprite[] shotgunImages; // 샷건 발사 스프라이트
    [SerializeField] private float spriteChangeDelay = 0.2f; // 스프라이트 교체 간격

    [Header("Attributes")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private float fireDelay;
    private float fireDelayTimer = 0f;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();     
    }

    private void Update()
    {
        Fire();
    }

    private void Fire()
    {
        if (CheckDelay() && Input.GetMouseButtonDown(0))
        {
            fireDelayTimer = 0f;
            audioSource.PlayOneShot(fireSound);
            StartCoroutine("PlayFireSprite");
        }
    }

    /// <summary>
    /// 발사 딜레이 체크
    /// </summary>
    private bool CheckDelay()
    {
        if (fireDelayTimer >= fireDelay)
        {
            return true;
        }
        else
        {
            fireDelayTimer += Time.deltaTime;
            return false;
        }
    }

    /// <summary>
    /// 발사 Sprite 재생
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayFireSprite()
    {
        shotgunImage.sprite = shotgunImages[1];
        yield return new WaitForSeconds(spriteChangeDelay);
        shotgunImage.sprite = shotgunImages[2];
        yield return new WaitForSeconds(spriteChangeDelay);
        shotgunImage.sprite = shotgunImages[3];
        yield return new WaitForSeconds(spriteChangeDelay);
        shotgunImage.sprite = shotgunImages[4];
        yield return new WaitForSeconds(spriteChangeDelay);
        shotgunImage.sprite = shotgunImages[5];
        yield return new WaitForSeconds(spriteChangeDelay);
        shotgunImage.sprite = shotgunImages[6];
        yield return new WaitForSeconds(spriteChangeDelay);
    }
}
