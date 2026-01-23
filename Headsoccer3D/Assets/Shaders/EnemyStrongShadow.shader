Shader "PrasinShaders/EnemyStrongShadow"
{
	Properties
	{
		_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
		//_SpecularPower("Specular Power", Float) = 3
		//_LineDarkness("Line Darkness", Range(0, 1)) = 1

		//AO
		//_AOTexture("AO Texture", 2D) = "black" {}
		_AODarkTexture("AO Dark Texture", 2D) = "black" {}
		_AOFrequency("AO Frequency", Float) = 1

		//Shadow
		//_ShadowTex("Shadow Texture", 2D) = "black" {}
		//_ShadowFrequency("_Shadow Frequency", Float) = 1

		//fresnel
		_FresnelPower("Fresnel Power", Range(0, 7)) = 5
		_FresnelStep("Fresnel Step", Range(0, 1)) = 0.5

	}

	SubShader
	{
		Tags { "RenderType"="Opaque" 
			"Queue"="Geometry" "RenderPipeline" = "UniversalPipeline" }

		HLSLINCLUDE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			//Color
			float4 _BaseColor;
			float4 _BaseMap_ST;

			//float _SpecularPower;

			//Shadow
			// float4 _ShadowTex_ST;
			// TEXTURE2D(_ShadowTex);
			// SAMPLER(sampler_ShadowTex);
			// float _ShadowFrequency;
			//float _LineDarkness;

			//AO
			//float4 _AOTexture_ST;
			float4 _AODarkTexture_ST;
			//TEXTURE2D(_AOTexture);
			TEXTURE2D(_AODarkTexture);
			//SAMPLER(sampler_AOTexture);
			SAMPLER(sampler_AODarkTexture);
			float _AOFrequency;

			//fresnel
			float _FresnelPower;
			float _FresnelStep;


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

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
			    return v;
			}

			half4 frag(Varyings i) : SV_Target
			{
				//normalize ofc
				float3 normal = normalize(i.normalWS);
				float3 view = normalize(i.viewWS);

				//for specular and AO
				//float2 screenUV = GetNormalizedScreenSpaceUV(i.positionCS);

				//-------------------------------Specular from Blinn, modifed to screenspace
				//float2 specularUV =  (screenUV * 2.0) - 1.0;
				/*
				float specular = ((0.5 - screenUV.x) * (0.5 - screenUV.x)) + ((0.5 - screenUV.y) * (0.5 - screenUV.y));
				specular /= _SpecularPower;
				specular = clamp(specular, 0, 1);*/

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
				//float aoStylized = SAMPLE_TEXTURE2D(_AOTexture, sampler_AOTexture, i.uv * _AOFrequency).r;
				float aoDark = SAMPLE_TEXTURE2D(_AODarkTexture, sampler_AODarkTexture, i.uv * _AOFrequency).r;
				float aoGradient = 1.0 - smoothstep(0.4, 0.75, aoFactor.indirectAmbientOcclusion);
				//the dark to stulysed happends been 0.0 to 0.7
				float aoStyleBlend = lerp(1.0, aoDark, aoGradient );
				//float aoPattern = lerp(aoGradient, 1.0, aoFactor.directAmbientOcclusion);

				//fresnel 
				float fresnel = 1.0f - max(0, dot(normal, view));
				fresnel = pow(fresnel, 5);

				//manipulate the UV and stylize the crosshatch, mix
				//float shadowStylized = SAMPLE_TEXTURE2D(_ShadowTex, sampler_ShadowTex, i.uv * _ShadowFrequency).r;
				//shadowStylized = lerp(shadowStylized * 0.1f, min(1, shadowStylized + 1), _LineDarkness);
				//float shadowPattern = lerp(1.0, shadowStylized, specular);


				//combine

				return _BaseColor * aoStyleBlend;
				//return _BaseColor * aoStyleBlend * (1.0 - step(_FresnelStep, fresnel));
				//return _BaseColor * aoPattern * shadowPattern;
				//return distSqr;
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