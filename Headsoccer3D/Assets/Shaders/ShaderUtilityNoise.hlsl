#ifndef SHADER_UTILITY_NOISE_INCLUDED
#define SHADER_UTILITY_NOISE_INCLUDED

#include "ShaderUtility.hlsl"

float ValueNoise1d(float3 value)
{
    float interpolatorX = easeInOut(frac(value.x));
    float interpolatorY = easeInOut(frac(value.y));
    float interpolatorZ = easeInOut(frac(value.z));

    float cellNoiseZ[2];
    [unroll]
    for (int z = 0; z <= 1; z++)
    {
        float cellNoiseY[2];
        [unroll]
        for (int y = 0; y <= 1; y++)
        {
            float cellNoiseX[2];
            [unroll]
            for (int x = 0; x <= 1; x++)
            {
                float3 cell = floor(value) + float3(x, y, z);
                cellNoiseX[x] = rand3dTo1d(cell);
            }
            cellNoiseY[y] = lerp(cellNoiseX[0], cellNoiseX[1], interpolatorX);
        }
        cellNoiseZ[z] = lerp(cellNoiseY[0], cellNoiseY[1], interpolatorY);
    }
    float noise = lerp(cellNoiseZ[0], cellNoiseZ[1], interpolatorZ);
    return noise;
}

// Based on Morgan McGuire @morgan3d
// https://www.shadertoy.com/view/4dS3Wd
float Noise(float2 value)
{
    float2 i = floor(value);
    float2 f = value - i; //fraction part, or the fract

    // Four corners in 2D of a tile
    float a = rand2dTo1d(i);
    float b = rand2dTo1d(i + float2(1.0, 0.0));
    float c = rand2dTo1d(i + float2(0.0, 1.0));
    float d = rand2dTo1d(i + float2(1.0, 1.0));

    float2 u = f * f * (3.0 - 2.0 * f);

    return lerp(a, b, u.x) +
            (c - a) * u.y * (1.0 - u.x) +
            (d - b) * u.x * u.y;
}

float3 ValueNoise3d(float3 value)
{
    float interpolatorX = easeInOut(frac(value.x));
    float interpolatorY = easeInOut(frac(value.y));
    float interpolatorZ = easeInOut(frac(value.z));

    float3 cellNoiseZ[2];
    [unroll]
    for (int z = 0; z <= 1; z++)
    {
        float3 cellNoiseY[2];
        [unroll]
        for (int y = 0; y <= 1; y++)
        {
            float3 cellNoiseX[2];
            [unroll]
            for (int x = 0; x <= 1; x++)
            {
                float3 cell = floor(value) + float3(x, y, z);
                cellNoiseX[x] = rand3dTo3d(cell);
            }
            cellNoiseY[y] = lerp(cellNoiseX[0], cellNoiseX[1], interpolatorX);
        }
        cellNoiseZ[z] = lerp(cellNoiseY[0], cellNoiseY[1], interpolatorY);
    }
    float3 noise = lerp(cellNoiseZ[0], cellNoiseZ[1], interpolatorZ);
    return noise;
}

float PerlinNoise2d(float2 uv, int octaves)
{
    uv *= 3.0f;
    
    //FBM
    //https://thebookofshaders.com/13/
    // Initial values
    float value = 0.000f;
    float amplitude = 0.500f;
    float frequency = 0.000f;
    //
    // Loop of octaves
    for (int i = 0; i < octaves; i++)
    {
        value += amplitude * Noise(uv);
        uv *= 2.0f;
        amplitude *= 0.5f;
    }
    
    return value;
}

float PerlinNoise2DOffset(float2 uv, int octaves, float2 offset)
{
    uv = uv * 3.0f + offset;

    float value = 0.0f;
    float amplitude = 0.5f;

    for (int i = 0; i < octaves; i++)
    {
        value += amplitude * Noise(uv);
        uv *= 2.0f;
        amplitude *= 0.5f;
    }

    return value;
}


#endif // SHADER_UTILITY_NOISE_INCLUDED
