Shader "Custom/AlphaCutoutWithShadowsGPUInstancing"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        _Color ("Tint", Color) = (1,1,1,1)
        _WhiteAmount ("White Flash Amount", Range(0,1)) = 0.0 // Интенсивность осветления
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            // Включаем поддержку инстансинга
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
  #pragma multi_compile_instancing // Включаем поддержку инстансинга
            #pragma instancing_options assumeuniformscaling
 #pragma multi_compile_fwdbase
            #pragma multi_compile_shadowcaster


            #include "UnityCG.cginc"

            // Параметры
            uniform float _Cutoff;
            uniform float4 _Color;
            sampler2D _MainTex;
            float _WhiteAmount; // Добавленный параметр для осветления

            // Вершинная функция
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;

   UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
UNITY_SETUP_INSTANCE_ID(v);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = _Color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // Фрагментная функция
            half4 frag(v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.uv);
                if (texColor.a < _Cutoff)
                    discard; // Альфа-катаут

                // Осветление (White Flash Effect)
                texColor.rgb = lerp(texColor.rgb, 1.0, _WhiteAmount);


                return texColor * i.color; // Применяем цветовую коррекцию
            }

            ENDCG
        }

        // Pass для теней
        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }
            CGPROGRAM
            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster
#pragma multi_compile_instancing // Включаем поддержку инстансинга
            #pragma instancing_options assumeuniformscaling
 #pragma multi_compile_fwdbase
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
   UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2fShadow
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Cutoff;

            v2fShadow vertShadowCaster(appdata_t v)
            {
                v2fShadow o;
UNITY_SETUP_INSTANCE_ID(v);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 fragShadowCaster(v2fShadow i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                clip(texColor.a - _Cutoff);
                return 0;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
