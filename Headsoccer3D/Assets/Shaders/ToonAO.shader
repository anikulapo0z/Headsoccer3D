Shader "PrasinShaders/ToonAO"
{
	Properties
	{
		//Base
		_ColorTexture("Color Texture", 2D) = "white" {}
		_BaseColor ("Base Tint", Color) = (1, 1, 1, 1)

		//AO
		_AOTexture("AO Texture", 2D) = "black" {}
		_AOFrequency("AO Frequency", Float) = 1

		//Shadow
		_ShadowTex("Shadow Texture", 2D) = "black" {}
		_ShadowFrequency("_Shadow Frequency", Float) = 1

	}

	SubShader
	{
		Tags { "RenderType"="Opaque" 
			"Queue"="Geometry" "RenderPipeline" = "UniversalPipeline" }

		HLSLINCLUDE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			//Color
			float4 _ColorTexture_ST;
			TEXTURE2D(_ColorTexture);
			SAMPLER(sampler_ColorTexture);
			float4 _BaseColor;

			//AO
			float4 _AOTexture_ST;
			TEXTURE2D(_AOTexture);
			SAMPLER(sampler_AOTexture);
			float _AOFrequency;

			//Shadow
			float4 _ShadowTex_ST;
			TEXTURE2D(_ShadowTex);
			SAMPLER(sampler_ShadowTex);
			float _ShadowFrequency;

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl"

		ENDHLSL

		Pass
		{
			Tags{ "LightMode" = "UniversalForward" }

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

			#define _SCREEN_SPACE_OCCLUSION

			struct Attributes 
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				float3 normalOS : NORMAL;
				float2 lightmapUV : TEXCOORD1;
			};

			struct Varyings 
			{
				float4 positionCS : SV_POSITION;
			    float2 uv : TEXCOORD0;
			    float3 normalWS : TEXCOORD1;
				float3 viewWS : TEXCOORD2;
				//lightmap and shadow related data
				DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 3);
				float4 shadowCoord : TEXCOORD4;

			};

			Varyings vert(Attributes i)
			{
				Varyings v;

				VertexPositionInputs positionInputs = GetVertexPositionInputs(i.positionOS.xyz);
				VertexNormalInputs normalInputs = GetVertexNormalInputs(i.normalOS.xyz);

				v.positionCS = positionInputs.positionCS;
				v.normalWS = NormalizeNormalPerVertex(normalInputs.normalWS);
				v.viewWS = GetWorldSpaceViewDir(positionInputs.positionWS);

				//Lightmap and S.Harmonics things that I dont understand yet
				OUTPUT_LIGHTMAP_UV(i.lightmapUV, unity_LightmapST, v.lightmapUV);
				OUTPUT_SH(v.normalWS.xyz, v.vertexSH);
				v.shadowCoord = TransformWorldToShadowCoord(positionInputs.positionWS);

				v.uv = i.uv;
				//perhaps shadow needs its own uv through TEXSample?
			    return v;
			}

			half4 frag(Varyings i) : SV_Target
			{
				//normalize ofc
				float3 normal = normalize(i.normalWS);
				float3 view = normalize(i.viewWS);

				//------------------------------Base Color
				half3 color = SampleAlbedoAlpha(i.uv, TEXTURE2D_ARGS(_ColorTexture, sampler_ColorTexture)).rgb * _BaseColor.rgb;

				//------------------------------AO
				//for whatever reason, #define _SCREEN_SPACE_OCCLUSION is still not setting flag for us to get SSAO from function below
				//AmbientOcclusionFactor ssaoForMe = GetScreenSpaceAmbientOcclusion(GetNormalizedScreenSpaceUV(i.positionCS));
				// so I will sample ssao manually, cue copy pasta source code here
				float ssao = saturate(SampleAmbientOcclusion(GetNormalizedScreenSpaceUV(i.positionCS)) + (1.0 - _AmbientOcclusionParam.x));
				//even building AmbientOcclusionFactor manually
				AmbientOcclusionFactor aoFactor;
				aoFactor.indirectAmbientOcclusion = ssao;
				aoFactor.directAmbientOcclusion = lerp(1.0, ssao, _AmbientOcclusionParam.w);

				//move the AO with uv
				float2 screenUV = GetNormalizedScreenSpaceUV(i.positionCS);

				float aoStylized = SAMPLE_TEXTURE2D(_AOTexture, sampler_AOTexture, (i.uv * _AOFrequency) + (screenUV * 0.5)).r;
				float aoPattern = lerp(aoStylized, 1.0, aoFactor.directAmbientOcclusion);

				//------------------------------Shadow
				Light mainLight = GetMainLight(i.shadowCoord);
				float shadow = mainLight.shadowAttenuation;
				//manipulate the UV too
				float2 shadowUV = i.uv * _ShadowFrequency;
				float shadowStylized = SAMPLE_TEXTURE2D(_ShadowTex, sampler_ShadowTex, shadowUV).r;
				float shadowPattern = lerp(1.0, shadowStylized, 1.0 - shadow);

				return float4( color * aoPattern, 1.0);
			}


			ENDHLSL
		}

		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			ZWrite On
			HLSLPROGRAM
			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment
			#pragma multi_compile_instancing
			
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"

			ENDHLSL
		}

		//needed for AO

		Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ColorMask R
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LOD_FADE_CROSSFADE

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Includes
            //#include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

		Pass 
		{
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma vertex DepthNormalsVertex
			#pragma fragment DepthNormalsFragment

			// Material Keywords
			#pragma shader_feature_local _NORMALMAP
			//#pragma shader_feature_local _PARALLAXMAP
			//#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			// GPU Instancing
			#pragma multi_compile_instancing
			//#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"

			// Note if we do any vertex displacement, we'll need to change the vertex function. e.g. :
			/*
			#pragma vertex DisplacedDepthOnlyVertex (instead of DepthOnlyVertex above)

			Varyings DisplacedDepthOnlyVertex(Attributes input) {
			Varyings output = (Varyings)0;
			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			// Example Displacement
			input.positionOS += float4(0, _SinTime.y, 0, 0);

			output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
			output.positionCS = TransformObjectToHClip(input.position.xyz);
			VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangentOS);
			output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
			return output;
			}
			*/
	
			ENDHLSL
		}
	}
}