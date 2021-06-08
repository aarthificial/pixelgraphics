Shader "Hidden/PixelGraphics/Velocity/Blit"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment Fragment
            #pragma multi_compile_fragment _ _LINEAR_TO_SRGB_CONVERSION
            #pragma multi_compile _ _USE_DRAW_PROCEDURAL

            #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Fullscreen.hlsl"
            #include "Packages/com.aarthificial.pixelgraphics/Runtime/Shaders/VelocitySimulation.hlsl"

            float4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                return SimulateVelocity(input.uv);
            }
            ENDHLSL
        }
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment Fragment
            #pragma multi_compile_fragment _ _LINEAR_TO_SRGB_CONVERSION
            #pragma multi_compile _ _USE_DRAW_PROCEDURAL

            #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Fullscreen.hlsl"
            #include "Packages/com.aarthificial.pixelgraphics/Runtime/Shaders/VelocitySimulation.hlsl"

            float4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 result = SimulateVelocity(input.uv);

                float2 temporaryVelocity = SAMPLE_TEXTURE2D(_PG_TemporaryVelocityTexture, sampler_PG_TemporaryVelocityTexture, input.uv).zw;
                if (abs(temporaryVelocity.x) > 0)
                    result.z = temporaryVelocity.x;
                if (abs(temporaryVelocity.y) > 0)
                    result.w = temporaryVelocity.y;

                return result;
            }
            ENDHLSL
        }
    }
}