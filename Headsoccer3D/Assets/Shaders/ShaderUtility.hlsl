#ifndef SHADER_UTILITY_INCLUDED
#define SHADER_UTILITY_INCLUDED

//sourced fron the internet
//https://www.ronja-tutorials.com/post/024-white-noise/
float rand1dTo1d(float value, float seed)
{
    return frac(sin(value * seed) * 43758.5453);
}

float3 rand1dTo3d(float value)
{
    return float3(
        rand1dTo1d(value, 3.9812),
        rand1dTo1d(value, 7.1536),
        rand1dTo1d(value, 5.7241)
    );
}

float rand2dTo1d(float2 value, float2 dotDir = float2(12.9898, 78.233))
{
    float2 smallValue = sin(value);
    float random = dot(smallValue, dotDir);
    random = frac(sin(random) * 143758.5453);
    return random;
}

float2 rand2dTo2d(float2 value)
{
    return float2(
        rand2dTo1d(value, float2(12.989, 78.233)),
        rand2dTo1d(value, float2(39.346, 11.135))
    );
}
float3 rand2dTo3d(float2 value)
{
    return float3(
        rand2dTo1d(value, float2(12.989, 78.233)),
        rand2dTo1d(value, float2(82.9678, 67.69)),
        rand2dTo1d(value, float2(39.346, 11.135))
    );
}

float rand3dTo1d(float3 value, float3 dotDir = float3(12.9898, 78.233, 37.719))
{
    //make value smaller to avoid artefacts
    float3 smallValue = sin(value);
    //get scalar value from 3d vector
    float random = dot(smallValue, dotDir);
    //make value more random by making it bigger and then taking the factional part
    random = frac(sin(random) * 143758.5453);
    return random;
}

float3 rand3dTo3d(float3 value)
{
    return float3(
        rand3dTo1d(value, float3(12.989, 78.233, 37.719)),
        rand3dTo1d(value, float3(39.346, 11.135, 83.155)),
        rand3dTo1d(value, float3(73.156, 52.235, 09.151))
    );
}

inline float easeIn(float interpolator)
{
    return interpolator * interpolator;
}
float easeOut(float interpolator)
{
    return 1 - easeIn(1 - interpolator);
}
float easeInOut(float interpolator)
{
    float easeInValue = easeIn(interpolator);
    float easeOutValue = easeOut(interpolator);
    return lerp(easeInValue, easeOutValue, interpolator);
}

//uses baycentric formula to get the location of the pixel in the uv space, by looking for the vertices of the triangle the pixel falls in 
//https://gamedev.stackexchange.com/questions/72061/3d-position-to-uv-coordinates-in-fragment-shader
float2 WorldToUV(float3 P, float3 A, float3 B, float3 C, float2 uvA, float2 uvB, float2 uvC)
{
    float3 v0 = B - A;
    float3 v1 = C - A;
    float3 v2 = P - A;

    float d00 = dot(v0, v0);
    float d01 = dot(v0, v1);
    float d11 = dot(v1, v1);
    float d20 = dot(v2, v0);
    float d21 = dot(v2, v1);

    float denom = d00 * d11 - d01 * d01;
    float v = (d11 * d20 - d01 * d21) / denom;
    float w = (d00 * d21 - d01 * d20) / denom;
    float u = 1.0 - v - w;

    return uvA * u + uvB * v + uvC * w;
}

//custom bilinear
//https://www.gamedev.net/forums/topic/701772-how-to-manually-create-bilinear-interpolation-in-hlsl-using-gather-functions/
float3 bilinear(float2 texcoord, float tex_dimension, float4 reds, float4 greens, float4 blues)
{
    float3 result;

    // red channel
    float r1 = reds.x;
    float r2 = reds.y;
    float r3 = reds.z;
    float r4 = reds.w;

    float2 pixel = texcoord * tex_dimension + 0.5;
    float2 fract = frac(pixel);
      
    float top_row_red = lerp(r4, r3, fract.x);
    float bottom_row_red = lerp(r1, r2, fract.x);

    float final_red = lerp(top_row_red, bottom_row_red, fract.y);
    result.x = final_red;
            
    // green channel
    float g1 = greens.x;
    float g2 = greens.y;
    float g3 = greens.z;
    float g4 = greens.w;

    float top_row_green = lerp(g4, g3, fract.x);
    float bottom_row_green = lerp(g1, g2, fract.x);

    float final_green = lerp(top_row_green, bottom_row_green, fract.y);
    result.y = final_green;
            
    // blue channel
    float b1 = blues.x;
    float b2 = blues.y;
    float b3 = blues.z;
    float b4 = blues.w;

    float top_row_blue = lerp(b4, b3, fract.x);
    float bottom_row_blue = lerp(b1, b2, fract.x);

    float final_blue = lerp(top_row_blue, bottom_row_blue, fract.y);
    result.z = final_blue;

    return result;
}
#endif // SHADER_UTILITY_INCLUDED
