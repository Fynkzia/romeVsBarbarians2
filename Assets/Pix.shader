Shader "Custom/Pix"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                
                // Преобразуем вершины в экранное пространство
                float4 screenPos = UnityObjectToClipPos(v.vertex);
                
                // Пикселизация экранных координат
                screenPos.xy = floor(screenPos.xy / _PixelSize) * _PixelSize;

                o.vertex = screenPos;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Семплируем текстуру без изменений
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}

