//#define Alga
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using DataStructures.PriorityQueue;

using System;

public class TerrainAssetManager : MonoBehaviour
{
    List<string> toRemoveChunks = new List<string>();
    public List<TerrainObjects> TerrainObjectInfo = new List<TerrainObjects>();
    public static List<BiomeObjects> Biomes;
    public List<BiomeObjects> BiomesNS;
    public int maximumAssetsNS;
    public int chunkAngleCheck = 80;
    public int objectAngleCheck = 50;

    public BuildingManager buildings;
    public Transform player;

    private static int usedAssetCount;
    private static int maximumAssets;

    bool[,] usedChunks;

    private Dictionary<Vector3, float> brokenObjects = new Dictionary<Vector3, float>();
    private Dictionary<Vector3, GameObject> usedObjects = new Dictionary<Vector3, GameObject>();
    
#if Alga
    private List<Tuple<int, int>> chunkQueue = new List<Tuple<int, int>>();
#else
    private List<TerrainAssetChunk> chunkQueue = new List<TerrainAssetChunk>();
#endif
    [Serializable]
    public class BiomeObjects
    {
        public List<int> types;
        public List<float> percentages;
    }

    

    [Serializable]
    public class TerrainObjects
    {
        public GameObject reference;
        public int maxInstances;
        private LinkedList<GameObject> objectPool = new LinkedList<GameObject>();
        private int checkedOut = 0;

        public void InstantiatePool(GameObject parent, int index)
        {
            for(int i = 0; i<maxInstances; i++)
            {
                GameObject next = Instantiate(reference, parent.transform);
                next.name = index + "";
                next.SetActive(false);
                objectPool.AddLast(next);
            }
        }

        public GameObject CheckOutObject()
        {
            if(checkedOut >= maxInstances || usedAssetCount >= maximumAssets)
            {
                return null;
            }
            usedAssetCount++;
            GameObject temp = objectPool.First.Value;
            objectPool.RemoveFirst();
            checkedOut++;
            temp.SetActive(true);
            return temp;
        }

        public void CheckInObject(GameObject toAdd)
        {
            usedAssetCount--;
            checkedOut--;
            toAdd.SetActive(false);
            objectPool.AddLast(toAdd);
        }
        public bool FreeObjs()
        {
            return checkedOut != maxInstances; 
        }
    }


    GameObject CheckOut(int type)
    {
        type = Mathf.Min(type, TerrainObjectInfo.Count-1);
        return TerrainObjectInfo[type].CheckOutObject();
    }

    void CheckIn(GameObject toAdd)
    {
        int index = Int32.Parse(toAdd.name);
        TerrainObjectInfo[index].CheckInObject(toAdd);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        Biomes = BiomesNS;
        for (int i = 0; i < TerrainObjectInfo.Count; i++)
        {
            TerrainObjectInfo[i].InstantiatePool(gameObject, i);
        }
    }

    static Vector2 V3toV2(Vector3 input)
    {
        if(input.z == 0 && input.y != 0)
            return new Vector2(input.x, input.y);
        return new Vector2(input.x, input.z);
    }

    bool ObjAvailable()
    {
        bool res = false;
        foreach(var obj in TerrainObjectInfo)
        {
            res = res || obj.FreeObjs();
        }
        return res;
    }

    bool goodAngle(Tuple<int, int> chunkPos, Tuple<int, int> curChunk, Vector2 playerDir)
    {
        float distance = Vector2.Distance(new Vector2(chunkPos.Item1, chunkPos.Item2), new Vector2(curChunk.Item1, curChunk.Item2));
        Vector2 chunkDir = new Vector2(chunkPos.Item1, chunkPos.Item2) - new Vector2(curChunk.Item1, curChunk.Item2) + playerDir * Mathf.Sqrt(2) * TerrainGenerator.terrainChunksSideLen;
        return (Vector2.Angle(playerDir, chunkDir) < chunkAngleCheck) && (distance < TerrainGenerator.TerrainChunkWidth);
    }

    private float timeToGo;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.fixedTime >= timeToGo)
        {
            foreach (var item in brokenObjects.Where(kvp => kvp.Value < Time.fixedTime).ToList())
            {
                brokenObjects.Remove(item.Key);
            }

            timeToGo = Time.fixedTime + 1;
        }


        maximumAssets = maximumAssetsNS;
        Vector3Int currentChunk = Vector3Int.FloorToInt(player.position / TerrainGenerator.terrainChunksSideLen);

        usedChunks = new bool[2 * TerrainGenerator.TerrainChunkWidth + 1, 2 * TerrainGenerator.TerrainChunkWidth + 1];

        Vector2 worldPos;
        Vector2 p = V3toV2(player.forward);
        foreach (var obj in usedObjects.ToList())
        {
            Vector2 wp = V3toV2(obj.Key) + V3toV2(player.forward) * 5;
            if (Vector2.Angle(p, wp - V3toV2(player.position)) > objectAngleCheck || Vector2.Distance(wp, V3toV2(player.position)) > 100)
            {
                CheckIn(obj.Value);
                usedObjects.Remove(obj.Key);
            }
        }

        //usedObjects.Sele

        //usedObjects.Clear();

        /////////////////////////////////////////////

