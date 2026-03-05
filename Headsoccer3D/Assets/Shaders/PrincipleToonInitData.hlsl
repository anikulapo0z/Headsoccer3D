#ifndef PRINCIPLE_TOON_INIT_DATA_INCLUDED
#define PRINCIPLE_TOON_INIT_DATA_INCLUDED

#include "Assets/Shaders/SapheadLighting.hlsl"
#if defined(LOD_FADE_CROSSFADE)
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
#endif

    //Base
    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);

    //Mask
    TEXTURE2D(_MaskTex);
    SAMPLER(sampler_MaskTex);

    //AO
    TEXTURE2D(_AOTexture);
    SAMPLER(sampler_AOTexture);

    //Shadow
    TEXTURE2D(_ShadowTexture);
    SAMPLER(sampler_ShadowTexture);

    //Halftone
    TEXTURE2D(_HalftonePattern);
    SAMPLER(sampler_HalftonePattern);
    //Halftone Mask
    TEXTURE2D(_HalftoneMaskTex);
    SAMPLER(sampler_HalftoneMaskTex);

//CBUFFER perhaps to mnake it SRP Batch compatible
    CBUFFER_START(UnityPerMaterial)
        float4 _BaseColor;
        float _Lightness;
        float _BaseStrength;
        float4 _Color;
        float4 _Emission;
        float4 _FirstMaskColor;
        float4 _SecondMaskColor;

        float4 _MainTex_ST;
        float4 _HalftonePattern_ST;
        float4 _ShadowTexture_ST;
        float4 _AOTexture_ST;

        float _AOFrequency;
        float _AOLightness;
        float _ShadowFrequency;
        float _ShadowLightness;

        float4 _HalftoneColor;

        float _RemapInputMin;
        float _RemapInputMax;
        float _RemapOutputMin;
        float _RemapOutputMax;

        float _DesaturationValue;
    CBUFFER_END


struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 texcoord : TEXCOORD0;
    float2 texcoord1 : TEXCOORD1;
    float2 texcoord2 : TEXCOORD2;
    float2 texcoord3 : TEXCOORD3;
    float2 texcoord4 : TEXCOORD4;
    float2 staticLightmapUV : TEXCOORD5;
    float2 dynamicLightmapUV : TEXCOORD6;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1; // xyz: posWS
    half3 normalWS : TEXCOORD2;
    
#ifdef _ADDITIONAL_LIGHTS_VERTEX
        half4 fogFactorAndVertexLight  : TEXCOORD5; // x: fogFactor, yzw: vertex light
#else
    half fogFactor : TEXCOORD5;
    #endif

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord             : TEXCOORD6;
    #endif

    DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 7);

#ifdef DYNAMICLIGHTMAP_ON
        float2  dynamicLightmapUV : TEXCOORD8; // Dynamic lightmap UVs
#endif

#ifdef USE_APV_PROBE_OCCLUSION
        float4 probeOcclusion : TEXCOORD9;
#endif

float4 screenPos : TEXCOORD10;

UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
{
    inputData = (InputData) 0;

    inputData.positionWS = input.positionWS;
#if defined(DEBUG_DISPLAY)
        inputData.positionCS = input.positionCS;
#endif


    half3 viewDirWS = GetWorldSpaceNormalizeViewDir(inputData.positionWS);
    inputData.normalWS = input.normalWS;


    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    viewDirWS = SafeNormalize(viewDirWS);

    inputData.viewDirectionWS = viewDirWS;

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            inputData.shadowCoord = input.shadowCoord;
#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
            inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
#else
    inputData.shadowCoord = float4(0, 0, 0, 0);
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
            inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactorAndVertexLight.x);
            inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
#else
    inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactor);
    inputData.vertexLighting = half3(0, 0, 0);
#endif

    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

#if defined(DEBUG_DISPLAY)
#if defined(DYNAMICLIGHTMAP_ON)
            inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
#endif
    
#if defined(LIGHTMAP_ON)
            inputData.staticLightmapUV = input.staticLightmapUV;
#else
            inputData.vertexSH = input.vertexSH;
#endif
    
#if defined(USE_APV_PROBE_OCCLUSION)
            inputData.probeOcclusion = input.probeOcclusion;
#endif
#endif
}

void InitializeBakedGIData(Varyings input, inout InputData inputData)
{
#if defined(DYNAMICLIGHTMAP_ON)
    inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, inputData.normalWS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
    inputData.bakedGI = SAMPLE_GI(input.vertexSH,
        GetAbsolutePositionWS(inputData.positionWS),
        inputData.normalWS,
        inputData.viewDirectionWS,
        input.positionCS.xy,
        input.probeOcclusion,
        inputData.shadowMask);
#else
    inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, inputData.normalWS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
#endif
}

half3 GetBakedGIData(Varyings input, InputData inputData)
{
#if defined(DYNAMICLIGHTMAP_ON)
    return SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, inputData.normalWS);
    
#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
    return SAMPLE_GI(input.vertexSH,
        GetAbsolutePositionWS(inputData.positionWS),
        inputData.normalWS,
        inputData.viewDirectionWS,
        input.positionCS.xy,
        input.probeOcclusion,
        inputData.shadowMask);
#else
    return SAMPLE_GI(input.staticLightmapUV, input.vertexSH, inputData.normalWS);
#endif
}
#endif // PRINCIPLE_TOON_INIT_DATA_INCLUDED