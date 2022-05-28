Shader "Custom/TerrainShader"
{
    Properties
    {
        [Header(Textures)]
        GrassTexture("GrassTexture", 2D) = "white"
        StonyGrassTexture("StonyGrassTexture", 2D) = "white"
        Rocks1("Rocks1", 2D) = "white"
        Snow("Snow", 2D) = "white"
        Sand("Sand", 2D) = "white"

        [Header(Normals)]
        _NormalScale("Normal Scale", float) = 20
        _NormalInt("Normal Intensity", float) = 0.5
        GrassNormal("Grass Normal", 2D) = "white"
        DirtNormal("Dirt Normal", 2D) = "white"
        RockNormal("Rock Normal", 2D) = "white"
        SnowNormal("Snow Normal", 2D) = "white"
        SandNormal("Sand Normal", 2D) = "white"
        [Header(Parameters)]
        _SlopeLimit("Dirt Slope", float) = 0.5
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

        #include "BiomeNoise.hlsl"
        
        sampler2D GrassTexture;
        sampler2D StonyGrassTexture;
        sampler2D Rocks1;
        sampler2D Snow;
        sampler2D Sand;

        sampler2D GrassNormal;
        sampler2D DirtNormal;
        sampler2D StoneNormal;
        sampler2D SnowNormal;
        sampler2D SandNormal;

        float _NormalScale;
        float _NormalInt;

        float2 playerPos;
        sampler2D biomeInfo;
        int textWidth;

        float  _SlopeLimit;

        sampler2D textArray[6];
        float noiseScale;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal; INTERNAL_DATA
        };

        const static int maxColorCount = 8;

        int mountainHeightsCount;
        
        float3 colors[maxColorCount];
        float heights[maxColorCount];
        float scales[maxColorCount];
        float biomeTextureCounts[maxColorCount];

        UNITY_DECLARE_TEX2DARRAY(textures);

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float blend(float from, float to, float value, int strength){
            if (value <= from)
                return 1;
            if (value >= to)
                return 0;

            float domain = (value - from) / (to - from);
            
            return 1/(1+pow(domain/(1-domain), strength));
        }

        float hash4(float2 p) {
            return frac(sin(float4(1.0 + dot(p, float2(37.0, 17.0)),
                2.0 + dot(p, float2(11.0, 47.0)),
                3.0 + dot(p, float2(41.0, 29.0)),
                4.0 + dot(p, float2(23.0, 31.0)))) * 103.0).x;
        }

        half4 textureNoTile(float2 pos, sampler2D text, float scale) {


            float RotateTileSize = 20;

            float2 p = floor(pos / RotateTileSize);
            float2 f = frac(pos / RotateTileSize);



            float angle = hash4(p) * 2 * 3.1415;

            //angle = _Time;
            angle = angle + acos(f.x / length(f));

            float2 rotPoint = (float2(sin(angle) * length(f), cos(angle) * length(f)) + p) / float2(scale / RotateTileSize, scale / RotateTileSize);

            return tex2D(text, rotPoint);
            // normalization
            //return UNITY_SAMPLE_TEX2DARRAY(textures, float3(rotPoint.x, rotPoint.y, 0));;
        }

        float3 triplanar(float3 worldPos, float3 blendAxes, sampler2D text, float scale){
           
            
            /*
            float3 xProj = UNITY_SAMPLE_TEX2DARRAY(textures, float3(scaledPos.y, scaledPos.z, textureIndex)) * blendAxes.x;
            float3 yProj = UNITY_SAMPLE_TEX2DARRAY(textures, float3(scaledPos.x, scaledPos.z, textureIndex)) * blendAxes.y;
            float3 zProj = UNITY_SAMPLE_TEX2DARRAY(textures, float3(scaledPos.x, scaledPos.y, textureIndex)) * blendAxes.z;
            */

            float3 xProj = textureNoTile(float2(worldPos.y, worldPos.z), text, scale) * blendAxes.x;
            float3 yProj = textureNoTile(float2(worldPos.x, worldPos.z), text, scale) * blendAxes.y;
            float3 zProj = textureNoTile(float2(worldPos.x, worldPos.y), text, scale) * blendAxes.z;

            return xProj+yProj+zProj;
        }

        float3 hillsTexture(float3 worldPos, float3 blendAxes) {
            float noise = (GetNoise(worldPos.xz + float2(1000, 2301), 1000)) / 2 + 0.5;
            float3 dirt = triplanar(worldPos, blendAxes, StonyGrassTexture, scales[1]);
            float3 grass = triplanar(worldPos, blendAxes, GrassTexture, scales[0]) * 0.8 + float3(0, 0.8, 0) * 0.2;

            float3 dirtyGrass = grass * 0.6 + (noise * grass + (1 - noise) * dirt) * 0.4;

            float dirtStrength = blend(0, _SlopeLimit, blendAxes.y, 5);


            return (dirt * dirtStrength + (1 - dirtStrength) * grass) ;
        }

        float3 textByHeight(float3 worldPos, float3 normal, float3 blendAxes) {
            float blendRange = 100;
            float height = worldPos.y;

            float3 grass = hillsTexture(worldPos, blendAxes);
            float3 sGrass = triplanar(worldPos, blendAxes, StonyGrassTexture, scales[1]);
            float3 r1 = triplanar(worldPos, blendAxes, Rocks1, scales[2]);
            float3 r2Snow = (1 - blendAxes.y) * triplanar(worldPos, blendAxes, Rocks1, scales[3]) * 1.2 + (blendAxes.y) * triplanar(worldPos, blendAxes, Snow, scales[4]) * 0.8;
            
            float grassWeight = saturate(blend(heights[0] - blendRange, heights[0] + blendRange, height, 1));
            float sGrassWeight = (1- grassWeight)*saturate(blend(heights[1] - blendRange, heights[1] + blendRange, height, 1));
            float r1Weight = (1 - grassWeight- sGrassWeight) * saturate(blend(heights[2] - blendRange, heights[2] + blendRange, height, 1));
            float r2SnowWeight = (1 - (grassWeight + sGrassWeight + r1Weight));
            return (grass * grassWeight + sGrass * sGrassWeight+ r1Weight * r1 + r2Snow * r2SnowWeight);

        }


        float3 mountainTexture(float3 worldPos, float3 worldNormal,float3 blendAxes ){
            float blendRange = 100;
            worldPos.y = worldPos.y+GetNoise(worldPos.xz, 100)*10;
            
            return textByHeight(worldPos, worldNormal, blendAxes);
        }

       
        float3 desertTexture(float3 worldPos, float3 blendAxes){
            float noise = (GetNoise(worldPos.xz+float2(1000, 2301), 1000))/2+0.5;
            return triplanar(worldPos, blendAxes, Sand, 50);
        }


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            IN.worldNormal = WorldNormalVector(IN, float3(0, 0, 1));
            int oceanHeight = 100;
            float ranges[3] = { -0.33, 0.33, 1 };
            float blendRange = 0.1;

            float2 pos = IN.worldPos.xz;
            float2 textPos = mul(float2x2(0, 1, 1, 0), (IN.worldPos.xz - playerPos + (0.5 * textWidth)) / textWidth);
            float biomeBlendNoise = tex2D(biomeInfo, textPos) * 2 - 1;


            float worldNoise = 10*((GetNoise(IN.worldPos.xz+float2(1000, 2301), 500))/2+0.5);
            float3 noisedPos = IN.worldPos + float3(worldNoise, worldNoise, worldNoise);
            float3 blendAxes = abs(IN.worldNormal);
            blendAxes /= blendAxes.x+ blendAxes.y +blendAxes.z;
            

            float xEnd = ranges[0]-blendRange;
            float xyBlend = ranges[0]+blendRange;
            float yEnd = ranges[1]-blendRange;
            float yzBlend = ranges[1]+blendRange;

            float3 desert = desertTexture(noisedPos, blendAxes);
            float3 hills = hillsTexture(noisedPos, blendAxes);
            float3  mountians = mountainTexture(noisedPos, IN.worldNormal, blendAxes);


            float DesertWeight = saturate(blend(xEnd, xyBlend, biomeBlendNoise,10 ));
            float HillsWeight = (1 - DesertWeight) * saturate(blend(yEnd, yzBlend, biomeBlendNoise,10));
            float MountainsWeight = (1 - (DesertWeight + HillsWeight));

            float oceanWeight = blend(oceanHeight-5, oceanHeight + 5, IN.worldPos.y, 5);


            o.Albedo = (desert * DesertWeight + hills * HillsWeight +MountainsWeight * mountians)*(1-oceanWeight) + (desert) * oceanWeight;

            half3 grassNormal = UnpackScaleNormal(textureNoTile(IN.worldPos.xz, DirtNormal, _NormalScale), _NormalInt);
            //half3 sandNormal = UnpackNormal(textureNoTile(IN.worldPos.xz, SandNormal, 10));
            //half3 stoneNormal = UnpackNormal(textureNoTile(IN.worldPos.xz, StoneNormal, 10));

            o.Normal = grassNormal;//(sandNormal * DesertWeight + grassNormal * HillsWeight + MountainsWeight * stoneNormal)* (1 - oceanWeight) + (sandNormal)*oceanWeight;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
