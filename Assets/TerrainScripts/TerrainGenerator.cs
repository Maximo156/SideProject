using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using DataStructures.PriorityQueue;

using System.Threading;


public class TerrainGenerator : MonoBehaviour
{
    public static Dictionary<string, GameObject> HighChunks = new Dictionary<string, GameObject>();
    public static Dictionary<string, TerrainAssetChunk> TerrainAssetChunks = new Dictionary<string, TerrainAssetChunk>();

    List<string> toRemoveTerrain = new List<string>();
    List<string> toRemoveHigh = new List<string>();
    public static List<Chunk.ThreadInfo<Chunk.MapData>> ToLoadChunks = new List<Chunk.ThreadInfo<Chunk.MapData>>();

    private PriorityQueue<GameObject, float> PriChunks = new PriorityQueue<GameObject, float>(0);

    public static int sideLen = 200;
    public static int terrainChunksSideLen = 75;
    public static int TerrainChunkWidth = 10;
    
    private int vertexCount = 30;
    private static int high = 5;
    private static int med = 10;
    private static int low = 5;
    public static int chunkMax = high+med+low;

    private int midScale = 2;
    private int lowScale = 3;
    private float resPerMeterToGPU = 0.05f;
    private Vector3Int oldChunk = new Vector3Int(100, 100, 100);
    private bool updatedChunks = false;

    public Transform player;

    public Material terrainMaterial;
    public Material Grass;
    public TerrainLODController tLOD;

    public bool updateTextures = true;
    private bool midUpdate = false;
    public float[] heights;
    public float[] scales;
    Texture2D biomeTexture;

    int textWidth;
    public int cpf = 5;
    


    private Color[] biomeTextureColors = null;
    private Vector2 GPUPlayerPos;

    bool updateBiomeTexture = false;
    private bool testRun = true;


    

    void Start()
    {
        setShader(terrainMaterial, biomeTexture);
        int radius = (high + low + med) * sideLen;
        int length = (int)(2 * radius * resPerMeterToGPU);
        textWidth = length;
        biomeTexture = new Texture2D(length, length);
        biomeTextureColors = new Color[length * length];

    }

    void Update()
    {
        long start = System.DateTime.Now.Ticks;


        ChangePos(player.position);
        long total = System.DateTime.Now.Ticks - start;


        long ticks = System.DateTime.Now.Ticks;
        bool updated = false;
        long a,b,c,d,z;


        
        if (updateBiomeTexture)
        {
            biomeTexture.SetPixels(biomeTextureColors);
            biomeTexture.Apply(true);

            setBiomeTexture(terrainMaterial, biomeTexture);
            setBiomeTexture(Grass, biomeTexture);

            updateBiomeTexture = false;
            midUpdate = false;
        }
        a=(System.DateTime.Now.Ticks - ticks);
        ticks = System.DateTime.Now.Ticks;
        
        if (ToLoadChunks.Count != 0)
        {
            updated = true;
            for (int i = 0; i < cpf && ToLoadChunks.Count != 0; i++)
            {

                try
                {
                    ToLoadChunks[0].executeInfo();
                }
                catch (NullReferenceException e)
                {

                }
                catch (MissingReferenceException f)
                {

                }
                ToLoadChunks.RemoveAt(0);
            }
        }
        //print(System.DateTime.Now.Ticks - ticks);
        b=(System.DateTime.Now.Ticks - ticks);
        ticks = System.DateTime.Now.Ticks;
        


        if (PriChunks.Size() != 0)
        {
            try
            {
                PriChunks.Pop().GetComponent<Chunk>().AddCollider();
            }
            catch (NullReferenceException e)
            {

            }
            catch (MissingReferenceException f)
            {

            }
        }
        
        c =(System.DateTime.Now.Ticks - ticks);
        ticks=System.DateTime.Now.Ticks;
        if (updatedChunks && ToLoadChunks.Count == 0)
        {
            PlayerScript.SetHeight();
            updatedChunks = false;
            DestroyChunks(oldChunk, 10000);
        }
        d=(System.DateTime.Now.Ticks - ticks);
        ticks = System.DateTime.Now.Ticks;
        

        if (updateTextures)
        {
            setShader(terrainMaterial, biomeTexture);
            updateTextures = false;
        }
        z = (System.DateTime.Now.Ticks - ticks);
        
    }

