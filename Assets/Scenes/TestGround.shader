Shader "PFA/TestGround"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 positionWS : TEXCOORD0; };
            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                return output;
            }
            half4 Frag(Varyings input) : SV_Target
            {
                float checker = frac((floor(input.positionWS.x) + floor(input.positionWS.z)) * 0.5) * 2;
                return half4(lerp(half3(0.32, 0.36, 0.4), half3(0.45, 0.49, 0.53), checker), 1);
            }
            ENDHLSL
        }
    }
}
