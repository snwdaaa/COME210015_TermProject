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
    [SerializeField] private float damage = 50f;

    [Header("Layermask")]
    public LayerMask layerMask;


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
