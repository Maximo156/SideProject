using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Linq;

public class Chunk : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer mr;
    
    float toReturn;
    private bool isLoaded;
    int Chunkx;
    int Chunkz;
    int sideLen;
    int vertexCount;
    new bool collider;
    MeshCollider mc;
    bool thread = true;
    int biomeCount = 3;


    Mesh highMesh;
    Mesh medMesh;
    Mesh lowMesh;
    MapData data = null;
    string chunkID;

    GameObject grassLayer;

    int lowScale;
    int medScale;

    public enum Quality {High, Medium, Low};
    public Quality quality;

    BuildingManager buildings;

    void Update()
    {
    }

    void cullMesh(int xmin, int xmax, int zmin, int zmax)
    {
        List<Vector3> verts = highMesh.vertices.ToList();
        List<int> tri = highMesh.triangles.ToList();
        List<int> toRemove = new List<int>();
        for(int i = 0; i<verts.Count; i++)
        {
            if(verts[i].x > xmin && verts[i].x < xmax && verts[i].z > zmin && verts[i].z < zmax)
            {
                //verts.RemoveAt(i);
                toRemove.Add(i);
            }
        }

        for(int i = 0; i<toRemove.Count; i++)
        {
            int index = -1;
            while((index = tri.IndexOf(toRemove[i])) != -1)
            {
                tri.RemoveRange(index - index % 3, 3);
            }
        }

        highMesh.vertices = verts.ToArray();
        highMesh.triangles = tri.ToArray();
    }

    public float Initialize(int Chunkx, int Chunkz, int sideLen, int vertexCount, Quality quality, int medScale, int lowScale,  bool collider = false)
    {
        gameObject.layer = 2;
        chunkID = Chunkx + ":" + Chunkz;
        this.medScale = medScale;
        this.lowScale = lowScale;
        this.quality = quality;
        this.Chunkx = Chunkx;
        this.Chunkz = Chunkz;
        this.sideLen = sideLen;
        this.vertexCount = vertexCount;
        this.collider = collider;
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = GameObject.Find("TerrainGenerator").GetComponent<TerrainGenerator>().terrainMaterial;

        buildings = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();

        startShaderGenThread();
        
        return toReturn;
    }

    public Vector2 GetWorldPosition()
    {
        return new Vector2(Chunkx * sideLen + sideLen/2, Chunkz * sideLen + sideLen / 2);
    }

    public void Delete()
    {
        Destroy(highMesh);
        Destroy(medMesh);
        Destroy(lowMesh);
    }

    public bool IsLoaded()
    {
        return isLoaded;
    }

    Mesh getMesh(Vector3[] verts, int[] triangles, bool med = false)
    {
        Mesh mesh = new Mesh();

        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }

    public void ShaderCallBack(MapData data)
    {
        medMesh = getMesh(data.verticesMed,data.trianglesMed, true);
        highMesh = getMesh(data.verticesHigh, data.trianglesHigh);
        lowMesh = getMesh(data.verticesLow, data.trianglesLow, true);

        setMesh();
        if (collider)
        {
            mc = gameObject.AddComponent<MeshCollider>();
            mc.sharedMesh = medMesh;
        }
        float max = float.MinValue;
        float min = float.MaxValue;

        //cullMesh(Chunkx * sideLen, Chunkx * sideLen + 10, Chunkz * sideLen, Chunkz * sideLen + 10);
        buildings.TryAddTown(Chunkx, Chunkz, sideLen);
    }

    void addGrass()
    {
        if (grassLayer == null && SystemInfo.operatingSystem.ToLower().Contains("window"))
        {
            grassLayer = new GameObject("grassLayer");
            grassLayer.transform.parent = transform;
            var gmf = grassLayer.AddComponent<MeshFilter>();
            gmf.mesh = highMesh;
            var gmr = grassLayer.AddComponent<MeshRenderer>();
            gmr.material = GameObject.Find("TerrainGenerator").GetComponent<TerrainGenerator>().Grass;
        }
    }

    void removeGrass()
    {
        if (grassLayer != null)
        {
            Destroy(grassLayer);
            grassLayer = null;
        }
    }

    public void UpdateMesh(Quality qual, float dist)
    {       

        if (qual != quality)
        {
            quality = qual;
            setMesh();
        }
        
    }

    private void setMesh()
    {
        try
        {
            if (quality == Quality.High && highMesh == null)
            {
                highMesh = getMesh(data.verticesHigh, data.trianglesHigh);
                
            }
            else if (quality == Quality.Low && lowMesh == null)
                lowMesh = getMesh(data.verticesLow, data.trianglesLow);
            else if (quality == Quality.Medium && medMesh == null)
                medMesh = getMesh(data.verticesMed, data.trianglesMed);
        }
        catch (NullReferenceException e) { }
        meshFilter.mesh = (quality == Quality.High) ? highMesh : ((quality == Quality.Medium) ? medMesh : lowMesh);
        if(quality == Quality.High)
        {
            addGrass();
        }
    }

    public void AddCollider()
    {
        if (!collider)
        {
            collider = true;
            mc = gameObject.AddComponent<MeshCollider>();
            mc.sharedMesh = medMesh;
        }
    }
    
    private void startShaderGenThread()
    {
        ThreadStart threadStart = delegate
        {
            ShaderGen(vertexCount, sideLen, Chunkx, Chunkz, lowScale, medScale, this.ShaderCallBack);
        };
        new Thread(threadStart).Start();
    }

    private MeshInfo meshGen(int vertexCount, int sideLen, int xOff, int zOff)
    {

        int width = vertexCount;
        int height = vertexCount;
        // Creating a mesh object.
        int count = 0;

        Vector3[] verticesHigh = new Vector3[(width + 1) * (height + 1)];
        for (float d = 0; d <= height; d++)
        {
            for (float w = 0; w <= width; w++)
            {
                Vector3 newPos = new Vector3(((float)sideLen / (float)vertexCount) * w + (float)(xOff * sideLen), 0, ((float)sideLen / (float)vertexCount) * d + (float)(zOff * sideLen));
                newPos.y = HeightNoise.getHeight(newPos)[0];
                verticesHigh[count] = newPos;

                count++;
            }
        }



        // Defining triangles.

        int[] trianglesHigh = new int[width * height * 2 * 3]; // 2 - polygon per quad, 3 - corners per polygon
        for (int d = 0; d < height; d++)
        {
            for (int w = 0; w < width; w++)
            {
                // quad triangles index.
                int ti = (d * (width) + w) * 6; // 6 - polygons per quad * corners per polygon
                                                // First tringle
                trianglesHigh[ti] = (d * (width + 1)) + w;
                trianglesHigh[ti + 1] = ((d + 1) * (width + 1)) + w;
                trianglesHigh[ti + 2] = ((d + 1) * (width + 1)) + w + 1;
                // Second triangle
                trianglesHigh[ti + 3] = (d * (width + 1)) + w;
                trianglesHigh[ti + 4] = ((d + 1) * (width + 1)) + w + 1;
                trianglesHigh[ti + 5] = (d * (width + 1)) + w + 1;
            }
        }

        return new MeshInfo(verticesHigh, trianglesHigh);
    }

    private void ShaderGen(int vertexCount, int sideLen, int xOff, int zOff, int lowScale, int medScale, Action<MapData> callback)//int Chunkx, int Chunkz, int sideLen, int vertexCount)
    {
        MeshInfo highInfo = meshGen(vertexCount, sideLen, xOff, zOff);
        MeshInfo medInfo = meshGen(vertexCount/medScale, sideLen, xOff, zOff);
        MeshInfo lowInfo = meshGen(vertexCount/lowScale, sideLen, xOff, zOff);
        
        // Creating a mesh object.

        // Defining vertices.
        Vector3[] verticesHigh = highInfo.vertices;
        Vector3[] verticesMed = medInfo.vertices;
        Vector3[] verticesLow = lowInfo.vertices;

        int[] trianglesMed = medInfo.triangles;
        int[] trianglesLow = lowInfo.triangles;
        int[] trianglesHigh = highInfo.triangles; // 2 - polygon per quad, 3 - corners per polygon
        
        MapData map = new MapData(verticesHigh, trianglesHigh, verticesMed, trianglesMed, verticesLow, trianglesLow);

        lock (TerrainGenerator.ToLoadChunks)
        {
            TerrainGenerator.ToLoadChunks.Add(new ThreadInfo<Chunk.MapData>(callback, map));
        }
    }


    public class MapData
    {
        public Vector3[] verticesHigh;
        public int[] trianglesHigh;
        public Vector3[] verticesMed;
        public int[] trianglesMed;
        public Vector3[] verticesLow;
        public int[] trianglesLow;


        public MapData(Vector3[] verticesHigh, int[] trianglesHigh, Vector3[] verticesMed, int[] trianglesMed, Vector3[] verticesLow, int[] trianglesLow)
        {
            this.verticesHigh = verticesHigh;
            this.trianglesHigh = trianglesHigh;
            this.verticesMed = verticesMed;
            this.trianglesMed = trianglesMed;
            this.verticesLow = verticesLow;
            this.trianglesLow = trianglesLow;
        }
    }

    public class MeshInfo
    {
        public Vector3[] vertices;
        public int[] triangles;

        public MeshInfo(Vector3[] vertices, int[] triangles)
        {
            this.vertices = vertices;
            this.triangles = triangles;
        }
    }

    public class ThreadInfo<T> 
    {
        Action<T> callback;
        T info;


        public ThreadInfo(Action<T> callback, T info)
        {
            this.info = info;
            this.callback = callback;
        }

        public void executeInfo()
        {
            callback(info);
        }
    }

}
