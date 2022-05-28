using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightNoise
{
    private static FastNoiseLite mountainNoise = new FastNoiseLite(678);
    private static FastNoiseLite eleNoise = new FastNoiseLite(345);
    private static FastNoiseLite hillsNoise = new FastNoiseLite(12736);
    private static FastNoiseLite mountainFractalNoise = new FastNoiseLite(12736);
    private static FastNoiseLite flatNoise = new FastNoiseLite(12736);
    private static FastNoiseLite desertNoise = new FastNoiseLite(5675);
    private static FastNoiseLite biomeBlendNoise = new FastNoiseLite(5675);

    private static void MakeBiomeBlendNoise()
    {
        biomeBlendNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        biomeBlendNoise.SetFrequency(0.0001f);

    }

    private static void MakeMountainNoise()
    {
        mountainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        mountainNoise.SetFrequency(0.0001f);
    }

    private static void OldMakeMountainNoise()
    {
        mountainNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        mountainNoise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.EuclideanSq);
        mountainNoise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance);
        mountainNoise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2Reduced);
        mountainNoise.SetFractalType(FastNoiseLite.FractalType.DomainWarpProgressive);
        mountainNoise.SetDomainWarpAmp(90f);
        mountainNoise.SetFractalLacunarity(1.1f);
        mountainNoise.SetFractalOctaves(6);
        mountainNoise.SetFractalGain(1.6f);
    }

    private static void MakeEleNoise()
    {
        eleNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        eleNoise.SetFrequency(0.0001f);
    }

    private static void MakeHillsNoise()
    {
        hillsNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        hillsNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
        hillsNoise.SetFractalOctaves(4);
        hillsNoise.SetFrequency(0.0005f);
    }

    private static void MakeMountainFractalNoise()
    {
        mountainFractalNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        mountainFractalNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
        mountainFractalNoise.SetFractalOctaves(4);
        mountainFractalNoise.SetFrequency(0.001f);
    }

    private static void MakeDesertNoise()
    {
        desertNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

        desertNoise.SetFrequency(0.001f);
    }

    static HeightNoise()
    {
        MakeMountainNoise();
        MakeEleNoise();
        MakeHillsNoise();
        MakeDesertNoise();
        MakeBiomeBlendNoise();
        MakeMountainFractalNoise();
    }



    private static float getBiomeBlendNoise(float x, float z)
    {
        return biomeBlendNoise.GetNoise(x, z);
    }


    private static float getDesertNoise(float x, float z)
    {
        float Aamplitude = .08f;
        float Camplitude = .030f;
        float noiseA = -Mathf.Abs(desertNoise.GetNoise(x, z)) + 1;
        float noiseC = desertNoise.GetNoise(x * 3, z * 3) / 2 + 0.5f;

        return (noiseA * Aamplitude + noiseC * Camplitude) * 0.9f;
    }

    private static float getEleNoise(float x, float z)
    {
        return (eleNoise.GetNoise(x, z) + 1) / 2;
    }

    private static float getLandNoise(float x, float z)
    {
        return (eleNoise.GetNoise(x+102231, z-12313) + 1) / 2;
    }

    private static float getMountainNoise(float x, float z)
    {
        float frequency = 10;
        float amp = 0.2f;
        float baseNoise = -Mathf.Abs(mountainNoise.GetNoise(x, z)) + 1;

        float weight = baseNoise;

        float toAdd = (-Mathf.Abs(mountainNoise.GetNoise((x + 100) * frequency, (z + 1000) * frequency)) + 1) * Mathf.Pow(weight, 3) * amp;


        frequency = 2;
        baseNoise += (toAdd);
        baseNoise /= 1 + amp;

        toAdd = (mountainFractalNoise.GetNoise((x - 1000) * frequency, (z + 1000) * frequency) / 2 + 0.5f) * Mathf.Pow(weight, 1.2f) * amp;

        baseNoise += (toAdd);
        baseNoise /= 1 + amp;

        return baseNoise;
    }

    private static float getHillsNoise(float x, float z)
    {
        float Aamplitude = 0.1f;
        float Bamplitude = 0.005f;
        return (hillsNoise.GetNoise(x, z) + 1) * Aamplitude + (hillsNoise.GetNoise(x * 5, z * 5) + 1) * Bamplitude;
    }

    private static float[] ranges = new float[] { -0.33f, 0.33f, 1f };
    private static float blendRange = 0.3f;

    static float blend(float from, float to, float value, int strength)
    {
        if (value <= from)
            return 1;
        if (value >= to)
            return 0;

        float domain = (value - from) / (to - from);

        return 1 / (1 + Mathf.Pow(domain / (1 - domain), -strength));
    }

    static Vector2 offset = new Vector2(10000, -1000);

    public static float[] getHeight(Vector3 pos)
    {
        
        float amplitude = 1000;
        float x = pos.x + offset.x;
        float z = pos.z+offset.y;
        float eleWeight = 0.2f;
        float desertDescale = 0.8f;
        float hillDescale = 0.8f;


        float landNoise = getLandNoise(x, z);
        float EleNoise = getEleNoise(x, z);

        float mountainsNoise = getMountainNoise(x, z);

        float hillsNoise = getHillsNoise(x, z) * hillDescale;

        float desertNoise = getDesertNoise(x, z) * desertDescale;



        float[] biomes = new float[] { desertNoise, hillsNoise, mountainsNoise };


        float biomeBlendNoise = getBiomeBlendNoise(x, z);

        float final = 0;

        for (int i = 0; i<ranges.Length; i++)
        {
            if(biomeBlendNoise < ranges[i] - ((i==ranges.Length-1)?0:blendRange))
            {
                final = biomes[i];
                break;
            } else if(i< ranges.Length-1 && biomeBlendNoise < ranges[i] + blendRange && biomeBlendNoise >= ranges[i] - blendRange)
            {
                float thisNoise = blend(ranges[i] - blendRange, ranges[i] + blendRange, biomeBlendNoise, 1);
                if (i == 1)
                    thisNoise = Mathf.Pow(thisNoise, 2);
                final = biomes[i + 1] * thisNoise + biomes[i] * (1 - thisNoise);

                break;
            }
        }


        final = (1 - eleWeight) * final + eleWeight * EleNoise;
        float landBlend = blend(0, 1, landNoise, 7);

        return new float[] {amplitude * final * landBlend + amplitude*0.5f * (desertNoise)*(1-landBlend) };
    }

    public static float getBoimeData(float x, float z)
    {
        
        return getBiomeBlendNoise(x + offset.x, z + offset.y);
    }

    public static float getDesnityData(float x, float z)
    {
        return (getBiomeBlendNoise(x, z)+1)/2;
    }

    public static float getCutoffData(float x, float z)
    {
        return (getBiomeBlendNoise(x+100000, z-30000) + 1) / 2;
    }

}
