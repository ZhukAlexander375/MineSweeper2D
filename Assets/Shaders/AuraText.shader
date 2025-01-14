Shader "Custom/AuraText"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _TextColor ("Text Color", Color) = (1, 1, 1, 1)
        _GlowColor ("Glow Color", Color) = (0, 0.5, 1, 1)
        _GlowSize ("Glow Size", Float) = 1.0
        _GlowIntensity ("Glow Intensity", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _TextColor;
            float4 _GlowColor;
            float _GlowSize;
            float _GlowIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
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
                // Основной текст
                fixed4 baseColor = tex2D(_MainTex, i.uv) * _TextColor;

                // Свечение
                float glow = 0.0;
                for (int x = -4; x <= 4; x++)
                {
                    for (int y = -4; y <= 4; y++)
                    {
                        float2 offset = float2(x, y) * _GlowSize * 0.01;
                        glow += tex2D(_MainTex, i.uv + offset).a;
                    }
                }
                glow = glow / 81.0 * _GlowIntensity;

                // Итоговое смешение текста и свечения
                return baseColor + (glow * _GlowColor);
            }
            ENDCG
        }
    }
}
