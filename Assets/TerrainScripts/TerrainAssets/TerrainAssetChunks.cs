using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Linq;

public class TerrainAssetChunk
{
    float toReturn;
    public int Chunkx;
    public int Chunkz;
    int sideLen;

    string chunkID;

    public Vector4[] TerrainObjects;
    public Vector3[] TerrainNormals;
    public bool objGood = false;
    public bool hasRequested = false;

    void Update()
    {
    }

    public float Initialize(int Chunkx, int Chunkz, int sideLen)
    {
        chunkID = Chunkx + ":" + Chunkz;
        this.Chunkx = Chunkx;
        this.Chunkz = Chunkz;
        this.sideLen = sideLen;

        startObjectGenThread();

        return toReturn;
    }

    public Vector2 GetWorldPosition()
    {
        return new Vector2(Chunkx * sideLen + sideLen / 2, Chunkz * sideLen + sideLen / 2);
    }

    private void startObjectGenThread()
    {
        ThreadStart threadStart = delegate
        {
            ObjectGen(sideLen, 100, 1, Chunkx, Chunkz);
        };
        new Thread(threadStart).Start();
    }

    float nextDouble(System.Random rand, float from, float to)
    {
        return Mathf.InverseLerp(from, to, (float)rand.NextDouble());
    }


    int GetObjectType(float biomeNoise, System.Random rand)
    {
        try
        {
            if (biomeNoise < -0.43)
                return TerrainAssetManager.GetBiomeObject(0, rand); //return TerrainAssetManager.Biomes[0].types[rand.Next(0, TerrainAssetManager.Biomes[0].types.Count)];
            else if (biomeNoise < -0.23)
            {
                if (nextDouble(rand, -0.43f, -0.23f) > biomeNoise)
                {
                    return TerrainAssetManager.GetBiomeObject(0, rand); // Biomes[0].types[rand.Next(0, TerrainAssetManager.Biomes[0].types.Count)];
                }
                else
                {
                    return TerrainAssetManager.GetBiomeObject(1, rand);  //return TerrainAssetManager.Biomes[1].types[rand.Next(0, TerrainAssetManager.Biomes[1].types.Count)];
                }
            }
            else if (biomeNoise < 0.23f)
                return TerrainAssetManager.GetBiomeObject(1, rand);  //return TerrainAssetManager.Biomes[1].types[rand.Next(0, TerrainAssetManager.Biomes[1].types.Count)];
            else if (biomeNoise < 0.43f)
            {
                if (nextDouble(rand, 0.23f, 0.43f) > biomeNoise)
                {
                    return TerrainAssetManager.GetBiomeObject(1, rand);  //return TerrainAssetManager.Biomes[1].types[rand.Next(0, TerrainAssetManager.Biomes[1].types.Count)];
                }
                else
                {
                    return TerrainAssetManager.GetBiomeObject(2, rand);  //return TerrainAssetManager.Biomes[2].types[rand.Next(0, TerrainAssetManager.Biomes[2].types.Count)];
                }
            }

            return TerrainAssetManager.GetBiomeObject(2, rand);//int toReturn = TerrainAssetManager.Biomes[2].types[rand.Next(0, TerrainAssetManager.Biomes[2].types.Count)];
            //return toReturn;
        }
        catch (NullReferenceException e)
        {
            Console.WriteLine(biomeNoise);
            return 0;
        }
    }

    Vector3 Slope(float x, float y)
    {
        Vector3 p1 = new Vector3(x - 1, 0, y);
        p1.y = HeightNoise.getHeight(p1)[0];

        Vector3 p2 = new Vector3(x, 0, y - 1);
        p2.y = HeightNoise.getHeight(p2)[0];

        Vector3 p3 = new Vector3(x+1, 0, y + 1);
        p3.y = HeightNoise.getHeight(p3)[0];

        Vector3 normal = Vector3.Cross(p3 - p1, p2 - p1);
        normal.Normalize();
        return normal;
        //return 1 - normal.y;
    }

    private void ObjectGen(float sideLength, int numObjects, int minDistance, int Chunkx, int Chunkz)
    {
        var rand = new System.Random((Chunkx + ":" + Chunkz).GetHashCode());
        Vector4[] objs = new Vector4[numObjects];
        Vector3[] norms = new Vector3[numObjects];
        for (int i = 0; i < numObjects; i++)
        {
            float x = (float)rand.NextDouble() * sideLength + Chunkx * sideLength;
            float y = (float)rand.NextDouble() * sideLength + Chunkz * sideLength;
            bool valid = true;
            foreach (var vec in objs)
            {
                if (Vector2.Distance(new Vector2(x, y), new Vector2(vec.x, vec.z)) < minDistance)
                {
                    valid = false;
                    break;
                }
            }
            if (!valid)
            {
                i--;
                continue;
            }
            if (HeightNoise.getDesnityData(x, y) > rand.NextDouble()*0.9+0.1)
            {
                float height = HeightNoise.getHeight(new Vector3(x, 0, y))[0];
                Vector3 normal = Slope(x, y);
                if (height > 105 && 1 - normal.y < 0.1)
                {
                    objs[i] = new Vector4(x, height, y, GetObjectType(HeightNoise.getBoimeData(x, y), rand));
                    norms[i] = normal;
                }
            }
        }
        objs = objs.Where(x => x != new Vector4(0,0,0,0)).ToArray();
        objs = objs.Distinct().ToArray();
        this.TerrainObjects = objs;
        TerrainNormals = norms;
        objGood = true;

    }
}
