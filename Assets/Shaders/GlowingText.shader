Shader "Custom/GlowingText"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {} // Текстура текста (обычно от TextMeshPro)
        _Color ("Text Color", Color) = (1, 1, 1, 1) // Основной цвет текста
        _GlowColor ("Glow Color", Color) = (1, 1, 0, 1) // Цвет свечения
        _GlowIntensity ("Glow Intensity", Float) = 1.0 // Интенсивность свечения
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" } // Для рендера поверх других объектов
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha // Прозрачность текста
        Cull Off
        Lighting Off
        ZWrite Off

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            float4 _GlowColor;
            float _GlowIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Основной текст
                fixed4 baseColor = tex2D(_MainTex, i.uv) * _Color;

                // Добавляем свечение (умножается на альфа-канал текста)
                fixed4 glow = _GlowColor * _GlowIntensity * baseColor.a;

                // Итоговый цвет (основной цвет + свечение)
                return baseColor + glow;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
