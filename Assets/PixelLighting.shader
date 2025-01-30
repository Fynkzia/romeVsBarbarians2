Shader "Custom/PixelLighting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        _PixelSize ("Pixel Size", Range(1, 32)) = 8
        _LightSteps ("Light Steps", Range(2, 32)) = 4
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _PixelSize;
            float _LightSteps;

// unity defined variables
			uniform float4 _LightColor0;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                float4 texColor = tex2D(_MainTex, i.uv) * _Color;

                // Calculate pixelated screen coordinates
                float2 pixelatedPos = floor(i.pos.xy / _PixelSize) * _PixelSize;

                // Calculate lighting
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;

                // Calculate diffuse lighting using pixelated positions
               float diffuse = pow(max(dot(normal, lightDir), 0.5f), 1.1f);

                // Quantize diffuse lighting to light steps
                //diffuse = floor(diffuse * _LightSteps) / _LightSteps;

                // Apply pixelation to lighting
                diffuse -= step(dot(normal, lightDir), frac(pixelatedPos.x * pixelatedPos.y));

                // Combine texture color with pixelated lighting
                float3 finalColor = texColor.rgb * lightColor * diffuse;

                return float4(finalColor, texColor.a);
            }
            ENDCG
        }
    }
}
