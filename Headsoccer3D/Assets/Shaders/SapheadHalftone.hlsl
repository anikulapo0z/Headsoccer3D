#ifndef SAPHEAD_HALFTONE_INCLUDED
#define SAPHEAD_HALFTONE_INCLUDED

//Heavily based on: https://www.ronja-tutorials.com/post/040-halftone-shading/
#include "Assets/Shaders/SapheadLighting.hlsl"
#if defined(LOD_FADE_CROSSFADE)
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
#endif

//CBUFFER perhaps to mnake it SRP Batch compatible?
//since I am keeping vars in ttwo separate hlsl file. We will ned to put the hlsls declareds in a single file and have just one CBUFFER
    //Halftone
    float4 _HalftonePattern_ST;
    TEXTURE2D(_HalftonePattern);
    SAMPLER(sampler_HalftonePattern);
    //remapping values
    float _RemapInputMin;
    float _RemapInputMax;
    float _RemapOutputMin;
    float _RemapOutputMax;


// This function remaps values from a input to a output range
float map(float input, float inMin, float inMax, float outMin, float outMax)
{
            //inverse lerp with input range
    float relativeValue = (input - inMin) / (inMax - inMin);
            //lerp with output range
    return lerp(outMin, outMax, relativeValue);
}

//our lighting function.
half4 LightingHalftone(float3 albedo, float lightIntensity, float2 ScreenPos)
{
	//get halftone comparison value
    
    float halftoneValue = SAMPLE_TEXTURE2D(_HalftonePattern, sampler_HalftonePattern, ScreenPos);
    
    //make lightness binary between fully lit and fully shadow based on halftone pattern (with a bit of antialiasing between)
    halftoneValue = map(halftoneValue, _RemapInputMin, _RemapInputMax, _RemapOutputMin, _RemapOutputMax);
    float halftoneChange = fwidth(halftoneValue) * 0.5;
    lightIntensity = smoothstep(halftoneValue - halftoneChange, halftoneValue + halftoneChange, lightIntensity);
    
	//combine the color
    half4 col;
    col.rgb = lightIntensity * albedo;
	//in case we want to make the shader transparent in the future - irrelevant right now
    col.a = 1.0;

    return col;
}
#endif // SAPHEAD_HALFTONE_INCLUDED