#if Alga
        Tuple<int, int> centre = new Tuple<int, int>(TerrainGenerator.TerrainChunkWidth + 1, TerrainGenerator.TerrainChunkWidth + 1);
        chunkQueue.Add(centre);
        usedChunks[centre.Item1, centre.Item2] = true;

        int index = 0;
        while (index < chunkQueue.Count() && chunkQueue.Count() < (2 * TerrainGenerator.TerrainChunkWidth + 1) * (2 * TerrainGenerator.TerrainChunkWidth + 1))
        {
            Tuple<int, int> curChunk = chunkQueue[index];
            

            if (curChunk.Item1 >= 1 && curChunk.Item1 < 2 * TerrainGenerator.TerrainChunkWidth && curChunk.Item2 >= 1 && curChunk.Item2 < 2 * TerrainGenerator.TerrainChunkWidth)
            {
                Tuple<int, int> potChunk = new Tuple<int, int>(curChunk.Item1 + 1, curChunk.Item2);
                if (goodAngle(potChunk, centre, player.forward) && !usedChunks[potChunk.Item1, potChunk.Item2])
                {
                    chunkQueue.Add(potChunk);
                    usedChunks[potChunk.Item1, potChunk.Item2] = true;
                }

                potChunk = new Tuple<int, int>(curChunk.Item1 - 1, curChunk.Item2);
                if (goodAngle(potChunk, centre, player.forward) && !usedChunks[potChunk.Item1, potChunk.Item2])
                {
                    chunkQueue.Add(potChunk);
                    usedChunks[potChunk.Item1, potChunk.Item2] = true;
                }

                potChunk = new Tuple<int, int>(curChunk.Item1, curChunk.Item2 + 1);
                if (goodAngle(potChunk, centre, player.forward) && !usedChunks[potChunk.Item1, potChunk.Item2])
                {
                    chunkQueue.Add(potChunk);
                    usedChunks[potChunk.Item1, potChunk.Item2] = true;
                }

                potChunk = new Tuple<int, int>(curChunk.Item1, curChunk.Item2 - 1);
                if (goodAngle(potChunk, centre, player.forward) && !usedChunks[potChunk.Item1, potChunk.Item2])
                {
                    chunkQueue.Add(potChunk);
                    usedChunks[potChunk.Item1, potChunk.Item2] = true;
                }
                
            }
            
            index++;
        }
        index = 0;
        
        while (index < chunkQueue.Count() && usedAssetCount < maximumAssets)
        {
            Tuple<int, int> curChunk = chunkQueue[index];
            TerrainAssetChunk chunk = null;
            string chunkid = (curChunk.Item1 - 2 * TerrainGenerator.TerrainChunkWidth + currentChunk.x) + ":" + (curChunk.Item2 - 2 * TerrainGenerator.TerrainChunkWidth + currentChunk.z);
            TerrainGenerator.TerrainAssetChunks.TryGetValue(chunkid, out chunk);
            if (chunk != null)
            {
                RequestCheckout(chunk.TerrainObjects);
            }
            
            index++;
        }
        //print(usedChunks);
        chunkQueue.Clear();
        
