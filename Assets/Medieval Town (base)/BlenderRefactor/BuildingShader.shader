Shader "Custom/BuildingShader"
{
    Properties
    {
        _Colors("Colors", 2D) = "white" {}

        _StoneNormal("Stone Normal", 2D) = "white" {}

        _BrickNormal("Brick Normal", 2D) = "white" {}

        _WoodNormal("Wood Normal", 2D) = "white" {}

        
        _StoneScale("Stone Scale" , float) = 1
        _BrickScale("Brick Scale" , float) = 1
        _WoodScale("Wood Scale" , float) = 1

        [Header (Moss)]
        _Moss("Moss Texture", 2D) = "white" {}
        _MossScale("MossScale" , float) = 1
        _MossTextureScale("MossTextureScale" , float) = 1
        _MossStrength("MossStrength" , float) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0
            #include "Assets/Materials/BiomeNoise.hlsl"

            sampler2D _Colors;

            sampler2D _StoneNormal;
            float _StoneScale;

            sampler2D _BrickNormal;
            float _BrickScale;

            sampler2D _WoodNormal;
            float _WoodScale;


            sampler2D _Moss;
            float _MossScale;
            float _MossTextureScale;
            float _MossStrength;

            struct Input
            {
                float3 worldNormal; INTERNAL_DATA
                float3 worldPos;
                float2 uv_MainTex;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            half3 triplanar(float3 worldPos, float3 blendAxes, sampler2D text) {

                float3 xProj = (tex2D(text, worldPos.zy)) * blendAxes.x;
                float3 yProj = (tex2D(text, worldPos.xz)) * blendAxes.y;
                float3 zProj = (tex2D(text, worldPos.xy)) * blendAxes.z;

                return xProj + yProj + zProj;
            }

            half3 triplanarNormal(float3 worldPos, float3 blendAxes, sampler2D text) {

                float3 xProj = UnpackNormal(tex2D(text, worldPos.zy)) * blendAxes.x;
                float3 yProj = UnpackNormal(tex2D(text, worldPos.xz)) * blendAxes.y;
                float3 zProj = UnpackNormal(tex2D(text, worldPos.xy)) * blendAxes.z;

                return xProj + yProj + zProj;
            }

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                IN.worldNormal = WorldNormalVector(IN, float3(0, 0, 1));
                float3 blendAxes = abs(IN.worldNormal);
                float2 uv = IN.uv_MainTex;

                blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
                // Albedo comes from a texture tinted by color

                float mossStrengthX = GetNoise(IN.worldPos.zy, _MossScale) * _MossStrength * blendAxes.x;
                float mossStrengthY = GetNoise(IN.worldPos.xz, _MossScale) * _MossStrength * blendAxes.y;
                float mossStrengthZ = GetNoise(IN.worldPos.xy, _MossScale) * _MossStrength * blendAxes.x;
                float mossStrength = mossStrengthX + mossStrengthY + mossStrengthZ;

                float3 moss = triplanar(IN.worldPos / _MossTextureScale, blendAxes, _Moss);


                mossStrength = max(0, min(1, mossStrength));
                o.Albedo = ((1 - mossStrength) * tex2D(_Colors, uv)) + (mossStrength * moss);
                o.Alpha = _Color.a;
                

                float2 n = uv;


                if (n.x < 0.33) {
                    if (n.y < 0.33) {
                          o.Normal = (1 - pow(mossStrength, 0.5)) * triplanarNormal(IN.worldPos / _WoodScale, blendAxes, _WoodNormal) + pow(mossStrength, 0.5) * half3(0, 0, 1);
                    }
                    else if (n.y < 0.66) {
                        o.Normal = (1 - pow(mossStrength, 0.5)) * triplanarNormal(IN.worldPos / _StoneScale, blendAxes, _StoneNormal) + pow(mossStrength, 0.5) * half3(0, 0, 1);
                    }
                    else {
                        o.Normal = (1 - pow(mossStrength, 0.5)) * triplanarNormal(IN.worldPos / _BrickScale, blendAxes, _BrickNormal) + pow(mossStrength, 0.5) * half3(0, 0, 1);
                    }
                }

                
            }
            ENDCG
        }
            FallBack "Diffuse"
}
