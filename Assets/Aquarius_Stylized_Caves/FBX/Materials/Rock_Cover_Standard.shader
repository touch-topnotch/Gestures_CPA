Shader "AquariusMax/Rock Cover Top URP"
{
    Properties{
        [Header(Rock Setting)]
        _MainColor("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}
        [NoScaleOffset]_NormalTex("Normal", 2D) = "bump" {}
        _NormalPower("Normal Power",Range(0,5)) = 1
        [NoScaleOffset]_OcclusionMap("Occlusion (AO)", 2D) = "white" {}
        _OcclusionStrength("Occlusion Strength",Range(0,1)) = 0.5
        [NoScaleOffset] _MetallicTex("Metallic Map", 2D) = "black" {}
        _Metallic("Metallic", Range(0, 1)) = 0.5
        _Smoothness("Smoothness ",Range(0,1)) = 0.5
        _MainCon("Contrast", Range(0,4)) = 1.0
        _MainBri("Brightness", Range(0,4)) = 1.0

        [Space(8)][Header(Cover Setting)]
        _TopTex("Top Cover Albedo", 2D) = "gray" {}
        [NoScaleOffset][HideInInspector]_TopNormalTex("Top Cover Normal", 2D) = "bump" {}
        _CoverageAmount("Top Coverage Amount", Range(-1 , 1)) = 0
        _CoverageFallOff("Top Coverage FallOff", Range(0 , 10)) = 6
        _TriplanarContrast("Top Triplanar Contrast", Range(0.1,4)) = 4

        [HideInInspector] _texcoord("", 2D) = "white" {}
    }

    SubShader{
        Tags{ "RenderType"="Opaque" "Queue"="Geometry+0" }
        LOD 200

        Pass {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _NORMALMAP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            sampler2D _MainTex, _NormalTex, _OcclusionMap, _MetallicTex, _TopTex;
            float4 _MainColor;
            float4 _MainTex_ST;
            float4 _TopTex_ST;
            float _NormalPower;
            float _OcclusionStrength;
            float _Metallic;
            float _Smoothness;
            float _MainCon;
            float _MainBri;
            float _TriplanarContrast;
            float _CoverageAmount;
            float _CoverageFallOff;

            Varyings vert(Attributes input) {
                Varyings output;
                // Преобразуем позицию в HClip явно через float4
                output.positionHCS = mul(UNITY_MATRIX_MVP, float4(input.positionOS.xyz, 1.0)); 
                output.worldPos = mul(unity_ObjectToWorld, float4(input.positionOS.xyz, 1.0)).xyz;
                output.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, input.normalOS));
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half3 blend_rnm(half3 n1, half3 n2) {
                n1.z += 1;
                n2.xy = -n2.xy;
                return n1 * dot(n1, n2) / n1.z - n2;
            }

            half3 DoTriplanarNormal(sampler2D bump, float4 scaleOffset, float3 worldPos, float3 worldNormal, float triplanarContrast) {
                half3 n = abs(worldNormal);
                n = normalize(max(n, 0.00001)); // Минимальное значение для безопасности
                half3 triblend = pow(n, triplanarContrast);
                triblend = triblend / (triblend.x + triblend.y + triblend.z);

                float2 uvX = worldPos.zy * scaleOffset.xy + scaleOffset.zw;
                float2 uvY = worldPos.xz * scaleOffset.xy + scaleOffset.zw;
                float2 uvZ = worldPos.xy * scaleOffset.xy + scaleOffset.zw;

                half3 axisSign = worldNormal < 0 ? -1 : 1;

                half3 tnormalX = UnpackNormal(tex2D(bump, uvX));
                half3 tnormalY = UnpackNormal(tex2D(bump, uvY));
                half3 tnormalZ = UnpackNormal(tex2D(bump, uvZ));

                tnormalX.x *= axisSign.x;
                tnormalY.x *= axisSign.y;
                tnormalZ.x *= -axisSign.z;

                tnormalX = blend_rnm(half3(worldNormal.zy, n.x), tnormalX);
                tnormalY = blend_rnm(half3(worldNormal.xz, n.y), tnormalY);
                tnormalZ = blend_rnm(half3(worldNormal.xy, n.z), tnormalZ);

                tnormalX.z *= axisSign.x;
                tnormalY.z *= axisSign.y;
                tnormalZ.z *= axisSign.z;

                half3 normal = normalize(
                    tnormalX.zyx * triblend.x +
                    tnormalY.xzy * triblend.y +
                    tnormalZ.xyz * triblend.z
                );
                return normal;
            }

            half4 frag(Varyings input) : SV_Target {
                half4 color = tex2D(_MainTex, input.uv) * _MainColor;

                half4 top = tex2D(_TopTex, input.uv);
                // Работаем с pow безопасно
                float coverPower = pow(saturate(input.worldNormal.y + _CoverageAmount), 10.3 - _CoverageFallOff);
                coverPower = max(coverPower, 0.0); // Защита от отрицательных значений

                color = lerp(color, top, coverPower);

                color.rgb = pow(color.rgb, _MainCon) * _MainBri;

                half3 texNormal = UnpackNormal(tex2D(_NormalTex, input.uv));
                texNormal.xy *= _NormalPower;

                half ao = lerp(1, tex2D(_OcclusionMap, input.uv).g, _OcclusionStrength);

                float metallic = tex2D(_MetallicTex, input.uv).r * _Metallic;

                return half4(color.rgb * ao, color.a);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
