Shader "SOTN Custom/ScreenMeltUnlit" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MeltSpeed ("Melt Speed", Range (0,1)) = 0
		[HideInInspector] _Timer ("Timer", float) = 0
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
			float _Timer;
			float2 _Offset[256];

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

			// Vertex Shader
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			// Fragment Shader
			fixed4 frag (v2f i) : SV_Target
			{
				// Melt 효과 적용을 위해 y 좌표 조정
				i.uv.y += _Timer * _Offset[(int)floor(i.uv.x * 256.0f)].x;

				// 텍스처 샘플링 및 색상 적용
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;

				// 알파가 거의 없는 픽셀은 버리기
				if (col.a < 0.01) discard;

				return col;
			}
			ENDCG
		}
	}
}
