Shader "Custom/NeonText"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}  // �������� ������ (������ �� TextMeshPro)
        _Color ("Text Color", Color) = (1, 1, 1, 1) // �������� ���� ������
        _GlowColor ("Glow Color", Color) = (0, 1, 1, 1) // ���� ��������
        _GlowIntensity ("Glow Intensity", Float) = 1.0 // ������������� ��������
        _GlowSize ("Glow Size", Float) = 2.0 // ������ ��������
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

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
            float4 _MainTex_ST;

            float4 _Color;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowSize;

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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // �������� �����
                fixed4 baseColor = tex2D(_MainTex, i.uv) * _Color;

                // ������ ��������: ��������� �����-����� ������ ������ � ������� ��������
                float2 offsets[8] = {
                    float2(-_GlowSize, 0), float2(_GlowSize, 0),  // ��������������
                    float2(0, -_GlowSize), float2(0, _GlowSize),  // ������������
                    float2(-_GlowSize, -_GlowSize), float2(_GlowSize, _GlowSize), // ���������
                    float2(-_GlowSize, _GlowSize), float2(_GlowSize, -_GlowSize)  // ���������
                };

                float glow = 0.0;
                for (int j = 0; j < 8; j++)
                {
                    glow += tex2D(_MainTex, i.uv + offsets[j] / _GlowSize).a;
                }
                glow = glow / 8.0 * _GlowIntensity;

                // ����: ����������� �������� ����� � ��������
                return baseColor + (glow * _GlowColor);
            }
            ENDCG
        }
    }
}