    public void ChangePos(Vector3 pos)
    {
        Vector3Int currentChunk = Vector3Int.FloorToInt(pos / sideLen);

        currentChunk.y = 0;
        if (currentChunk != oldChunk)
        {
            long start = System.DateTime.Now.Ticks;
            updateChunks(currentChunk);
            //print(System.DateTime.Now.Ticks - start);
            start = System.DateTime.Now.Ticks;
            startBiomeThread();
            //print(System.DateTime.Now.Ticks - start);
            oldChunk = currentChunk;
            updatedChunks = true;
        }
    }

    private void updateChunks(Vector3Int currentChunk)
    {
        int start = DateTime.Now.Millisecond;
        int start2 = DateTime.Now.Millisecond;
        int highTimes = 0;
        int midTimes = 0;
        int lowTimes = 0;
        int initTime = 0;

        for (int i = -(high + med + low); i <= (high + med + low); i++)
        {
            int z_max = (int)Math.Floor(Math.Sqrt(Math.Pow((high + med + low), 2) - Math.Pow(i, 2)));
            for (int k = -z_max; k <= z_max; k++)
            {
                string chunkid = (i + currentChunk.x) + ":" + (k + currentChunk.z);
                start = DateTime.Now.Millisecond;
                float distance = Vector3.Distance(currentChunk, new Vector3(i + currentChunk.x, 0, k + currentChunk.z));
                GameObject posChunk = null;

                if (distance < high + med + low)
                {
                    HighChunks.TryGetValue(chunkid, out posChunk);
                    if (!posChunk)
                    {
                        posChunk = new GameObject();
                        posChunk.name = chunkid;
                        Chunk script = posChunk.AddComponent<Chunk>();

                        if (distance < high)
                        {
                            script.Initialize(currentChunk.x + i, currentChunk.z + k, sideLen, vertexCount, Chunk.Quality.High, midScale, lowScale, true);
                            HighChunks[chunkid] = posChunk;
                        }
                        else if (distance < high + med)
                        {
                            script.Initialize(currentChunk.x + i, currentChunk.z + k, sideLen, vertexCount, Chunk.Quality.Medium, midScale, lowScale);
                            HighChunks[chunkid] = posChunk;
                        }
                        else
                        {
                            script.Initialize(currentChunk.x + i, currentChunk.z + k, sideLen, vertexCount, Chunk.Quality.Low, midScale, lowScale);
                            HighChunks[chunkid] = posChunk;
                        }
                    }
                    else
                    {
                        if (distance < 2)
                        {
                            PriChunks.Insert(HighChunks[chunkid], distance);
                        }
                        Chunk.Quality qual = (distance < high) ? Chunk.Quality.High : ((distance < high + med) ? Chunk.Quality.Medium : Chunk.Quality.Low);
                        posChunk.GetComponent<Chunk>().UpdateMesh(qual, distance);
                    }
                }

                if (posChunk)
                    posChunk.transform.parent = gameObject.transform;
            }
        }


        /////////////////////////////////
        
        Vector3Int currentTerrainChunk = Vector3Int.FloorToInt(player.position / terrainChunksSideLen);
        currentTerrainChunk.y = 0;
        for (int i = -(TerrainChunkWidth); i <= TerrainChunkWidth ; i++)
        {
            int z_max = (int)Math.Floor(Math.Sqrt(Math.Pow((TerrainChunkWidth), 2) - Math.Pow(i, 2)));
            for (int k = -z_max; k <= z_max; k++)
            {
                string chunkid = (i + currentTerrainChunk.x) + ":" + (k + currentTerrainChunk.z);
                float distance = Vector3.Distance(currentTerrainChunk, new Vector3(i + currentTerrainChunk.x, 0, k + currentTerrainChunk.z));

                if (distance < TerrainChunkWidth)
                {
                    TerrainAssetChunk posChunk = null;
                    TerrainAssetChunks.TryGetValue(chunkid, out posChunk);
                    if (posChunk == null)
                    {
                        posChunk = new TerrainAssetChunk();
                        posChunk.Initialize(i + currentTerrainChunk.x, k + currentTerrainChunk.z, terrainChunksSideLen);
                        TerrainAssetChunks.Add(chunkid, posChunk);
                    }
                }
            }
        }
    }

