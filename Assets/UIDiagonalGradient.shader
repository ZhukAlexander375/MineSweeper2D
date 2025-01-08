Shader "Unlit/UIDiagonalGradient"
{
    Properties
    {
        _Color1 ("Color Top-Left", Color) = (1, 0, 0, 1) // Верхний левый цвет
        _Color2 ("Color Bottom-Right", Color) = (1, 1, 0, 1) // Нижний правый цвет
        _MainTex ("Sprite Texture", 2D) = "white" {} // Текстура спрайта
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color1;
            fixed4 _Color2;
            sampler2D _MainTex;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Получаем альфа-канал текстуры (для формы спрайта)
                fixed4 spriteColor = tex2D(_MainTex, i.uv);
                float alpha = spriteColor.a;

                // Вычисляем градиент по диагонали
                float gradient = (1.0 - i.uv.x + i.uv.y) * 0.5;

                // Интерполируем между цветами градиента
                fixed4 gradientColor = lerp(_Color2, _Color1, gradient);

                // Ограничиваем градиент формой спрайта через альфа-канал
                return fixed4(gradientColor.rgb, gradientColor.a * alpha);
            }
            ENDCG
        }
    }
}
