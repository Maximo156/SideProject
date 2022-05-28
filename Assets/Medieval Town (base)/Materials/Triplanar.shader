Shader "Custom/Triplanar"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Normal ("Normal", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Scale("Scale" , float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _Normal;
        float _Scale;

        struct Input
        {
            float3 worldNormal; INTERNAL_DATA
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;


        half3 triplanar(float3 worldPos, float3 blendAxes, sampler2D text) {

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

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            IN.worldNormal = WorldNormalVector(IN, float3(0, 0, 1));
            // Albedo comes from a texture tinted by color
            o.Albedo = _Color;
            // Metallic and smoothness come from slider variables
            o.Alpha = _Color.a;
            //half3 sandNormal = UnpackNormal(textureNoTile(IN.worldPos.xz, SandNormal, 10));
            //half3 stoneNormal = UnpackNormal(textureNoTile(IN.worldPos.xz, StoneNormal, 10));
            float3 blendAxes = abs(IN.worldNormal);
            blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
            o.Normal = triplanar(IN.worldPos / _Scale, blendAxes, _Normal);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
