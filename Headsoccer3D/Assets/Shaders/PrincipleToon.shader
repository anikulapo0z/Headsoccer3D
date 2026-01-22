Shader "Saphead Studios/Principle Toon"
{
    Properties
	{
		//Base
		_BaseMap("Color Texture", 2D) = "white" {}
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
			"Queue"="Geometry" 
			"RenderPipeline" = "UniversalPipeline" 
             }
        
		Pass
		{
			Tags{ "LightMode" = "UniversalForward" }

			HLSLPROGRAM
            
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Assets/Shaders/PrincipleToonInitData.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl"

			#pragma vertex PrincipleToonVertexLit
			#pragma fragment PrincipleToonFragmentLit

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            //#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

            //Then This
            void InitializeSurfaceData(Varyings i, out SurfaceData surfaceData)
            {
                surfaceData = (SurfaceData) 0; // avoids "not completely initalized" errors

                half4 albedoAlpha = SampleAlbedoAlpha(i.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    
                surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;
                surfaceData.alpha = 1.0h;
                surfaceData.normalTS = half3(0.0h,0.0h,1.0h);
                surfaceData.occlusion = 1.0h;
            }

			Varyings PrincipleToonVertexLit(Attributes input)
			{
				Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                half fogFactor = 0;

                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionWS.xyz = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;

                output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);

                OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
                #ifdef DYNAMICLIGHTMAP_ON
                    output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                OUTPUT_SH4(vertexInput.positionWS, output.normalWS.xyz, GetWorldSpaceNormalizeViewDir(vertexInput.positionWS), output.vertexSH, output.probeOcclusion);

                #ifdef _ADDITIONAL_LIGHTS_VERTEX
                    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
                    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
                #else
                    output.fogFactor = fogFactor;
                #endif

                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    output.shadowCoord = GetShadowCoord(vertexInput);
                #endif

                return output;
			}

			half4 PrincipleToonFragmentLit(Varyings i) : SV_Target
			{

				//Calc BlinnPhong
				SurfaceData surfaceData;
				InitializeSurfaceData(i, surfaceData);

				InputData inputData;
				InitializeInputData(i, surfaceData.normalTS, inputData);


				half4 color = UniversalFragmentBlinnPhong(inputData, surfaceData);

				//------------------------------AO
				//sample ssao manually, cue copy pasta source code from ShaderLibrary/AmbientOcclusion.hlsl
				float ssao = saturate(SampleAmbientOcclusion(GetNormalizedScreenSpaceUV(i.positionCS)) + (1.0 - _AmbientOcclusionParam.x));
				//even building AmbientOcclusionFactor manually
				AmbientOcclusionFactor aoFactor;
				aoFactor.indirectAmbientOcclusion = ssao;
				aoFactor.directAmbientOcclusion = lerp(1.0h, ssao, _AmbientOcclusionParam.w);

				//move the AO with uv
				float2 screenUV = GetNormalizedScreenSpaceUV(i.positionCS);

				half aoStylized = SAMPLE_TEXTURE2D(_AOTexture, sampler_AOTexture, (i.uv * _AOFrequency) + (screenUV * 0.5)).r;
				half aoPattern = lerp(aoStylized, 1.0, aoFactor.directAmbientOcclusion);

				return half4( color.rgb * aoPattern, 1.0h);

                /*
				//------------------------------Shadow
				Light mainLight = GetMainLight(i.shadowCoord);
				float shadow = mainLight.shadowAttenuation;
				//manipulate the UV too
				float2 shadowUV = i.uv * _ShadowFrequency;
				float shadowStylized = SAMPLE_TEXTURE2D(_ShadowTex, sampler_ShadowTex, shadowUV).r;
				float shadowPattern = lerp(1.0, shadowStylized, 1.0 - shadow);

				return float4( color.rgb * aoPattern, 1.0);*/
				//return float4( color * shadow * aoPattern, 1.0);
			}


			ENDHLSL
		}


        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

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

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            // -------------------------------------
            // Includes
            //#include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Assets/Shaders/SapheadShadowCasterPass.hlsl"
            ENDHLSL
        }
        
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


    }

    Fallback  "Hidden/Universal Render Pipeline/FallbackError"
}