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
    private Vector2 originPos;

    [Header("Effect")]
    public Transform hitEffect;

    [Header("Attributes")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private float fireDelay;
    private float fireDelayTimer = 0f;
    [SerializeField] private float damage = 50f;

    [Header("Layermask")]
    public LayerMask layerMask;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        originPos = shotgunImage.rectTransform.anchoredPosition;
    }

    private void Update()
    {
        Fire();
    }

    private void Fire()
    {
        if (GameManager.gameMode == GameManager.GameMode.Doom && CheckDelay() && Input.GetMouseButtonDown(0))
        {
            fireDelayTimer = 0f;
            audioSource.PlayOneShot(fireSound);
            StartCoroutine("PlayFireSprite");

            AttackTarget();
        }
    }

    /// <summary>
    /// 맞은 오브젝트 확인 후 대미지 적용
    /// </summary>
    private void AttackTarget()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // 화면 정중앙 ray 발사

        if (Physics.Raycast(ray, out hit, 1000f, layerMask))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            Transform effect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            //Destroy(effect.gameObject, 3f);
            enemy.ApplyDamage(damage);
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
        // 앞 방향 시야 확보 위해서 왼쪽으로 살짝 옮김
        MoveShotgunSpriteLeft();
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
        // 다시 원래 위치로 되돌림
        ResetShotgunSpritePos();
        shotgunImage.sprite = shotgunImages[6];
        yield return new WaitForSeconds(spriteChangeDelay);

    }

    private void MoveShotgunSpriteLeft()
    {
        // 기존 anchoredPosition 유지, PosX만 - 1/4 지점으로 이동
        Vector2 newPosition = shotgunImage.rectTransform.anchoredPosition;
        newPosition.x = -250.0f;

        // 새로운 위치로 설정
        shotgunImage.rectTransform.anchoredPosition = newPosition;
    }

    private void ResetShotgunSpritePos()
    {
        shotgunImage.rectTransform.anchoredPosition = originPos;
    }
}
