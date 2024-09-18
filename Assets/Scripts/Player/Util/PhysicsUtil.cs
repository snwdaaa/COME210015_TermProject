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
}
