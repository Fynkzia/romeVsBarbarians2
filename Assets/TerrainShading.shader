Shader "Custom/TerrainShadingWithNormals"
{
    Properties
    {
        // Control map
        _Control ("Control (Splatmap)", 2D) = "white" {}
// Global smoothness and metallic
       


        // Layer 0
        _Splat0 ("Splat 0 (Base Texture)", 2D) = "white" {}
        _Splat0Normal ("Splat 0 Normal Map", 2D) = "bump" {}
        _Splat0Tiling ("Splat 0 Tiling", Vector) = (10, 10, 0, 0)
        _Splat0Albedo ("Splat 0 Albedo Color", Color) = (1, 1, 1, 1)
       

        // Layer 1
        _Splat1 ("Splat 1", 2D) = "white" {}
        _Splat1Normal ("Splat 1 Normal Map", 2D) = "bump" {}
        _Splat1Tiling ("Splat 1 Tiling", Vector) = (10, 10, 0, 0)
        _Splat1Albedo ("Splat 1 Albedo Color", Color) = (1, 1, 1, 1)
     

        // Layer 2
        _Splat2 ("Splat 2", 2D) = "white" {}
        _Splat2Normal ("Splat 2 Normal Map", 2D) = "bump" {}
        _Splat2Tiling ("Splat 2 Tiling", Vector) = (10, 10, 0, 0)
        _Splat2Albedo ("Splat 2 Albedo Color", Color) = (1, 1, 1, 1)


        // Layer 3
        _Splat3 ("Splat 3", 2D) = "white" {}
        _Splat3Normal ("Splat 3 Normal Map", 2D) = "bump" {}
        _Splat3Tiling ("Splat 3 Tiling", Vector) = (10, 10, 0, 0)
        _Splat3Albedo ("Splat 3 Albedo Color", Color) = (1, 1, 1, 1)
      

        // Shadow settings
        _ShadowTintColor ("Shadow Tint Color", Color) = (0, 0, 0, 1)
        _ShadowStrength ("Shadow Strength", Range(0, 1)) = 0.5
        _ShadowTransition ("Shadow Transition", Range(0.1, 2)) = 1.0

        // Height-based brightness and darkening
        _HeightBrightnessScale ("Height Brightness Scale", Range(0, 1)) = 0.1
        _MaxHeight ("Max Height for Brightness", Float) = 50.0
        _MinHeight ("Min Height for Darkening", Float) = 10.0
        _HeightDarknessScale ("Height Darkness Scale", Range(0, 1)) = 0.2

        // Custom colors for min/max height
        _LowColor ("Low Altitude Color", Color) = (0.1, 0.1, 0.3, 1.0)
        _HighColor ("High Altitude Color", Color) = (1.0, 0.8, 0.6, 1.0)

// Custom colors for light
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5

        _LightColor ("Light Color", Color)= (1.0,1.0,1.0)
        _SpecColor("Spec Color", Color) = (1.0,1.0,1.0)
        _Shininess("Shininess", Float) = 10
        _SpecularPower(" Specular Power", Float) = 0.1
        
    }

    SubShader
    {
        Tags { "Queue" = "Geometry" "LightMode"="ForwardBase"  }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog // Включение поддержки тумана
            #pragma multi_compile_fwdbase
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float2 uv : TEXCOORD0;
               

                  UNITY_FOG_COORDS(3)
                  UNITY_LIGHTING_COORDS(4,5)
            };

            // Control map
            sampler2D _Control;

 // Global smoothness and metallic
            float _Metallic;
            float _Smoothness; 

            // Layer 0
            sampler2D _Splat0;
            sampler2D _Splat0Normal;
            float4 _Splat0Tiling;
            float4 _Splat0Albedo;


            // Layer 1
            sampler2D _Splat1;
            sampler2D _Splat1Normal;
            float4 _Splat1Tiling;
            float4 _Splat1Albedo;


// Layer 2
            sampler2D _Splat2;
            sampler2D _Splat2Normal;
            float4 _Splat2Tiling;
            float4 _Splat2Albedo;


            // Layer 3
            sampler2D _Splat3;
            sampler2D _Splat3Normal;
            float4 _Splat3Tiling;
            float4 _Splat3Albedo;


            // Shadow and height settings
            float4 _ShadowTintColor;
            float _ShadowStrength;
            float _ShadowTransition;
            float _HeightBrightnessScale;
            float _MaxHeight;
            float _MinHeight;
            float _HeightDarknessScale;

            // Custom colors
            float4 _LowColor;
            float4 _HighColor;

	        uniform float4 _LightColor;
            uniform float4 _SpecColor;
			uniform float _Shininess;
            uniform float _SpecularPower;

// unity defined variables
			uniform float4 _LightColor0;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject));


                

               
                o.uv = v.uv;

                UNITY_TRANSFER_FOG(o, o.vertex);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Read control map
                fixed4 control = tex2D(_Control, i.uv);

                // Read and apply tiling for textures
                fixed4 tex0 = tex2D(_Splat0, i.uv * _Splat0Tiling.xy) * _Splat0Albedo;
                fixed4 tex1 = tex2D(_Splat1, i.uv * _Splat1Tiling.xy) * _Splat1Albedo;
                fixed4 tex2 = tex2D(_Splat2, i.uv * _Splat2Tiling.xy) * _Splat2Albedo;
                fixed4 tex3 = tex2D(_Splat3, i.uv * _Splat3Tiling.xy) * _Splat3Albedo;




                // Blend textures based on control map
               fixed4 terrainColor = tex0 * control.r + tex1 * control.g + tex2 * control.b + tex3 * control.a;


                // Apply height-based brightness
                float heightFactor = saturate(i.worldPos.y / _MaxHeight);
                float brightness = heightFactor * _HeightBrightnessScale;

                // Apply height-based darkening
                float lowFactor = saturate((_MinHeight - i.worldPos.y) / _MinHeight);
                float darkness = lowFactor * _HeightDarknessScale;

                // Interpolate between low and high colors
                float altitudeFactor = saturate((i.worldPos.y - _MinHeight) / (_MaxHeight - _MinHeight));
                fixed4 altitudeColor = lerp(_LowColor, _HighColor, altitudeFactor);

                // Combine everything
                terrainColor *= altitudeColor;
                terrainColor.rgb += brightness;
                terrainColor.rgb -= darkness;


                  
                float3 normalDirection = i.worldNormal;
                float atten = _SpecularPower;

                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

			    float3 diffuseReflection = atten * _LightColor0.xyz * dot(normalDirection, lightDirection);


                    float shadowFactor = 1.0 - _ShadowTransition; // Factor for shadow, based on light intensity
                    fixed3 shadowColor = _ShadowTintColor.rgb * _ShadowStrength * shadowFactor; // Shadow color effect

                    float dotProduct = dot(normalDirection, lightDirection);
                  float shadowAmount = saturate(1.0 - dotProduct);  // Чем больше угол между нормалью и направлением света, тем сильнее затемнение
                  fixed3 shadowFinal = lerp(diffuseReflection.rgb, shadowColor, shadowAmount * shadowFactor);

                    diffuseReflection.rgb += shadowFinal;




                float3 lightReflectDirection = reflect(-lightDirection, normalDirection);
				float3 viewDirection = normalize(float3(float4(_WorldSpaceCameraPos.xyz, 1.0) - i.worldPos.xyz));
				float3 lightSeeDirection = max(0.0,dot(lightReflectDirection, viewDirection));
				float3 shininessPower = pow(lightSeeDirection, _Shininess);


				float3 specularReflection = atten * _SpecColor.rgb  * shininessPower;

                   

                // Specular and diffuse lighting
                //float3 diffuse = terrainColor.rgb * _LightColor0.rgb * max(0, dot(normal, _WorldSpaceLightPos0));
                //float3 specular = _LightColor0.rgb * pow(max(0, dot(normal, halfDir)), _Smoothness * 128.0);

                //float3 color = diffuse + specular * _Metallic;
                  fixed shadow = SHADOW_ATTENUATION(i);

                float3 lightFinal = terrainColor + diffuseReflection + specularReflection;
                lightFinal *=shadow;

                float4 finalColorWithFog = float4(lightFinal * _LightColor.rgb, 1.0);

                   UNITY_APPLY_FOG(i.fogCoord, finalColorWithFog);

              

				return finalColorWithFog;
                


                //return terrainColor;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
