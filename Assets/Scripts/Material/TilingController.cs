using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나의 Material로 여러 Mesh에 각각 다른 tiling과 offset을 적용하는 스크립트
/// Play Mode와 Edit Mode 둘 다 동작
/// </summary>
[ExecuteAlways]
public class TilingController : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    public Vector2 tiling = new Vector2(1, 1);
    public Vector2 offset = new Vector2(0, 0);

    /// <summary>
    /// 타일링, 오프셋 값 업데이트
    /// </summary>
    private void UpdateMaterial()
    {
        if (meshRenderer == null) return;

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        // 타일링, 오프셋 값 설정
        propertyBlock.SetVector("_MainTex_ST", new Vector4(tiling.x, tiling.y, offset.x, offset.y));

        // 메시 렌더러에 적용
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    /// <summary>
    /// 타일링, 오프셋 값 적용
    /// </summary>
    private void Awake()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void Update()
    {
        if (meshRenderer == null) return;

        UpdateMaterial();
    }

    /// <summary>
    /// 유니티 에디터 기본 값 설정
    /// </summary>
    private void Reset()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void OnValidate()
    {
        if (meshRenderer == null) return;

        UpdateMaterial();
    }
}
