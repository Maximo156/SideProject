using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderHeightNoise
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
        biomeBlendNoise.SetFrequency(0.00005f);

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

    static ShaderHeightNoise()
    {
        MakeMountainNoise();
        MakeEleNoise();
        MakeHillsNoise();
        MakeDesertNoise();
        MakeBiomeBlendNoise();
        MakeMountainFractalNoise();
    }

  

    private static  float getBiomeBlendNoise(float x, float z)
    {
        return biomeBlendNoise.GetNoise(x, z);
    }

    

    private static float getDesertNoise(float x, float z)
    {
        float Aamplitude = .08f;
        float Camplitude = .030f;
        float noiseA = -Mathf.Abs(desertNoise.GetNoise(x, z))+1;
        float noiseC = desertNoise.GetNoise(x * 3, z * 3)/2+0.5f;

        return (noiseA * Aamplitude  + noiseC* Camplitude)*0.9f;
    }

    private static float getEleNoise(float x, float z)
    {
        return (eleNoise.GetNoise(x, z)+1)/2;
    }

    private static float getMountainNoise(float x, float z)
    {
        float frequency = 10;
        float amp = 0.2f;
        float baseNoise = -Mathf.Abs(mountainNoise.GetNoise(x, z))+1;

        float weight = baseNoise;

        float toAdd = (-Mathf.Abs(mountainNoise.GetNoise((x+100) * frequency, (z+1000) * frequency)) + 1) * Mathf.Pow(weight, 3) * amp;
        

        frequency = 2;
        baseNoise += (toAdd);
        baseNoise /= 1 + amp;

        toAdd = (mountainFractalNoise.GetNoise((x - 1000) * frequency, (z + 1000) * frequency)/2+0.5f)   * Mathf.Pow(weight, 1.2f) * amp;
        
        baseNoise += (toAdd);
        baseNoise /= 1 + amp;

        return baseNoise;
    }

    private static float getHillsNoise(float x, float z)
    {
        float Aamplitude = 0.1f;
        float Bamplitude = 0.005f;
        return (hillsNoise.GetNoise(x, z) + 1)  * Aamplitude + (hillsNoise.GetNoise(x*5, z*5) + 1) * Bamplitude;
    }

    public static float[] getHeight(Vector3 pos)
    {
        
        float amplitude = 1000;
        float x = pos.x;
        float z = pos.z;
        float eleWeight = 0.2f;
        float desertDescale = 0.8f;
        float hillDescale = 0.8f;

        float EleNoise = getEleNoise(x, z);

        float mountainsNoise = getMountainNoise(x, z);

        float hillsNoise = getHillsNoise(x, z)* hillDescale;

        float desertNoise = getDesertNoise(x, z)* desertDescale;

        float[] biomes = new float[] { (1-eleWeight)*desertNoise + EleNoise*eleWeight, (1 - eleWeight) * hillsNoise + EleNoise * eleWeight, (1 - eleWeight) * mountainsNoise + EleNoise * eleWeight };

        return biomes;


    }

}

