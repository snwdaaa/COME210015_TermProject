Shader "SOTN Custom/ScreenMeltUnlit" 
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MeltSpeed ("Melt Speed", Range (0,1)) = 0
        _Timer ("Timer", float) = 0
        _OffsetTex ("Offset Texture", 2D) = "white" {} // 오프셋 텍스처
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            fixed4 _Color;
            sampler2D _MainTex;
            sampler2D _OffsetTex; // 오프셋 텍스처
            float _Timer;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 텍스처 UV에서 오프셋 샘플링
                float index = i.uv.x; 
                float offset = tex2D(_OffsetTex, float2(index, 0)).r;

                // 오프셋 값을 UV Y축에 적용
                i.uv.y += clamp(_Timer, 0.0, 1.0) * offset;

                // 텍스처 샘플링 및 알파 값 처리
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                if (col.a < 0.01) discard;

                return col;
            }
            ENDCG
        }
    }
}
