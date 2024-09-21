using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtil : MonoBehaviour
{
    /// <summary>
    /// 물체 위쪽에 충분한 공간이 있는지 확인하는 메서드
    /// </summary>
    /// <param name="startPos">Ray를 발사할 위치</param>
    /// <param name="needLength">필요한 공간(길이)</param>
    /// <returns>충분한 경우 True, 충분하지 않은 경우 False 리턴</returns>
    public static bool CheckUpperSpace(Vector3 startPos, float needLength)
    {
        RaycastHit hit;
        Debug.DrawRay(startPos, Vector3.up * needLength, Color.green);

        if (Physics.Raycast(startPos, Vector3.up, out hit, needLength))
        {
            // Ray의 이동 거리와 필요한 거리의 차이가 매우 적으면 같은 것으로 생각
            if (hit.distance - needLength < 0.01f)
                return true;           
        }

        return false;
    }

    /// <summary>
    /// Character Controller 컴포넌트를 가지는 오브젝트가 경사면 위에 있는지 확인
    /// </summary>
    /// <param name="obj">Character Controller 게임 오브젝트</param>
    /// <param name="slopeHit">경사면 정보를 담을 RaycastHit 레퍼런스</param>
    /// <returns>경사면에서 위에 있는지 여부 리턴</returns>
    public static bool IsOnSlope(GameObject obj, ref RaycastHit slopeHit)
    {
        CharacterController characterController = obj.GetComponent<CharacterController>();

        if (characterController.isGrounded)
        {
            if (Physics.Raycast(obj.transform.position, Vector3.down * 1.5f, out slopeHit))
            {
                float slopeAngle = Vector3.Angle(Vector3.Normalize(slopeHit.normal), Vector3.up); // 경사면과의 각도 = 경사면의 노멀 벡터와 Vector3.up 사이의 각도
                if (slopeAngle > 0f)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Character Controller 컴포넌트를 가지는 오브젝트가 올라가있는 경사면의 각도가 캐릭터 컨트롤러의 Slope Limit보다 큰지 확인
    /// </summary>
    /// <param name="obj">Character Controller 게임 오브젝트</param>
    /// <param name="slopeHit">경사면 정보를 담을 RaycastHit 레퍼런스</param>
    /// <returns>Slope Limit보다 가파른 곳에 있는지 여부</returns>
    public static bool IsOnSteepSlope(GameObject obj, ref RaycastHit slopeHit)
    {
        CharacterController characterController = obj.GetComponent<CharacterController>();
        if (characterController.isGrounded)
        {
            if (Physics.Raycast(obj.transform.position, Vector3.down * 1.5f, out slopeHit))
            {
                float slopeAngle = Vector3.Angle(Vector3.Normalize(slopeHit.normal), Vector3.up); // 경사면과의 각도 = 경사면의 노멀 벡터와 Vector3.up 사이의 각도
                if (slopeAngle > characterController.slopeLimit)
                    return true;
            }
        }

        return false;
    }
}
