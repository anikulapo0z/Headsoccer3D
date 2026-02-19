#ifndef SAPHEAD_HALFTONE_INCLUDED
#define SAPHEAD_HALFTONE_INCLUDED

//Heavily based on: https://www.ronja-tutorials.com/post/040-halftone-shading/
#include "Assets/Shaders/SapheadLighting.hlsl"
#include "Assets/Shaders/PrincipleToonInitData.hlsl"

#if defined(LOD_FADE_CROSSFADE)
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
#endif


float halftoneValue(float4 screenPos)
{
    float2 screenUV = screenPos.xy / screenPos.w;
    screenUV = TRANSFORM_TEX(screenUV, _HalftonePattern);

    float aspect = _ScreenParams.x / _ScreenParams.y;
    screenUV.x *= aspect;
    return SAMPLE_TEXTURE2D(_HalftonePattern, sampler_HalftonePattern, screenUV).r;
}

// This function remaps values from a input to a output range
float RemapLight(float input, float inMin, float inMax, float outMin, float outMax)
{
    float relative = (input - inMin) / (inMax - inMin);
    return lerp(outMin, outMax, relative);
}

//our lighting function.
half LightingHalftone(half lightIntensity, float4 screenPos)
{
    float halftone = halftoneValue(screenPos);
        //make lightness binary between fully lit and fully shadow based on halftone pattern (with a bit of antialiasing between)
        //honestly the math here is confusing me, so shamelessly translated from Ronja's tutorial
    halftone = RemapLight(halftone,
                                    _RemapInputMin,
                                    _RemapInputMax,
                                    _RemapOutputMin,
                                    _RemapOutputMax);
    float change = fwidth(halftone) * 0.5;
    lightIntensity = smoothstep(
                        halftone - change,
                        halftone + change,
                        lightIntensity);
          
    return lightIntensity;
}

    //DEPRICATED
    /*
    half StylizedAO()
    {
		//------------------------------AO and its Pattern
		//sample ssao manually, cue copy pasta source code from ShaderLibrary/AmbientOcclusion.hlsl
		float ssao = saturate(SampleAmbientOcclusion(GetNormalizedScreenSpaceUV(i.positionCS)) + (1.0 - _AmbientOcclusionParam.x));
		//even building AmbientOcclusionFactor manually
		AmbientOcclusionFactor aoFactor;
		aoFactor.indirectAmbientOcclusion = ssao;
		aoFactor.directAmbientOcclusion = lerp(1.0h, ssao, _AmbientOcclusionParam.w);
		//move the AO with uv
		float2 screenUV = GetNormalizedScreenSpaceUV(i.positionCS);
        //and the pattern
		half aoStylized = SAMPLE_TEXTURE2D(_AOTexture, sampler_AOTexture, (i.uv * _AOFrequency) + (screenUV * 0.5)).r;
		half aoPattern = lerp(aoStylized, 1.0, aoFactor.directAmbientOcclusion);
        aoPattern = smoothstep(0.7, 0.9, aoPattern);
        aoPattern += _AOLightness;
        aoPattern = saturate(_AOLightness);
        return aoPattern;
    }*/
#endif // SAPHEAD_HALFTONE_INCLUDED