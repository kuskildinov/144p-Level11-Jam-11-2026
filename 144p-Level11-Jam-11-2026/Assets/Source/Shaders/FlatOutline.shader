
Shader "IcePiecesGames/Outline/FlatOutline"
{
    Properties
    {
        [HDR] _OutlineColor("Outline Color", Color) =
            (1, 1, 1, 1)

        _OutlineWidth("Outline Width", Range(0.001, 0.1)) =
            0.015

        _Center("Mesh Center XY", Vector) =
            (0, 0, 0, 0)

        _HalfSize("Mesh Half Size XY", Vector) =
            (0.5, 0.5, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent+10"
        }

        Pass
        {
            Name "FlatOutline"

            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }

            Cull Off
            ZWrite Off
            ZTest LEqual

            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineWidth;
                float4 _Center;
                float4 _HalfSize;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                float3 positionOS = input.positionOS.xyz;

                // Расширяем прямоугольник наружу.
                float2 direction = sign(
                    positionOS.xy - _Center.xy
                );

                positionOS.xy += direction * _OutlineWidth;

                output.positionCS =
                    TransformObjectToHClip(positionOS);

                output.uv = input.uv;

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                // Определяем, насколько далеко пиксель
                // находится от центра прямоугольника.
                float2 centeredUV = abs(input.uv - 0.5);

                // Ширина рамки в UV-координатах.
                float2 borderUV = float2(
                    _OutlineWidth /
                    max(_HalfSize.x * 2.0, 0.0001),

                    _OutlineWidth /
                    max(_HalfSize.y * 2.0, 0.0001)
                );

                float2 innerLimit =
                    0.5 - borderUV;

                float border =
                    step(innerLimit.x, centeredUV.x) +
                    step(innerLimit.y, centeredUV.y);

                clip(border - 0.5);

                return _OutlineColor;
            }

            ENDHLSL
        }
    }
}
