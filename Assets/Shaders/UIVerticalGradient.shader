Shader "Custom/UIVerticalGradient"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {} // Текстура объекта
        _TopColor ("Top Color", Color) = (1, 1, 1, 1) // Цвет вверху
        _BottomColor ("Bottom Color", Color) = (0, 0, 0, 1) // Цвет внизу
        
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
            float4 _TopColor;
            float4 _BottomColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 worldUV : TEXCOORD1; // Для расчета градиента
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // Преобразование координат для определения позиции в мире
                o.worldUV = UnityObjectToClipPos(v.vertex).xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Основной цвет текстуры
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Расчет градиента: нормализация высоты
                float gradientFactor = saturate(i.worldUV.y);

                // Интерполяция между верхним и нижним цветом
                fixed4 gradientColor = lerp(_TopColor, _BottomColor, gradientFactor);

                // Итоговое смешение текстуры и градиента
                return texColor * gradientColor;
            }
            ENDCG
        }
    }
}
