using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 Bobbing 스크립트
/// </summary>
public class HeadBobbing : MonoBehaviour
{
    private PlayerStateMachine stateMachine;

    [Header("카메라 움직임 (Walk)")]
    [Range(0.001f, 1.0f)] public float walkAmount = 0.1f;
    [Range(1f, 30f)] public float walkFrequency = 7.0f;

    [Header("카메라 움직임 (Sprint)")]
    [Range(0.001f, 1.0f)] public float sprintAmount = 0.14f;
    [Range(1f, 30f)] public float sprintFrequency = 14.0f;

    [Header("카메라 움직임 (Crouch Walk)")]
    [Range(0.001f, 1.0f)] public float crouchWalkAmount = 0.1f;
    [Range(1f, 30f)] public float crouchWalkFrequency = 4.0f;

    [Header("기타 설정")]
    [Range(10f, 100f)] public float smoothness = 10.0f;

    private Vector3 pivotOriginPos;

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = GetComponentInParent<PlayerStateMachine>(); // CamPivot의 부모인 Player에서 PlayerStateMachine을 가져옴

        pivotOriginPos = transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckBobbingCondition();
        ResetCamPivotPosition();
    }

    private void CheckBobbingCondition()
    {
        bool isWalking = stateMachine.CurrentMoveState == stateMachine.walkState;
        bool isSprinting = stateMachine.CurrentMoveState == stateMachine.sprintState;
        bool isCrouchWalking = stateMachine.CurrentMoveState == stateMachine.crouchWalkState;

        if (isWalking)
        {
            DoHeadBobbing(walkFrequency, walkAmount, smoothness);
        }
        else if (isSprinting)
        {
            DoHeadBobbing(sprintFrequency, sprintAmount, smoothness);
        }
        else if (isCrouchWalking)
        {
            DoHeadBobbing(crouchWalkFrequency, crouchWalkAmount, smoothness);
        }
    }

    private void DoHeadBobbing(float frequency, float amount, float smoothness)
    {
        Vector3 targetPos = Vector3.zero;

        targetPos.y += Mathf.Lerp(targetPos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smoothness * Time.deltaTime);
        // targetPos.x += Mathf.Lerp(targetPos.y, Mathf.Cos(Time.time * frequency) * amount * 1.6f, smoothness * Time.deltaTime);
        transform.localPosition = targetPos;
    }

    private void ResetCamPivotPosition()
    {
        if (transform.localPosition == pivotOriginPos) return;

        transform.localPosition = Vector3.Lerp(transform.localPosition, pivotOriginPos, 1 * Time.deltaTime);
    }
}
