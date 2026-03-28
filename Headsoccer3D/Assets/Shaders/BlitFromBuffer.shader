//heavily copied from Blit.hlsl so setup 
//custom modifications are just in returning from the buffer
Shader "TallHorseUtls/BlitFromBuffer"
{
    Properties
    {
        _BufferRed("_BufferRed", float) = 0
        _BufferGreen("_BufferGreen", float) = 0
        _BufferBlue("_BufferBlue", float) = 0
        _BufferAlpha("_BufferAlpha", float) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "BlitFromBuffer"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#pragma vertex Vert
			#pragma fragment Frag

            //For our use
            //StructuredBuffer<float4> Result;
            float _BufferRed;
            float _BufferGreen;
            float _BufferBlue;
            float _BufferAlpha;


            //copy of the Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DynamicScaling.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"

            #ifdef USE_FULL_PRECISION_BLIT_TEXTURE
            TEXTURE2D_X_FLOAT(_BlitTexture);
            #else
            TEXTURE2D_X(_BlitTexture);
            #endif
            TEXTURECUBE(_BlitCubeTexture);

            uniform float4 _BlitScaleBias;
            uniform float4 _BlitScaleBiasRt;
            uniform float4 _BlitTexture_TexelSize;
            uniform float _BlitMipLevel;
            uniform float2 _BlitTextureSize;
            uniform uint _BlitPaddingSize;
            uniform int _BlitTexArraySlice;
            uniform float4 _BlitDecodeInstructions;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord   : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);

                output.positionCS = pos;
                output.texcoord   = DYNAMIC_SCALING_APPLY_SCALEBIAS(uv);

                return output;
            }

            float4 FragBlit(Varyings input, SamplerState s)
            {
                #if defined(USE_TEXTURE2D_X_AS_ARRAY) && defined(BLIT_SINGLE_SLICE)
                    return SAMPLE_TEXTURE2D_ARRAY_LOD(_BlitTexture, s, input.texcoord.xy, _BlitTexArraySlice, _BlitMipLevel);
                #endif

                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, s, input.texcoord.xy, _BlitMipLevel);
            }

            float4 Frag(Varyings i) : SV_Target
            {
                //return FragBlit(i, sampler_PointClamp);
                // Show compute output
                return float4(_BufferRed, _BufferGreen, _BufferBlue, _BufferAlpha);
            }

            ENDHLSL
        }
    }
}
