using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoomHUD : MonoBehaviour
{
    // 얼굴 이미지를 표시하는 UI 이미지
    public Image faceImage;

    // 플레이어 얼굴 애니메이션 스프라이트들
    public Sprite[] faceExpressions;

    // 남은 적 수 표시하는 텍스트
    public Text enemyCountText;

    // 컴포넌트
    private GameManager gameManager;

    void Start()
    {
        StartCoroutine("UpdateHUD");

        gameManager = GameObject.Find("Managers").GetComponent<GameManager>();
    }

    private void OnGUI()
    {
        enemyCountText.text = "남은 적: " + (EnemySpawner.enemiesOnMap - gameManager.eliminatedCount).ToString();
    }

    IEnumerator UpdateHUD()
    {
        while (true)
        {
            // 얼굴 표정 업데이트 (예시: 체력 상태에 따라 다른 표정을 표시)
            faceImage.sprite = faceExpressions[0];
            yield return new WaitForSeconds(1f);
            faceImage.sprite = faceExpressions[1];
            yield return new WaitForSeconds(1f);
            faceImage.sprite = faceExpressions[2];
            yield return new WaitForSeconds(1f);
        }
    }
}
