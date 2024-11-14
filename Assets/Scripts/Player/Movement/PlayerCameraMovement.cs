using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 마우스를 사용한 카메라 조작 스크립트
/// </summary>
public class PlayerCameraMovement : MonoBehaviour
{
    // 컴포넌트
    private PlayerMouseInput playerMouseInput;
    private Camera playerFpCam; // 플레이어 1인칭 카메라

    [Header("카메라 설정")]
    [SerializeField] private GameObject camPivot; // 카메라 Pivot
    public static float mouseSensitivity = 1.0f; // 마우스 감도
    [SerializeField] private float mouseMaxAngleY = 85f;

    private void Start()
    {
        playerMouseInput = GetComponent<PlayerMouseInput>();
    }

    private void LateUpdate()
    {
        Look(playerMouseInput.mouseInput);
    }

    private void Look(Vector2 mouseInput)
    {
        // X축 입력 -> 플레이어 Y축에 대해 회전
        Vector3 playerRotateAngle = Vector3.up * mouseInput.x;
        transform.Rotate(playerRotateAngle * mouseSensitivity);

        // Y축 입력 -> 카메라 X축에 대해 회전    
        Vector3 currentCamRotation = camPivot.transform.localEulerAngles; // 현재 카메라 회전 각도
        if (currentCamRotation.x > 180) // 현재 회전 각도를 -180 ~ 180 사이로 변환
        {
            currentCamRotation.x -= 360;
        }

        float xAngle = currentCamRotation.x - Vector3.right.x * mouseInput.y * mouseSensitivity; // 계산할 회전 각도
        xAngle = Mathf.Clamp(xAngle, -mouseMaxAngleY, mouseMaxAngleY);
        camPivot.transform.localEulerAngles = new Vector3(xAngle, currentCamRotation.y, currentCamRotation.z);
    }
}
