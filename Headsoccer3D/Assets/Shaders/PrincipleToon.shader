Shader "Saphead Studios/Principle Toon"
{
    Properties
	{
		//Base
		_BaseMap("Surface Texture", 2D) = "white" {}
		_BaseColor ("Base Tint", Color) = (1, 1, 1, 1)
		_BaseStrength("Surface Texture Lightness", Range(0,1)) = 0
        [Toggle(_AMBIENTLIGHTING)] _AmbientToggle ("Use Ambient Light", Float) = 0
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
            #include "Assets/Shaders/SapheadLighting.hlsl"
            #include "Assets/Shaders/PrincipleToonInitData.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl"

			#pragma vertex PrincipleToonVertexLit
			#pragma fragment PrincipleToonFragmentLit

            //Shader speciific
            #pragma shader_feature_local_fragment _AMBIENTLIGHTING

            //From Simple Lit
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            //#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_COOKIES


            //Then This
            void InitializeSurfaceData(Varyings i, out SurfaceData surfaceData)
            {
                surfaceData = (SurfaceData) 0; // avoids "not completely initalized" errors

                surfaceData.albedo = 1.0h;
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
                half3 ambientLight = 0.0h;
                #if _AMBIENTLIGHTING
					ambientLight = GetBakedGIData(i, inputData);
				#endif
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
                aoPattern = smoothstep(0.7, 0.9, aoPattern);
				//return half4( color.rgb * aoPattern, 1.0h);

                
				//------------------------------Shadow
				Light mainLight = GetMainLight(inputData, CalculateShadowMask(inputData), aoFactor);
				float shadow = saturate((mainLight.shadowAttenuation * mainLight.distanceAttenuation) + color.r + color.g + color.b);
				//manipulate the UV too
				float2 shadowUV = i.uv * _ShadowFrequency;

                //------------------Blend base and shadow
				half shadowStylized = SAMPLE_TEXTURE2D(_ShadowTex, sampler_ShadowTex, shadowUV).r;
				half baseTexture = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).r;
				float shadowPattern = lerp(1.0, shadowStylized, 1 - shadow);
                //harsher shadow
                //shadowPattern =  smoothstep(0.1, 0.3, shadowPattern);

				half basePattern = lerp(baseTexture, 1.0, 
                                        saturate(color.r + color.g + color.b + _BaseStrength));

                //return basePattern * shadowPattern;

                half3 baseLitPattern = (color.rgb + ambientLight) * basePattern;

                half4 finalPattern = half4 (shadowPattern * baseLitPattern, 1.0h);

				return finalPattern * aoPattern;
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