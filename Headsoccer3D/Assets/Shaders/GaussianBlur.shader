//A very shitty Gaussian Blur with no gaussian math 
Shader "Saphead Studios/Blur"
{
    Properties
	{
		_TexelSizeX("Texel Size X", Float) = 1920
		_TexelSizeY("Texel Size Y", Float) = 1080
		_GridSize("Grid Size", Range(0, 10)) = 1
	}

    SubShader
    {
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

		HLSLINCLUDE

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

			float _TexelSizeX;
			float _TexelSizeY;
			float _GridSize;

		ENDHLSL
		ZWrite Off Cull Off
		Pass
		{
			Name "Non Accurate Horizontal Gaussian Blur by Prasin"

			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment frag_horizontal

				//Out frag function takes as input a struct that contains the screen space coordinate we are
				//going to use to sample our texture. It also writes to SV_Target0, this has to match the index set in the
				//SetRenderAttachment(sourceTexture, 0) we defined in our render pass script.
				float4 frag_horizontal (Varyings i) : SV_Target0
				{
					float3 col = float3(0.0f, 0.0f, 0.0f);
					float gridSum = 0.0f;

					int upper = floor(_GridSize);
					int lower = -upper;

					for (int x = lower; x <= upper; ++x)
					{
						float2 uv = saturate( i.texcoord.xy + float2(x/_TexelSizeX, 0.0f));
						col += SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearRepeat, uv, _BlitMipLevel).xyz;
					}

					col /= (upper - lower + 1);
					return float4(col, 1.0f);
				}

			ENDHLSL
		}

		
		Pass
		{
			Name "Non Accurate Vertical Gaussian Blur by Prasin"

			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment frag_vertical
				//Out frag function takes as input a struct that contains the screen space coordinate we are
				//going to use to sample our texture. It also writes to SV_Target0, this has to match the index set in the
				//SetRenderAttachment(sourceTexture, 0) we defined in our render pass script.
				float4 frag_vertical (Varyings i) : SV_Target0
				{
					float3 col = float3(0.0f, 0.0f, 0.0f);
					float gridSum = 0.0f;

					int upper = floor(_GridSize);
					int lower = -upper;

					for (int y = lower; y <= upper; ++y)
					{
						float2 uv = saturate( i.texcoord.xy + float2(0.0f, y/_TexelSizeY));
						col += SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearRepeat, uv, _BlitMipLevel).xyz;
					}

					col /= (upper - lower + 1);
					return float4(col, 1.0f);
				}

			ENDHLSL
		}
		
     }
}
