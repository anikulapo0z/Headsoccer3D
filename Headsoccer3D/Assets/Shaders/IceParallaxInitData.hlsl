#ifndef ICE_PARALLAX_INIT_DATA_INCLUDED
#define ICE_PARALLAX_INIT_DATA_INCLUDED

//no need CBuffer ig since we wont be super duper batching this shader materials
float4 _BaseColor1;
float4 _BaseColor2;

float _FloatStrength;
float _FloatAmplitude;

float _Depth;
float _Screen_Mix;
int _Parallax_Layers;

//bump and base map and thei samplers handled in the SurfaceInput
float4 _BaseMap_ST;
float _BumpScale;

TEXTURE2D(_OverlayMap);
SAMPLER(sampler_OverlayMap);
float4 _OverlayMap_ST;
float _OverlayStrength;

struct Attributes 
{
    float4 positionOS	: POSITION;
    float4 tangentOS 	: TANGENT;
    float4 normalOS		: NORMAL;
    float2 uv		    : TEXCOORD0;
    float2 lightmapUV	: TEXCOORD1;
};

struct Varyings 
{
    float4 positionCS 					: SV_POSITION; // CS: clip space
    float3 positionWS					: TEXCOORD0;   // WS: world space
    float2 uv		    				: TEXCOORD1;
    half4 normalWS					: TEXCOORD2;
    half4 tangentWS					: TEXCOORD3;
    half4 bitangentWS				: TEXCOORD4;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 5);
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord 				: TEXCOORD6;
    #endif
};

float2 ParallaxUV(float2 uv, float3 viewDirWS, float3 normalWS, float3 tangentWS, float3 bitangentWS, float depth)
{
    float3 viewDirTS = TransformWorldToTangent(viewDirWS, half3x3(tangentWS, bitangentWS, normalWS));
    viewDirTS = normalize(viewDirTS);
    float2 offset = (viewDirTS.xy / viewDirTS.z) * depth;
    return uv + offset;
}

void InitializeSurfaceData(Varyings i, out SurfaceData surfaceData)
{
    //init
    surfaceData = (SurfaceData)0;

    //parallax init
    float totalDepth = 0;
    float2 parallaxUV;
    half4 parallaxLayer;

    //top layer (base)
    float2 baseUV = i.uv;
    half4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, baseUV);
    half3 overlay = SAMPLE_TEXTURE2D(_OverlayMap, sampler_OverlayMap, baseUV).rgb;

    _Depth /= 1000.0h;

    for (int layer = 0; layer < _Parallax_Layers; ++layer)
    {
        totalDepth = _Depth * layer;

        //calculate parallax for this layer
        parallaxUV = ParallaxUV(baseUV, GetWorldSpaceViewDir(i.positionWS), i.normalWS.xyz, i.tangentWS.xyz, i.bitangentWS.xyz, totalDepth);

        //blend the Screen Mix too
        parallaxLayer = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, parallaxUV) * _Screen_Mix;
        //parallaxLayer *= SAMPLE_TEXTURE2D(_OverlayMap, sampler_OverlayMap, parallaxUV) * _Screen_Mix;

        //time to add to the cumulative 
        //combine (formula: 1 - (1-a)(1-b))
        albedoAlpha = 1.0 - ((1.0 - albedoAlpha) * (1.0 - parallaxLayer));
    }

    surfaceData.alpha = 1.0h;
    surfaceData.albedo = lerp(_BaseColor1.rgb, _BaseColor2.rgb, albedoAlpha.r + (overlay * _OverlayStrength));

    surfaceData.smoothness = 0.9h;
    surfaceData.metallic = 0.0h;

    surfaceData.normalTS = SampleNormal(baseUV, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
    surfaceData.occlusion = 1.0h;
}

void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData) 
{
    inputData = (InputData)0; // avoids "not completely initalized" errors

    inputData.positionWS = input.positionWS;

    half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
    inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));

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

    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
}

#endif // PRINCIPLE_TOON_INIT_DATA_INCLUDED