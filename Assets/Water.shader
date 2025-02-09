Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

       float3 albedo : source_color;
 sampler2D water_texture1;
 sampler2D water_texture2;
 sampler2D noise_texture;
 float2 scroll_speed1 = float2(0.05, 0.0);
 float2 scroll_speed2 = float2(-0.03, 0.0);
 float blend_factor = 0.5;
 float2 scale1 = float2(1.0, 1.0);
 float2 scale2 = float2(1.0, 1.0);
 float wave_strength = 1.0;
 float wave_scale = 0.02;
 int pixelation_level = 64;
 float FoamSize = 0.5;
 sampler2D DepthTexture : hint_depth_texture; 
 float WaterOpacity = 1.0;
 float FoamGlowIntensity = 0.5;

void vertex() {
    vec2 global_position = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xz;
    float noise_value = texture(noise_texture, global_position * wave_scale).r;
    float wave = sin(global_position.x * 0.2 + global_position.y * 0.2 + TIME + noise_value * 10.0) * wave_strength;
    VERTEX.y += wave;
}

void fragment() {
    vec2 scaledUV1 = UV * scale1;
    vec2 scaledUV2 = UV * scale2;
    vec2 scrolledUV1 = scaledUV1 + scroll_speed1 * TIME;
    vec2 scrolledUV2 = scaledUV2 + scroll_speed2 * TIME;
    scrolledUV1 = mod(scrolledUV1, vec2(1.0, 1.0));
    scrolledUV2 = mod(scrolledUV2, vec2(1.0, 1.0));
    scrolledUV1 = floor(scrolledUV1 * float(pixelation_level)) / float(pixelation_level);
    scrolledUV2 = floor(scrolledUV2 * float(pixelation_level)) / float(pixelation_level);

    vec4 water_color1 = texture(water_texture1, scrolledUV1);
    vec4 water_color2 = texture(water_texture2, scrolledUV2);
    vec4 blended_water_color = mix(water_color1, water_color2, blend_factor);

    float depthValue = texture(DepthTexture, SCREEN_UV).r;
    vec4 worldPosition = INV_PROJECTION_MATRIX * vec4(SCREEN_UV * 2.0 - 1.0, depthValue, 1.0);
    worldPosition.xyz /= worldPosition.w;
    float foamEffect = clamp(1.0 - smoothstep(worldPosition.z + FoamSize, worldPosition.z, VERTEX.z), 0.0, 1.0);
    float foamOpacity = 1.0 - foamEffect;
    float foamEffectRounded = round(foamOpacity);
    float finalOpacity = foamEffectRounded + WaterOpacity;
	
    ALBEDO = blended_water_color.rgb * albedo;
    ALPHA = finalOpacity;
    EMISSION = vec3(foamEffectRounded) * FoamGlowIntensity;
    METALLIC = 0.0;
    ROUGHNESS = 1.0;
}
        ENDCG
    }
    FallBack "Diffuse"
}
