using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

using System;


[Serializable]
public class BuildingParts
{
    public int spawnAttempts;
    public GameObject Floor;
    public GameObject[] Walls;
    public GameObject[] Roofs;
    public GameObject[] Fillers;
    public GameObject Ladder;

    public GameObject[] spawnables;

    public GameObject GetWall()
    {
        return Walls[UnityEngine.Random.Range(0, Walls.Length - 2)];
    }

    public GameObject GetDoor()
    {
        return Walls[Walls.Length-2];
    }

    public GameObject GetFoundation()
    {
        return Walls[Walls.Length - 1];
    }

    public GameObject GetRandom()
    {
        return Fillers[UnityEngine.Random.Range(0, Fillers.Length)];
    }

    public GameObject GetSpawnable()
    {
        if(spawnables.Length > 0)
        {
            return spawnables[Random.Range(0, spawnables.Length)];
        }
        return null;
    }

    
}

[Serializable]
public class DungeonParts
{
    public GameObject Floor;
    public GameObject Walls;
    public GameObject Ladder;
    public GameObject Door;
    public GameObject roof;
    public List<GameObject> OuterObjects;
    

    public GameObject GetOuter()
    {
        if (OuterObjects.Count > 0)
        {
            return OuterObjects[Random.Range(0, OuterObjects.Count)];
        }
        return null;
    }
}


public class BuildingManager : MonoBehaviour
{

    private int hash(int x)
    {
        x = ((x >> 16) ^ x) * 0x45d9f3b;
        x = ((x >> 16) ^ x) * 0x45d9f3b;
        x = (x >> 16) ^ x;
        return x;
    }


    private List<Rect> townBounds = new List<Rect>();

    private SortedSet<int> hashes = new SortedSet<int>();
    private List<BuildingUnifier> objects = new List<BuildingUnifier>();

    public BuildingParts GreenParts;
    public BuildingParts RedParts;
    public BuildingParts RuinParts;
    public DungeonParts DungeonParts;

    public float chunkTownChance = 0.1f;
    public float ruinChance = 0.1f;
    public float forgetDistance = 10000;
    public float manageDistance = 4000;
    public float renderDistance = 1000;
    public float secondsBetweenUpdates = 1f;
    public Transform player;

    private float timeToGo;

    void Start()
    {
        timeToGo = Time.fixedTime + secondsBetweenUpdates;

        int seed = Random.Range(0,102312314);

        Dungeon dung = DungeonGenerator.Generate(10, 10, 2, 2, seed, 0,0, 20);
        dung.Initialize(0,0, DungeonParts, seed, this);
        dung.Build();

        /*int seed = UnityEngine.Random.Range(0, 100000000);
        Building building = BuildingGenerator.Generate(200/3, 200 / 3, 3, 100, 10, seed);
        building.Initialize(337, 9758, GreenParts, seed, this, false);
        building.Build();*/
    }

    void FixedUpdate()
    {
        if (Time.fixedTime >= timeToGo)
        {
            for(int i = 0; i<objects.Count; i++)
            {
                float dist = objects[i].DistFromPlayer(player.position);
                if(dist < renderDistance)
                {
                    if(!objects[i].SetActive(true))
                        return;
                } 
                else if(dist < manageDistance)
                {
                    objects[i].SetActive(false);
                }
                else if (dist < forgetDistance)
                {
                    objects[i].DestoryGameObject();
                }
                else
                {
                    int toRemove = objects[i].Forget();
                    objects.RemoveAt(i);
                    if(townBounds.Count > i)
                        townBounds.RemoveAt(i);
                    hashes.Remove(toRemove);
                }
            }

            timeToGo = Time.fixedTime + secondsBetweenUpdates;
        }
    }

    public bool ValidTerrainPos(Vector2 pos)
    {
        foreach (var town in townBounds)
        {
            if (town.Contains(pos))
            {
                return false;
            }
        }
        return true;
    }

    public void AddBounds(Rect r)
    {
        townBounds.Add(r);
    }

    public void TryAddTown(int chunkX, int chunkY, float chunkSideLength)
    {
        int chunkSeed = (new Vector2(hash(chunkX), hash(chunkY))).GetHashCode();

        Random.seed = chunkSeed+10;
        float spawn = Random.Range(0f, 1f);
        if (spawn < chunkTownChance)
        {
            float biome = HeightNoise.getBoimeData(chunkX * chunkSideLength, chunkY * chunkSideLength);
            if (hashes.Add(chunkSeed) && biome > -0.2 && biome < 0.2)
            {
                AddTown(chunkX, chunkY, chunkSideLength, chunkSeed);
            }
        }
    }

    private void AddTown(int chunkX, int chunkY, float chunkSideLength, int chunkSeed)
    {
        Random.seed = chunkSeed;
        int density = Random.Range(1, 101);
        bool ruin = (Random.Range(0f, 1f) < ruinChance);
        int stories = ruin ? 1 : Random.Range(1, 5);
        int size = Random.Range(2, 8);


        Building building = BuildingGenerator.Generate((int)(chunkSideLength / 3), (int)(chunkSideLength / 3), stories, density, size, chunkSeed);
        
        building.Initialize((int)(chunkX*chunkSideLength), (int)(chunkY*chunkSideLength), (ruin ? RuinParts : (Random.Range(0f, 1f) > 0.5 ? GreenParts : RedParts)), chunkSeed, this, ruin);

        objects.Add(building);
    }
}
