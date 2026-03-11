Shader "Custom/TerrainHDRP"
{
    Properties
    {
        [HideInInspector] _BaseTextures("Base Textures", 2DArray) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "HDRenderPipeline"
            "RenderType"     = "Opaque"
            "Queue"          = "Geometry"
        }

        // ── ForwardOnly ───────────────────────────────────────────────────────────────
        Pass
        {
            Name "ForwardOnly"
            Tags { "LightMode" = "ForwardOnly" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma target   4.5
            #pragma vertex   Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            static const int   maxLayerCount = 8;
            static const float epsilon       = 1e-4;

            // ── Scalar uniforms in cbuffer (NO arrays, NO int) ────────────────────────
            CBUFFER_START(UnityPerMaterial)
                float _MinHeight;
                float _MaxHeight;
                float _LayerCount;   // float, not int — ints break cbuffer alignment
            CBUFFER_END

            // ── Arrays declared outside cbuffer (set via material.SetFloatArray etc.) ─
            float4 _BaseColours[maxLayerCount];
            float  _BaseStartHeights[maxLayerCount];
            float  _BaseBlends[maxLayerCount];
            float  _BaseColourStrength[maxLayerCount];
            float  _BaseTextureScales[maxLayerCount];

            TEXTURE2D_ARRAY(_BaseTextures);
            SAMPLER(sampler_BaseTextures);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float inverseLerp(float a, float b, float v)
            {
                return saturate((v - a) / (b - a));
            }

            float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int idx)
            {
                float3 p = worldPos / scale;
                return SAMPLE_TEXTURE2D_ARRAY(_BaseTextures, sampler_BaseTextures, float2(p.y, p.z), idx).rgb * blendAxes.x
                     + SAMPLE_TEXTURE2D_ARRAY(_BaseTextures, sampler_BaseTextures, float2(p.x, p.z), idx).rgb * blendAxes.y
                     + SAMPLE_TEXTURE2D_ARRAY(_BaseTextures, sampler_BaseTextures, float2(p.x, p.y), idx).rgb * blendAxes.z;
            }

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                // TransformObjectToWorld returns camera-relative pos in HDRP.
                // GetAbsolutePositionWS converts it to true world space so
                // triplanar UVs and height sampling stay fixed in the world.
                float3 camRelativeWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS  = TransformWorldToHClip(camRelativeWS);
                OUT.worldPos    = GetAbsolutePositionWS(camRelativeWS);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float heightPercent = inverseLerp(_MinHeight, _MaxHeight, IN.worldPos.y);
                float3 normalWS     = normalize(IN.worldNormal);

                float3 blendAxes = abs(normalWS);
                blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

                // DEBUG: uncomment ONE line at a time to diagnose
                // return float4(heightPercent, heightPercent, heightPercent, 1); // should NOT be solid white or black - should vary across terrain
                // return float4(IN.worldPos.y / 100.0, 0, 0, 1);                // red channel = raw world height
                // return float4(_MinHeight / 100.0, _MaxHeight / 100.0, 0, 1);  // rg = min/max heights (should differ)
                // return float4((float)_LayerCount / 8.0, 0, 0, 1);             // red = layer count (should not be black)

                float3 albedo = 0;
                int layerCount = (int)_LayerCount;
                for (int i = 0; i < layerCount; i++)
                {
                    float drawStrength = inverseLerp(
                        -_BaseBlends[i] * 0.5 - epsilon,
                         _BaseBlends[i] * 0.5,
                        heightPercent - _BaseStartHeights[i]);

                    float3 baseColour = _BaseColours[i].rgb * _BaseColourStrength[i];
                    float3 texColour  = triplanar(IN.worldPos, _BaseTextureScales[i], blendAxes, i)
                                       * (1.0 - _BaseColourStrength[i]);

                    albedo = lerp(albedo, baseColour + texColour, drawStrength);
                }

                return float4(albedo, 1.0);
            }
            ENDHLSL
        }

        // ── GBuffer (deferred) ────────────────────────────────────────────────────────
        Pass
        {
            Name "GBuffer"
            Tags { "LightMode" = "GBuffer" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma target   4.5
            #pragma vertex   Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            static const int   maxLayerCount = 8;
            static const float epsilon       = 1e-4;

            CBUFFER_START(UnityPerMaterial)
                float _MinHeight;
                float _MaxHeight;
                float _LayerCount;
            CBUFFER_END

            float4 _BaseColours[maxLayerCount];
            float  _BaseStartHeights[maxLayerCount];
            float  _BaseBlends[maxLayerCount];
            float  _BaseColourStrength[maxLayerCount];
            float  _BaseTextureScales[maxLayerCount];

            TEXTURE2D_ARRAY(_BaseTextures);
            SAMPLER(sampler_BaseTextures);

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Varyings   { float4 positionCS : SV_POSITION; float3 worldPos : TEXCOORD0; float3 worldNormal : TEXCOORD1; UNITY_VERTEX_INPUT_INSTANCE_ID };

            float inverseLerp(float a, float b, float v) { return saturate((v-a)/(b-a)); }

            float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int idx)
            {
                float3 p = worldPos / scale;
                return SAMPLE_TEXTURE2D_ARRAY(_BaseTextures, sampler_BaseTextures, float2(p.y,p.z), idx).rgb * blendAxes.x
                     + SAMPLE_TEXTURE2D_ARRAY(_BaseTextures, sampler_BaseTextures, float2(p.x,p.z), idx).rgb * blendAxes.y
                     + SAMPLE_TEXTURE2D_ARRAY(_BaseTextures, sampler_BaseTextures, float2(p.x,p.y), idx).rgb * blendAxes.z;
            }

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                float3 camRelativeWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS  = TransformWorldToHClip(camRelativeWS);
                OUT.worldPos    = GetAbsolutePositionWS(camRelativeWS);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            struct GBufferOut
            {
                float4 GBuffer0 : SV_Target0;
                float4 GBuffer1 : SV_Target1;
                float4 GBuffer2 : SV_Target2;
                float4 GBuffer3 : SV_Target3;
            };

            GBufferOut Frag(Varyings IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float heightPercent = inverseLerp(_MinHeight, _MaxHeight, IN.worldPos.y);
                float3 blendAxes = abs(normalize(IN.worldNormal));
                blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

                float3 albedo = 0;
                int layerCount = (int)_LayerCount;
                for (int i = 0; i < layerCount; i++)
                {
                    float ds = inverseLerp(-_BaseBlends[i]*0.5 - epsilon, _BaseBlends[i]*0.5,
                                            heightPercent - _BaseStartHeights[i]);
                    float3 col = _BaseColours[i].rgb * _BaseColourStrength[i]
                               + triplanar(IN.worldPos, _BaseTextureScales[i], blendAxes, i)
                                 * (1.0 - _BaseColourStrength[i]);
                    albedo = lerp(albedo, col, ds);
                }

                float3 normalWS = normalize(IN.worldNormal) * 0.5 + 0.5;

                GBufferOut o;
                o.GBuffer0 = float4(albedo, 1.0);
                o.GBuffer1 = float4(normalWS, 1.0);
                o.GBuffer2 = float4(0, 1.0, 0, 0);
                o.GBuffer3 = float4(0, 0, 0, 0);
                return o;
            }
            ENDHLSL
        }

        // ── ShadowCaster ──────────────────────────────────────────────────────────────
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma target   4.5
            #pragma vertex   Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _MinHeight;
                float _MaxHeight;
                float _LayerCount;
            CBUFFER_END

            // Arrays must also be declared here to keep cbuffer layout consistent
            float4 _BaseColours[8];
            float  _BaseStartHeights[8];
            float  _BaseBlends[8];
            float  _BaseColourStrength[8];
            float  _BaseTextureScales[8];

            struct Attributes { float4 positionOS : POSITION; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Varyings   { float4 positionCS : SV_POSITION; };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                float4 posCS    = TransformWorldToHClip(worldPos);
                posCS.z += 0.001 * posCS.w;
                OUT.positionCS  = posCS;
                return OUT;
            }
            float4 Frag(Varyings IN) : SV_Target { return 0; }
            ENDHLSL
        }

        // ── DepthOnly ─────────────────────────────────────────────────────────────────
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask R
            Cull Back

            HLSLPROGRAM
            #pragma target   4.5
            #pragma vertex   Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _MinHeight;
                float _MaxHeight;
                float _LayerCount;
            CBUFFER_END

            float4 _BaseColours[8];
            float  _BaseStartHeights[8];
            float  _BaseBlends[8];
            float  _BaseColourStrength[8];
            float  _BaseTextureScales[8];

            struct Attributes { float4 positionOS : POSITION; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Varyings   { float4 positionCS : SV_POSITION; };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                OUT.positionCS = TransformWorldToHClip(TransformObjectToWorld(IN.positionOS.xyz));
                return OUT;
            }
            float4 Frag(Varyings IN) : SV_Target { return 0; }
            ENDHLSL
        }
    }

    FallBack "HDRP/Lit"
}
