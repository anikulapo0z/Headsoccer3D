Shader "Saphead Studios/Principle Toon"
{
    Properties
	{
		//Base
        [Header(Base)]
		_MainTex("Surface Texture", 2D) = "white" {}
        _Lightness("Lightness", Range(0,1)) = 0
		_BaseColor ("Base Tint", Color) = (1, 1, 1, 1)
        [Space(10)]
        [Toggle(_MASKING)] _MaskToggle ("Use Mask (Disables Surface Texture)", Float) = 0
		[NoScaleOffset] _MaskTex("Mask Map", 2D) = "white" {}
        _FirstMaskColor ("Mask Color 1 (Black)", Color) = (0, 0, 0, 1)
        _SecondMaskColor ("Mask Color 2 (White)", Color) = (1, 1, 1, 1)
		//_BaseStrength("Surface Texture Lightness", Range(0,1)) = 0
        //[Toggle(_AMBIENTLIGHTING)] _AmbientToggle ("Use Ambient Light", Float) = 0

        //[Space(20)]
		//AO
		//_AOTexture("AO Texture", 2D) = "black" {}
		//_AOFrequency("AO Frequency", Float) = 1
		//_AOLightness("AO Lightness", Range(0,1)) = 0

		//Shadow
        [Space(20)]
        [Header(Shadows)]
        [Toggle(_SHADOWSLIGHT)] _ShadowsToggle ("Calculate Shadows", Float) = 1
		[NoScaleOffset]_ShadowTexture("Shadow Texture", 2D) = "black" {}
		_ShadowFrequency("Shadow Frequency", Float) = 1
		_ShadowLightness("Shadow Lightness", Range(0,1)) = 0

        //Halftone 
        //https://www.ronja-tutorials.com/post/040-halftone-shading/
        [Space(20)]
        [Header(Halftone)]
        [Toggle(_HALFTONELIGHT)] _HalftoneToggle ("Use Halftone", Float) = 1
        [Toggle(_USESHADOWS)] _UseShadowsToggle ("Use Shadow for Halftone calc", Float) = 1
		_HalftoneColor ("Halftone Color", Color) = (1, 1, 1, 1)
        _HalftonePattern("Halftone Pattern", 2D) = "white" {}
        [Toggle(_HALFTONEMASK)] _HalftoneMaskToggle ("Use Halftone Mask", Float) = 0
		[NoScaleOffset] _HalftoneMaskTex("Halftone Mask Map", 2D) = "white" {}
        [Toggle(_HALFTONEMASKINVERT)] _HalftoneMaskInvertToggle ("Invert Halftone Mask", Float) = 0

        [Space(10)]
        _RemapInputMin ("Remap input min value", Range(-1, 1)) = 0
        _RemapInputMax ("Remap input max value", Range(-1, 1)) = 1
        _RemapOutputMin ("Remap output min value", Range(-1, 1)) = 0
        _RemapOutputMax ("Remap output max value", Range(-1, 1)) = 1
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
            #include "Assets/Shaders/SapheadHalftone.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl"

			#pragma vertex PrincipleToonVertexLit
			#pragma fragment PrincipleToonFragmentLit

            //Shader speciific
            #pragma shader_feature_local_fragment _AMBIENTLIGHTING
            #pragma shader_feature_local_fragment _SHADOWSLIGHT
            #pragma shader_feature_local_fragment _USESHADOWS
            #pragma shader_feature_local_fragment _HALFTONELIGHT
            #pragma shader_feature_local_fragment _MASKING
            #pragma shader_feature_local_fragment _HALFTONEMASK
            #pragma shader_feature_local_fragment _HALFTONEMASKINVERT

            //From Simple Lit
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _FORWARD_PLUS
            //#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
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

                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
                output.positionWS.xyz = vertexInput.positionWS;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);;

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

                //for halftone screen space
                output.screenPos = ComputeScreenPos(output.positionCS);

                return output;
			}

			half4 PrincipleToonFragmentLit(Varyings i) : SV_Target
			{
                //----------------------------Basis
				SurfaceData surfaceData;
				InitializeSurfaceData(i, surfaceData);
				InputData inputData;
				InitializeInputData(i, surfaceData.normalTS, inputData);
                
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(i.positionWS)); //passing the shadow coord

                half4 baseTexture;

                #if _MASKING
                    baseTexture = lerp(_FirstMaskColor, _SecondMaskColor, SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv));
                #else
                    baseTexture = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                    baseTexture += _Lightness;
                    baseTexture = saturate(baseTexture);
                    baseTexture *= _BaseColor;
                #endif

				//----------------------------BlinnPhong for lighting data
                //half3 ambientLight = 0.0h;
                //#if _AMBIENTLIGHTING
				//	ambientLight = GetBakedGIData(i, inputData);
				//#endif
                //excludes color
				//half4 lighting = UniversalFragmentBlinnPhong(inputData, surfaceData);

                //---------------------------Halftone
                #if _HALFTONELIGHT
                    //world space normal needed for dot of light intensity
                    float NdotL = dot(normalize(i.normalWS), mainLight.direction);
                    NdotL = NdotL * 0.5 + 0.5;
                    float lightIntensity = NdotL;
                    #if _USESHADOWS
                        lightIntensity *= mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                    #endif
                    lightIntensity = saturate(lightIntensity);
                    //get pattern
                    half halftonePattern = LightingHalftone(lightIntensity, i.screenPos);
                    //get mask if needed
                    half halftoneMask = 0.0h;
                    #if _HALFTONEMASK
                        halftoneMask = SAMPLE_TEXTURE2D(_HalftoneMaskTex, sampler_HalftoneMaskTex, i.uv);
                        #if _HALFTONEMASKINVERT
                            halftoneMask = 1 - halftoneMask;
                        #endif
                    #endif
					baseTexture = lerp(_HalftoneColor, baseTexture, saturate(halftonePattern + halftoneMask));
				#endif
                //------------------------------Shadow and its Pattern
                #if _SHADOWSLIGHT
				    half shadow = saturate((mainLight.shadowAttenuation * mainLight.distanceAttenuation));
				    //manipulate the UV too
				    float2 shadowUV = i.uv * _ShadowFrequency;
                    float shadowPattern = lerp(1.0, SAMPLE_TEXTURE2D(_ShadowTexture, sampler_ShadowTexture, shadowUV).r, 1 - shadow);
                    shadowPattern = smoothstep(0.047,0.7438, shadowPattern);
                    shadowPattern += _ShadowLightness;
                    shadowPattern = saturate(shadowPattern);
					baseTexture *= shadowPattern;
				#endif

                return baseTexture;
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