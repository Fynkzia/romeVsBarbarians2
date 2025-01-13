Shader "Custom/CutoutSpecularWindTree"
{
    Properties
    {
        _AlbedoTex ("Albedo Texture", 2D) = "white" {}
        _AlbedoColor ("Albedo Color", Color) = (1, 1, 1, 1)
        _LightColor ("Light Color ", Color) = (1, 1, 1, 1)
        _CutoutThreshold ("Cutout Threshold", Range(0, 1)) = 0.5

        // Параметры для затенения
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 0.5
        _ShadowTransition ("Shadow Transition", Range(0, 1)) = 0.5

        // Параметры для ветра
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.1
        _WindSpeed ("Wind Speed", Range(0, 10)) = 1.0
        _WindFrequency ("Wind Frequency", Range(0, 10)) = 2.0
        _WindFrameStep ("Wind Frame Step", Range(0.01, 1.0)) = 0.2

        // Параметр для волны UV
        _UVWaveStrength ("UV Wave Strength", Range(0, 0.1)) = 0.02

        // Параметры для спекуляра
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularStrength ("Specular Strength", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType"="TransparentCutout" }
        Blend SrcAlpha OneMinusSrcAlpha
        AlphaToMask On
        ZWrite On
        ZTest LEqual
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Входные параметры
            sampler2D _AlbedoTex;
            fixed4 _AlbedoColor;
            fixed4 _LightColor;
            float _CutoutThreshold;

            // Параметры для затенения
            fixed4 _ShadowColor;
            float _ShadowIntensity;
            float _ShadowTransition;

            // Параметры для ветра
            float _WindStrength;
            float _WindSpeed;
            float _WindFrequency;
            float _WindFrameStep;

            // Параметры для волны UV
            float _UVWaveStrength;

            // Параметры для спекуляра
            float _Smoothness; // Объявляем параметр
            fixed4 _SpecularColor;
            float _SpecularStrength;

            struct appdata_t
            {
                float4 vertex : POSITION;
                half3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                half3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2; // Передаем позицию в мир. пространстве
            };

            v2f vert(appdata_t v)
            {
                v2f o;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Дискретизация времени для движения вершин
                float timeDiscrete = floor(_Time.y / _WindFrameStep) * _WindFrameStep;

                // Уникальный сдвиг для каждого меша
                float noise = sin(dot(worldPos.xz, float2(12.9898, 78.233))) * 43758.5453;
                noise = frac(noise);

                float windOffset = noise * 3.14159;
                float wind = _WindStrength * sin(_WindFrequency * (worldPos.x + timeDiscrete * _WindSpeed) + windOffset);
                v.vertex.y += wind * 0.2;

                // Дискретизация времени для волнения UV
                float uvTimeDiscrete = floor(_Time.y / _WindFrameStep) * _WindFrameStep;

                // Волнение UV
                float uvWave = _UVWaveStrength * sin(_WindFrequency * worldPos.x + uvTimeDiscrete * _WindSpeed);
                v.uv.x += uvWave;
                v.uv.y += uvWave;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                o.worldPos = worldPos;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                fixed4 albedo = tex2D(_AlbedoTex, i.uv) * _AlbedoColor;
                clip(albedo.a - _CutoutThreshold);

                half3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed4 lightColor = _LightColor;

                float dotProduct = max(0.0, dot(i.normal, lightDir));

                float shadowFactor = 1.0 - dotProduct;
                shadowFactor = smoothstep(0.0, _ShadowTransition, shadowFactor);
                shadowFactor *= _ShadowIntensity;

                fixed4 shadowedAlbedo = lerp(albedo, _ShadowColor, shadowFactor);

                half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                half3 halfVec = normalize(lightDir + viewDir);

                float spec = pow(max(0.0, dot(i.normal, halfVec)), _Smoothness * 128.0);
                fixed4 specular = _SpecularColor * _SpecularStrength * spec;

                return shadowedAlbedo * lightColor + specular;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
