Shader "Saphead Studios/Principle Parallax"
{
    
	Properties
	{
		_BaseMap("Base Map", 2D) = "white" {}
		_BaseColor1("Base Color 1", Color) = (1, 1, 1, 1)
		_BaseColor2("Base Color 2", Color) = (1, 1, 1, 1)

		[Space(20)]
		_OverlayMap("Overlay Map", 2D) = "white" {}
		_OverlayStrength("Overlay Strength", Range(0,2)) = 1


		[Space(20)]
		[NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Bump Scale", Float) = 1

		//Parallax
		[Space(20)]
		_Depth("Parallax Unit Depth", float) = -0.4
		_Screen_Mix("Screen Mix", Range(0, 1)) = 1
		_Parallax_Layers("Parallax Layers", int) = 1 

		[Space(20)]
		_FloatStrength("General Floating", Range(0,0.5)) = 0.4
		_FloatAmplitude("Floating Dist", Range(0,0.5)) = 0.4


	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

		HLSLINCLUDE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
		ENDHLSL

		Pass
		{
			HLSLPROGRAM

			#include "Assets/Shaders/ShaderUtilityNoise.hlsl"
			#include "Assets/Shaders/IceParallaxInitData.hlsl"


			#pragma vertex vert
			#pragma fragment frag

			Varyings vert(Attributes i)
			{
				Varyings v;

				VertexPositionInputs positionInputs = GetVertexPositionInputs(i.positionOS.xyz);

				//Vertex animation
				//https://discussions.unity.com/t/shader-get-object-position-or-distinct-value-per-object/31873
				float3 objectWS = GetObjectToWorldMatrix()._m03_m13_m23;
				objectWS.z += _Time.x * _FloatStrength;
				float noise = PerlinNoise2d(objectWS.xz, 5);
				positionInputs.positionWS.y += noise * _FloatAmplitude; 

				VertexNormalInputs normalInputs = GetVertexNormalInputs(i.normalOS.xyz, i.tangentOS);

				v.positionCS = TransformWorldToHClip(positionInputs.positionWS);
				v.positionWS = positionInputs.positionWS;

				half3 viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
				half3 vertexLight = VertexLighting(positionInputs.positionWS, normalInputs.normalWS);
				
				v.normalWS = half4(normalInputs.normalWS, viewDirWS.x);
				v.tangentWS = half4(normalInputs.tangentWS, viewDirWS.y);
				v.bitangentWS = half4(normalInputs.bitangentWS, viewDirWS.z);

				OUTPUT_LIGHTMAP_UV(i.lightmapUV, unity_LightmapST, v.lightmapUV);
				OUTPUT_SH(v.normalWS.xyz, v.vertexSH);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					v.shadowCoord = GetShadowCoord(positionInputs);
				#endif

				v.uv = TRANSFORM_TEX(i.uv, _BaseMap);
				return v;
			}

			half4 frag(Varyings i) : SV_Target
			{
				SurfaceData surfaceData;
				InitializeSurfaceData(i, surfaceData);
				InputData inputData;
				InitializeInputData(i, surfaceData.normalTS, inputData);
				
				half4 color; 
				
				color = UniversalFragmentBlinnPhong(inputData, surfaceData);

				return color;
			}
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
    FallBack "Hidden/Shader Graph/FallbackError"
}
