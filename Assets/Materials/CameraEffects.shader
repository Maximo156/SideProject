
Shader "Custom/CameraEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor("Fog Color", Color) = (1,1,1,1)
        _FogExp("Fog Strength", Float) = 1
        _FogAngle("Fog Angle Limit", Float) = 1.570796325
    }
        SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Common.hlsl"

            sampler2D_float _CameraDepthTexture;
            float4 _FogColor;
            float _FogExp;
            float _FogAngle;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD0;
                float3 camRelativeWorldPos : TEXCOORD1;
            };


            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.projPos = ComputeScreenPos(o.pos);
                o.camRelativeWorldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz - _WorldSpaceCameraPos;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 screenUV = i.projPos.xy / i.projPos.w;

                // sample depth texture
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV);


                float depthPow = pow(Linear01Depth(depth), _FogExp)*1.5;

                float sceneZ = LinearEyeDepth(depth);
                float3 viewPlane = i.camRelativeWorldPos.xyz / dot(i.camRelativeWorldPos.xyz, unity_WorldToCamera._m20_m21_m22);
                float3 worldPos = viewPlane * sceneZ+_WorldSpaceCameraPos;
                worldPos = mul(unity_CameraToWorld, float4(worldPos, 1.0));

                //float ang = abs(asin((pos.y - _WorldSpaceCameraPos.y) / dist))/ 1.570796325;
                

                fixed4 col = tex2D(_MainTex, screenUV);

                if (worldPos.y < _FogAngle) {
                    col = (1 - depthPow)* col + depthPow * _FogColor;
                }
                // just invert the colors
                return col;
            }
            ENDCG
        }
    }
}
