Shader "PrasinShaders/ToonFlatShadow"
{
	Properties
	{
		_AmbientIntensity ("Ambient Color", Range(0, 1)) = 1
		_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
		_SpecularPower("Specular Power", Float) = 400
		_ToonStep("Toon Step", Range(0, 1)) = 0.5
		_FresnelNumber("Fresnel Power", Range(0, 5)) = 0.2
		_LineDarkness("Line Darkness", Range(0, 1)) = 1

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
			float _AmbientIntensity;
			float4 _BaseColor;
			float _SpecularPower;

			//Shadow
			float4 _ShadowTex_ST;
			TEXTURE2D(_ShadowTex);
			SAMPLER(sampler_ShadowTex);
			float _ShadowFrequency;
			float _LineDarkness;

			//Toon
			float _ToonStep;
			float _FresnelNumber;


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
				//perhaps shadow needs its own uv through TEXSample?
			    return v;
			}

			half4 frag(Varyings i) : SV_Target
			{
				//normalize ofc
				float3 normal = normalize(i.normalWS);
				float3 view = normalize(i.viewWS);
				//SampleSHVertex used to calculate the ambient light flat shader 
				//Here, SampleSH instead because it’s running in the frag
				float3 ambient = SampleSH(i.normalWS) + _AmbientIntensity;
				Light mainLight = GetMainLight(i.shadowCoord);

				//the amount of reflected light depends on the properties of the surface and the angle between the light and the surface
				//dot product is already normalized. negatives are just saying light is not hitting it on the outside surface, so its dark, so 0, so we clamp it betwen 0 and its +ve value
				//The dot product between normal and direction of light gives us the proportion of ambient light acting on the surface
				//therefore: L(diffuse) = clamp01(dotPrduct(normal, lightDir)) and mult by the color
				float diffuse =  max(0, dot(normal, mainLight.direction));

				//uses Blinn's modification to make specular easier to calculate
				//normally, specular, by concept is the dot of viewDir and reflected vector of the main light (ray of reflection) on the surface
				//reflection is too hard to calculate so my boi Jim Blinn came up with alternative
				//we calculate the half vector betwen viewDir and lightDir, and dot product it with NORMAL
				//and raise it to a power for intensity
				//therefore: L(specular) = color * dotPrduct(normal, half)^power
				float3 halfVector = normalize(mainLight.direction + view);
				float specular = max(0, dot(normal, halfVector));
				specular = pow(specular, _SpecularPower);

				//Fresnel 
				//its just invert dot product of view and normal we know that already
				
				float fresnel = 1.0f - max(0, dot(normal, view));
				fresnel = pow(fresnel, _FresnelNumber);


				// final light in grayscale
				float finalLighting = lerp(ambient.r, 1.0, step(_ToonStep, diffuse)) ;

				//manipulate the UV and stylize the crosshatch, mix
				float shadowStylized = SAMPLE_TEXTURE2D(_ShadowTex, sampler_ShadowTex, i.uv * _ShadowFrequency).r;
				shadowStylized = lerp(shadowStylized * 0.1f, min(1, shadowStylized + 1), _LineDarkness);
				float shadowPattern = lerp(1.0, shadowStylized, 1.0 - (specular * mainLight.shadowAttenuation));

				//outline
				float outline = lerp(1.0, shadowStylized, step(0.5, fresnel));

				
				//return step(_StepNumber, fresnel);
				return _BaseColor * finalLighting * shadowPattern * outline;
				//* shadowPattern;
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
	}
}