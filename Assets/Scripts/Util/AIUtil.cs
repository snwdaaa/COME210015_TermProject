using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AIUtil : MonoBehaviour
{
    /// <summary>
    /// 시야각 내에 목표물이 있고, 다른 물체에 가려지지 않는지 확인
    /// </summary>
    /// <param name="target">확인할 목표</param>
    /// <param name="eyeTransform">시야 시작 위치</param>
    /// <param name="fov">시야각</param>
    /// <param name="distance">시야 거리</param>
    /// <param name="targetLayer">타겟의 레이어 마스크</param>
    /// <returns>시야각 내에 감지 여부</returns>
    public static bool IsTargetOnSight(Transform target, Transform eyeTransform, float fov, float distance, string targetTag)
    {
        Vector3 direction = target.position - eyeTransform.position; // 목표로 향하는 방향 벡터

        direction.y = eyeTransform.forward.y; // 시야각 내부에 있는지 검사하기 위해 잠시 높이 차이를 없앰
        // 만약 시야각 밖에 있다면 false 리턴
        if (Vector3.Angle(eyeTransform.forward, direction) > fov * 0.5f)
        {
            return false;
        }
        direction = target.position - eyeTransform.position; // 다시 계산해서 높이 차이 되돌림

        RaycastHit hit;
        if (Physics.Raycast(eyeTransform.position, direction, out hit, distance))
        {
            if (hit.transform.tag == targetTag) // 대상 태그를 발견하면 True
            {
                Debug.DrawRay(eyeTransform.position, direction, Color.red);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// NavMesh 위의 랜덤한 지점을 리턴
    /// </summary>
    /// <param name="center"></param>
    /// <param name="distance"></param>
    /// <param name="areaMask"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float radius, int areaMask)
    {
        Vector3 randomPos = Random.insideUnitSphere * radius + center;

        NavMeshHit hit; // NavMesh 샘플링 결과 저장

        // areaMask에 해당하는 NavMesh 중에서 randomPos에서 radius만큼 떨어진 곳 사이에서 가장 가까운 곳 찾음
        NavMesh.SamplePosition(randomPos, out hit, radius, areaMask);

        return hit.position;
    }
}