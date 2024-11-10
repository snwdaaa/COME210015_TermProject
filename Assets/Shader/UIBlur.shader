Shader "Custom/MaskedBlurShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Blur("Blur Amount", Float) = 5
        _TintColor("Tint Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        GrabPass { "_GrabTexture" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float _Blur;
            fixed4 _TintColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float blur = _Blur;
                float2 uv = i.uv;
                fixed4 originalColor = tex2D(_MainTex, uv);

                if (originalColor.a < 0.01)
                    discard;

                fixed4 blurredColor = 0;
                float weightSum = 0;

                int blurRadius = 5;
                for (int x = -blurRadius; x <= blurRadius; x++)
                {
                    for (int y = -blurRadius; y <= blurRadius; y++)
                    {
                        float2 offset = float2(x, y) * blur * 0.001;
                        float distance = sqrt(x * x + y * y);
                        float weight = exp(-distance * distance / (2 * blur * blur));
                        blurredColor += tex2D(_GrabTexture, (i.grabPos.xy / i.grabPos.w) + offset) * weight;
                        weightSum += weight;
                    }
                }
                blurredColor /= weightSum;

                // Apply _TintColor to the final color and retain original alpha
                blurredColor.rgb *= _TintColor.rgb;
                blurredColor.a = originalColor.a * _TintColor.a;

                return blurredColor;
            }
            ENDCG
        }
    }
}
