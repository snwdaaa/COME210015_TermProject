Shader "Custom/DoubleSidedShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _NormalMap ("Normal Map", 2D) = "bump" {} // 노멀 맵 추가
        _OcclusionMap ("Ambient Occlusion Map", 2D) = "white" {} // AO 맵 추가
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off // 양면 렌더링
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap; // 노멀 맵 샘플러
        sampler2D _OcclusionMap; // AO 맵 샘플러

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_OcclusionMap;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // Normal Map
            fixed3 normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));

            // Double-sided lighting: Flip normals if back-facing
            o.Normal = -normal;

            // AO Map
            float ao = tex2D(_OcclusionMap, IN.uv_OcclusionMap).r; // AO 값을 가져옴
            o.Occlusion = ao;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
