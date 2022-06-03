Shader "Custom/FireShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)

        [Header(Wind)]
        _WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
        _WindStrength("Wind Strength", Float) = 1
        _WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
    }
        SubShader
        {
            Tags { "RenderType" = "Unlit" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Lambert  vertex:vert 

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0
            sampler2D _WindDistortionMap;
            float4 _WindDistortionMap_ST;

            float _WindStrength;
            float2 _WindFrequency;


            struct Input
            {
                float2 uv_MainTex;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;



            void vert(inout appdata_full v) {

                float2 uv = v.vertex.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
                float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
                float3 wind = float3(windSample.x, 0, windSample.y);

                v.vertex.x += windSample.x;
                v.vertex.y += windSample.y;
            }

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf(Input IN, inout SurfaceOutput  o)
            {
                // Albedo comes from a texture tinted by color
                o.Albedo = _Color;
                o.Emission = _Color;
            }
            ENDCG
        }
        FallBack "Diffuse"
}
