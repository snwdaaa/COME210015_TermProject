using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CircleQTEUI : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] private PlayerMouseInput mouseInput;
    private AudioSource audioSource;

    [Header("필요 오브젝트")]
    [SerializeField] private RectTransform circleRectTransform; // 원
    [SerializeField] private RectTransform cursorRectTransform; // 커서
    [SerializeField] private RectTransform qteRangeTransform; // QTE 범위
    [SerializeField] private RectTransform successRectTransform; // 성공 범위
    [SerializeField] private RectTransform normalRectTransform; // 일반 범위

    [Header("회전 속성")]
    [SerializeField] private float speed = 50f;        // 각속도
    [SerializeField] private float radiusDelta = 0.05f; // 원의 중심에서부터 떨어진 커서의 위치를 조절하는 보정값

    private RectTransform rectTransform;
    private float currentAngle = 90f; // 현재 각도
    private float currentAngle_upper;


    // 이벤트
    public event Action OnQTEFail;
    public event Action OnQTENormal;
    public event Action OnQTESuccess;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();

        currentAngle = 90f; // 현재 각도

        SetRandomQTERange();
        audioSource.Play();
    }

    private void Update()
    {
        currentAngle_upper = currentAngle - 90f;
        currentAngle_upper = NormalizeAngle(currentAngle_upper);

        MoveCursorOnCircleEdge();

        if (mouseInput.mouseButtonPressed_LeftButton)
        {
            CheckCursorCurrentRange();
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360;

        if (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }

    /// <summary>
    /// 커서를 원의 경계면 위에서 움직임
    /// </summary>
    private void MoveCursorOnCircleEdge()
    {
        // 각도 업데이트 (시간에 따라 각도 변화)
        currentAngle += -speed * Time.deltaTime;
        currentAngle = NormalizeAngle(currentAngle);

        // 원의 중심과 반지름 계산
        Vector2 circleCenter = circleRectTransform.anchoredPosition;
        float radius = (circleRectTransform.rect.width / 2f) - radiusDelta;

        // 각도에 따라 x, y 좌표 계산 (극좌표 -> 직교좌표 변환)
        float radian = currentAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radian) * radius;
        float y = Mathf.Sin(radian) * radius;

        // 이미지의 위치를 원 경계에 맞춰 업데이트
        cursorRectTransform.anchoredPosition = circleCenter + new Vector2(x, y);
        cursorRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle + 90f));
    }

    /// <summary>
    /// 시작 위치에서 좀 각을 두고, 랜덤한 각도에 QTE 구간 추가
    /// </summary>
    private void SetRandomQTERange()
    {
        float randomAngle = UnityEngine.Random.Range(0f, 180f);
        qteRangeTransform.rotation = Quaternion.Euler(0, 0, NormalizeAngle(randomAngle));
    }

    /// <summary>
    /// 현재 커서가 어느 구간 위에 있는 지 확인 후 이벤트 실행
    /// </summary>
    public void CheckCursorCurrentRange()
    {
        float mid = NormalizeAngle(qteRangeTransform.rotation.eulerAngles.z); // success와 normal 사이 각
        float end = NormalizeAngle(mid - (successRectTransform.gameObject.GetComponent<Image>().fillAmount * 360)); // success의 마지막
        float start = NormalizeAngle(mid + (normalRectTransform.gameObject.GetComponent<Image>().fillAmount * 360)); // normal의 시작

        Debug.Log(start + " " + mid + " " + end + " " + currentAngle_upper);

        // 체크 범위 안에 들어올 때
        if (start >= currentAngle_upper && currentAngle_upper >= end)
        {
            if (currentAngle_upper <= mid)
            {
                OnQTESuccess?.Invoke();
            }
            else
            {
                OnQTENormal?.Invoke();
            }
        }
        else
        {
            OnQTEFail?.Invoke();
        }

    }
}