    private void DestroyChunks(Vector3Int currentChunk, int max)
    {
        toRemoveHigh.Clear();
        int count = 0;
        foreach (var kvp in HighChunks)
        {
            Vector3 chunkPos = new Vector3(float.Parse(kvp.Key.Split(':')[0]), 0, float.Parse(kvp.Key.Split(':')[1]));
            if (Vector3.Distance(currentChunk, chunkPos) > high+med+low)
            {
                kvp.Value.GetComponent<Chunk>().Delete();
                Destroy(kvp.Value);
                toRemoveHigh.Add(kvp.Key);
                count++;
            }
            if(kvp.Value == null)
            {
                toRemoveHigh.Add(kvp.Key);
                count++;
            }
            if (count > max)
                break;
        }
        foreach (string s in toRemoveHigh)
        {
            HighChunks.Remove(s);
        }

        
        Vector3Int currentTerrainChunk = Vector3Int.FloorToInt(player.position / terrainChunksSideLen);
        toRemoveTerrain.Clear();
        foreach (var kvp in TerrainAssetChunks)
        {
            Vector3 chunkPos = new Vector3(float.Parse(kvp.Key.Split(':')[0]), 0, float.Parse(kvp.Key.Split(':')[1]));
            if (Vector3.Distance(currentTerrainChunk, chunkPos) > TerrainChunkWidth)
            {
                toRemoveTerrain.Add(kvp.Key);
                count++;
            }
            if (kvp.Value == null)
            {
                toRemoveTerrain.Add(kvp.Key);
                count++;
            }
            if (count > max)
                break;
        }
        foreach (string s in toRemoveTerrain)
        {
            TerrainAssetChunks.Remove(s);
        }
        

    }

    public void setShader(Material material, Texture t)
    {
        material.SetInt("mountainHeightsCount", heights.Length);
        material.SetFloatArray("heights", heights);
        material.SetFloatArray("scales", scales);
        if (t != null)
        {
            material.SetTexture("biomeInfo", t);
            material.SetVector("playerPos", GPUPlayerPos);
            material.SetInt("textWidth", (int)(textWidth / resPerMeterToGPU));
        }
    }

    public void setBiomeTexture(Material material, Texture t)
    {
        
        material.SetTexture("biomeInfo",t);
        material.SetVector("playerPos", GPUPlayerPos);
        material.SetInt("textWidth", (int)(textWidth/ resPerMeterToGPU));

    }


    private void startBiomeThread()
    {
        if (!midUpdate)
        {
            midUpdate = true;
            Vector3Int currentChunk = Vector3Int.FloorToInt(player.position / sideLen);
            ThreadStart threadStart = delegate
            {

                genBiomeTexture(new Vector2(currentChunk.x * sideLen, currentChunk.z * sideLen));
            };
            new Thread(threadStart).Start();
        }
    }

    void genBiomeTexture(Vector2 pos)
    {
        int radius = (high + low + med) * sideLen;
        int length = (int)(2 * radius * resPerMeterToGPU);
        
        for(int i = 0; i < length; i++)
        {
            for (int k = 0; k < length; k++)
            {
                float xPos = pos.x + (i - length/ 2) / resPerMeterToGPU;
                float yPos = pos.y + (k - length / 2) / resPerMeterToGPU;
                float biome = (HeightNoise.getBoimeData(xPos, yPos)+1)/2;
                biomeTextureColors[i * length + k].r = biome;// = new Color(biome, biome, biome); ;
                biomeTextureColors[i * length + k].g = biome;
                biomeTextureColors[i * length + k].b = biome;
            }
        }
        GPUPlayerPos = pos;
        updateBiomeTexture = true;
    }
}
