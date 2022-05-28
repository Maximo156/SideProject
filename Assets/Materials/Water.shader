Shader "Custom/Water"
{
    Properties{

        [Header(Water)]
        _ShallowColor("Shallow Color", Color) = (1,1,1,1)
        _DeepColor("Deep Color", Color) = (1,1,1,1)
        _colorDepthFactor("Color Depth Factor", float) = 1.0

        [Header(Foam)]
        _foamSpeed("Foam Speed", float) = 1.0
        _foamFrequency("Foam Frequency", float) = 1.0
        _foamShallowIntensity("Foam Shallow Intensity", Range(0,1)) = 1.0
        _foamDeepIntensity("Foam Deep Intensity", Range(0,1)) = 1.0
        _foamEdgeIntensity("Foam Edge Intensity", float) = 1.0

        [Header(Waves)]
        _WaveSpeed("Wave Speed", float) = 0.1
        _WaveFrequency("Wave Frequency", float) = 1.0
        _WaveHeight("Wave Height", float) = 0.01
        _Tess("Tesselation", float) = 30

    }
        SubShader{
            Tags {"RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard vertex:vert  alpha:fade

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0
            #include "BiomeNoise.hlsl"


            struct Input {
                float2 uv_MainTex;
                float4 screenPos;
                float eyeDepth;
                float3 worldPos;
                float3 worldRefl;
                float3 worldNormal;
            };

            sampler2D _FoamNoise;

            sampler2D_float _CameraDepthTexture;
            float4 _CameraDepthTexture_TexelSize;

            float3 _ShallowColor;
            float3 _DeepColor;

            float _colorDepthFactor;
            float _foamSpeed;
            float _foamFrequency;
            float _foamShallowIntensity;
            float _foamDeepIntensity;

            float _WaveHeight;
            float _WaveSpeed;
            float _WaveFrequency;

            float _Tess;

            float _foamEdgeIntensity;


            void vert(inout appdata_full v, out Input o)
            {
                v.vertex.z += _WaveHeight  * GetCrests(v.vertex.xy + _Time.y * _WaveSpeed, _WaveFrequency);

                UNITY_INITIALIZE_OUTPUT(Input, o);
                COMPUTE_EYEDEPTH(o.eyeDepth);
            }

            float3 foamNoise(float2 pos) {
                return float3(1, 1, 1)*((GetNoise(pos - _Time.y * _foamSpeed, _foamFrequency) + 1) / 2 + (GetNoise(pos - _Time.y * _foamSpeed, _foamFrequency * 10) + 1) / 2);
            }

            void surf(Input IN, inout SurfaceOutputStandard o) {
                

                float rawZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
                float sceneZ = LinearEyeDepth(rawZ);
                float partZ = IN.eyeDepth;
                float depth = sceneZ - partZ;
                float3 waterColor = lerp(_ShallowColor + _foamShallowIntensity*foamNoise(IN.worldPos.xz), _DeepColor+ _foamDeepIntensity*foamNoise(IN.worldPos.xz), 1 - exp(-depth * _colorDepthFactor));

                float3 finalColor = lerp(foamNoise(IN.worldPos.xz) + (1- foamNoise(IN.worldPos.xz))*waterColor, waterColor, 1 - exp(-depth * _foamEdgeIntensity));

                o.Albedo = finalColor *IN.worldNormal.y;
                o.Alpha = 0.75;
            }
            ENDCG
        }
}
