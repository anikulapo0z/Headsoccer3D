Shader "Saphead Studios/ScreenVoronoi"
{
	Properties
	{
		_CellSize ("Cell Size", Float) = 0.1
		_TexTiling ("Tex Size", Float) = 0.1
		_Jaggedness ("Jaggedness", Float) = 0.1
		_NoiseScale ("Noise Scale", Range(0, 5)) = 1
		_NoiseIntensity ("Noise Intensity", Float) = 18
		_JitterInterval ("Jitter Interval", Range(0, 5)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
		HLSLINCLUDE

			float _CellSize;
			float _TexTiling;
			float _Jaggedness;
			float _NoiseScale;
			float _NoiseIntensity;
			float _JitterInterval;

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
			//customs
			#include "ShaderUtility.hlsl"
			#include "ShaderUtilityNoise.hlsl"
			#include "ShaderUtilityVoronoi.hlsl"

			//quick has, thanks chattyG
			float Hash(float n)
			{
				return frac(sin(n) * 43758.5453);
			}

			float2 GetJitterOffset(float time, float interval)
			{
				float t = floor(time / interval);

				//jitter
				float x = Hash(t);

				//but y with phase sifference of 0.5 seconds 
				float y = Hash(t + 0.5);

				return float2(x, y);
			}



		ENDHLSL
		ZWrite Off Cull Off
		Pass
		{
			Name "Blit Voronoi Pass"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag
			// Out frag function takes as input a struct that contains the screen space coordinate we are
			//going to use to sample our texture. It also writes to SV_Target0, this has to match the index set in the
			//SetRenderAttachment(sourceTexture, 0) we defined in our render pass script.
			float4 Frag(Varyings i) : SV_Target0
			{
				float2 uv = i.texcoord.xy;
				//process 2d UV
				float2 value = uv * _CellSize;
				float2 jitter = GetJitterOffset(_Time.y, _JitterInterval);
				float noise = PerlinNoise2DOffset(value * _NoiseScale, 6, jitter);
				VoronoiData2D voronoi = voronoiModified2D(value, noise * _NoiseIntensity, _Jaggedness, uv);
				//processed UV must be divided by the cellsize again
				half4 colorMapSampled = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearRepeat, voronoi.cellPosition / _CellSize, _BlitMipLevel);
				return colorMapSampled;
			}
			ENDHLSL
		}
	}
}
