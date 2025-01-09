Shader "Custom/PixelCompression"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTint ("Texture Color", Color) = (1, 1, 1, 1)
        _ColorDepth ("Color Compression", Range(2, 256)) = 16
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularIntensity ("Specular Intensity", Range(0, 1)) = 0.5
        _Shininess ("Shininess", Range(1, 128)) = 16
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _ColorTint;
            float _ColorDepth;
            float4 _SpecularColor;
            float _SpecularIntensity;
            float _Shininess;
            float4 _ShadowColor;
            float _ShadowIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Базовая текстура
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Применение цвета текстуры
                texColor *= _ColorTint;

                // Ограничение цветовой палитры
                texColor.rgb = floor(texColor.rgb * _ColorDepth) / _ColorDepth;

                // Освещение сцены
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float diffuse = max(0, dot(worldNormal, lightDir));

                // Спекуляр
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 reflectDir = reflect(-lightDir, worldNormal);
                float specular = pow(max(0, dot(viewDir, reflectDir)), _Shininess) * _SpecularIntensity;

                // Модификация цвета для теней
                fixed4 shadowColor = _ShadowColor * (1 - diffuse) * _ShadowIntensity;

                // Итоговый цвет
                fixed4 finalColor = texColor * diffuse + _SpecularColor * specular + shadowColor;

                return finalColor;
            }
            ENDCG
        }
    }
}