#else
        TerrainAssetChunk chunk = null;
        string chunkid = (currentChunk.x) + ":" + (currentChunk.z);
        TerrainGenerator.TerrainAssetChunks.TryGetValue(chunkid, out chunk);
        Vector2 p1 = V3toV2(player.forward);
        if (chunk != null)
        {
            chunkQueue.Add(chunk);
            chunk.hasRequested = true;
            int index = 0;
            while (index < chunkQueue.Count && usedAssetCount < maximumAssets)
            {
                TerrainAssetChunk curChunk = chunkQueue[index];
                if (curChunk.objGood)
                {
                    RequestCheckout(curChunk.TerrainObjects);
                }

                TerrainGenerator.TerrainAssetChunks.TryGetValue((curChunk.Chunkx - 1) + ":" + (curChunk.Chunkz), out chunk);
                if (chunk != null)
                {
                    worldPos = chunk.GetWorldPosition() + V3toV2(player.forward) * Mathf.Sqrt(2) * TerrainGenerator.terrainChunksSideLen;
                    if (Vector2.Angle(p1, worldPos - V3toV2(player.position)) < chunkAngleCheck && chunk.hasRequested == false)
                    {
                        chunkQueue.Add(chunk);
                        chunk.hasRequested = true;
                    }
                }

                TerrainGenerator.TerrainAssetChunks.TryGetValue((curChunk.Chunkx) + ":" + (curChunk.Chunkz - 1), out chunk);
                if (chunk != null)
                {
                    worldPos = chunk.GetWorldPosition() + V3toV2(player.forward) * Mathf.Sqrt(2) * TerrainGenerator.terrainChunksSideLen;
                    if (Vector2.Angle(p1, worldPos - V3toV2(player.position)) < chunkAngleCheck && chunk.hasRequested == false)
                    {
                        chunkQueue.Add(chunk);
                        chunk.hasRequested = true;
                    }
                }

                TerrainGenerator.TerrainAssetChunks.TryGetValue((curChunk.Chunkx + 1) + ":" + (curChunk.Chunkz), out chunk);
                if (chunk != null)
                {
                    worldPos = chunk.GetWorldPosition() + V3toV2(player.forward) * Mathf.Sqrt(2) * TerrainGenerator.terrainChunksSideLen;
                    if (Vector2.Angle(p1, worldPos - V3toV2(player.position)) < chunkAngleCheck && chunk.hasRequested == false)
                    {
                        chunkQueue.Add(chunk);
                        chunk.hasRequested = true;
                    }
                }

                TerrainGenerator.TerrainAssetChunks.TryGetValue((curChunk.Chunkx) + ":" + (curChunk.Chunkz + 1), out chunk);
                if (chunk != null)
                {
                    worldPos = chunk.GetWorldPosition() + V3toV2(player.forward) * Mathf.Sqrt(2) * TerrainGenerator.terrainChunksSideLen;
                    if (Vector2.Angle(p1, worldPos - V3toV2(player.position)) < chunkAngleCheck && chunk.hasRequested == false)
                    {
                        chunkQueue.Add(chunk);
                        chunk.hasRequested = true;
                    }
                }
                index++;
            }
        }

        foreach(var chunkDel in chunkQueue)
        {
            chunkDel.hasRequested = false;
        }
        chunkQueue.Clear();
#endif
    }

    void RequestCheckout(Vector4[] objs)
    {
        bool dup = false;
        foreach(var obj in objs)
        {
            Vector3 pos = new Vector3(obj.x, obj.y-0.2f, obj.z);
            Vector2 pos2D = new Vector2(obj.x, obj.z);

            if (!buildings.ValidTerrainPos(pos2D))
            {
                continue;
            }

            int type = (int)obj[3];

            Vector2 p1 = V3toV2(player.forward);
            Vector2 worldPos = V3toV2(pos) + V3toV2(player.forward) * 5;
            if (Vector2.Angle(p1, worldPos - V3toV2(player.position)) < objectAngleCheck)
            {
                if (!usedObjects.ContainsKey(pos) && !brokenObjects.ContainsKey(pos))
                {
                    GameObject instance = CheckOut(type);
                    if (instance != null)
                    {
                        instance.transform.position = pos;
                        usedObjects.Add(pos, instance);
                    }
                }
            }
            
            else
            {
                dup = true;
            }
        }

        
        if (dup &&false)
        {
            print("OBJECT LIST START");
            foreach (var obj in objs)
            {
                print(obj);
            }

            print("OBJECT LIST END");
        }
    }

    public void BreakObject(Vector3 pos, float regenTime)
    {
        if (usedObjects.ContainsKey(pos))
        {
            if (!brokenObjects.ContainsKey(pos))
            {
                brokenObjects.Add(pos, Time.time + regenTime);
            }
            CheckIn(usedObjects[pos]);
            usedObjects.Remove(pos);
        }
        else
        {
            float closestDist = 100;
            Vector3 closestLoc = new Vector3();
            foreach(KeyValuePair<Vector3, GameObject> kvp in usedObjects)
            {
                if(Vector3.Distance(pos, kvp.Key) < closestDist)
                {
                    closestDist = Vector3.Distance(pos, kvp.Key);
                    closestLoc = kvp.Key;
                }
            }
            if (!brokenObjects.ContainsKey(closestLoc))
            {
                brokenObjects.Add(closestLoc, Time.time + regenTime);
            }
            CheckIn(usedObjects[closestLoc]);
            usedObjects.Remove(closestLoc);
        }
    }

    public string GetName(string i)
    {
        return (TerrainObjectInfo[int.Parse(i)].reference.name);
    }

}
