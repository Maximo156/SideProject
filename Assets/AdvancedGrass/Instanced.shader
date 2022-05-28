Shader "Instanced" {
    Properties{
        _MainTex("Albedo (RGBA)", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.7
    }
        SubShader{
             Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 200
            Cull Off
            CGPROGRAM
            // Physically based Standard lighting model
            #pragma surface surf Standard addshadow fullforwardshadows alphatest:_Cutoff 
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

            sampler2D _MainTex;

            struct Input {
                float2 uv_MainTex;
            };

            float _scale = 1;

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            StructuredBuffer<float3> positionBuffer;
        #endif


            void setup()
            {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                float3 data = positionBuffer[unity_InstanceID];

                //float rotation = data.w * data.w * _Time.y * 0.5f;
                //rotate2D(data.xz, rotation);

                unity_ObjectToWorld._11_21_31_41 = float4(_scale, 0, 0, 0);
                unity_ObjectToWorld._12_22_32_42 = float4(0, _scale, 0, 0);
                unity_ObjectToWorld._13_23_33_43 = float4(0, 0, _scale, 0);
                unity_ObjectToWorld._14_24_34_44 = float4(data.xyz, 1);
                unity_WorldToObject = unity_ObjectToWorld;
                unity_WorldToObject._14_24_34 *= -1;
                unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
            #endif
            }

            half _Glossiness;
            half _Metallic;
            float4 _Color;

            void surf(Input IN, inout SurfaceOutputStandard o) {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = c.rgb;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
            ENDCG
        }
        FallBack "Transparent"
}