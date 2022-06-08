using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Object;

public class Building : BuildingUnifier
{
    int foundationScale = 5;
    float[] sections;
    (int,int)[] sectionsDoors;
    int[,] sectionsLadders;
    BuildingParts parts;
    //MeshRenderer mr;
    //MeshFilter mf;
    
    bool ruin;

    override
    public bool isBuilding()
    {
        return true;
    }

    public Building(int[,,] floors, int sections)
    {
        this.floors = floors;
        this.sections = new float[sections];
        this.sectionsDoors = new (int, int)[sections];
        this.sectionsLadders = new int[floors.GetLength(0), sections];
    }
    float combineStart = 0;
    

    public void Initialize(int x, int y, BuildingParts parts, int seed, BuildingManager manager, bool ruin = false)
    {
        xPos = x;
        yPos = y;
        this.parts = parts;
        this.seed = seed;
        this.ruin = ruin;
        this.manager = manager;
        
        bldg = new GameObject("Town").transform;
        details = new GameObject("Details").transform;
        container = new GameObject("TownContainer").transform;
        container.tag = "Container";

        container.gameObject.AddComponent<MeshRenderer>();
        container.gameObject.AddComponent<MeshFilter>();

        bldg.SetParent(container);
        details.SetParent(container);
        container.SetParent(manager.transform);

        container.position = new Vector3(xPos, 0, yPos);

        CatSecs();
    }

    override
    public void Build()
    {
        if (!built)
        {
            bldg = new GameObject("Town").transform;
            details = new GameObject("Details").transform;
            container = new GameObject("TownContainer").transform;
            container.tag = "Container";


            container.gameObject.AddComponent<MeshRenderer>();
            container.gameObject.AddComponent<MeshFilter>();

            bldg.SetParent(container);
            details.SetParent(container);
            container.SetParent(manager.transform);

            container.position = new Vector3(xPos, 0, yPos);
        }

        Random.seed = seed;
        built = true;

        for(int i = 0; i<floors.GetLength(0); i++)
        {
            for (int x = 0; x < floors.GetLength(1); x++)
            {
                for (int y = 0; y < floors.GetLength(2); y++)
                {
                    if (floors[i, x, y] != 0)
                    {
                        
                        int sectionNumber = floors[i, x, y];
                        bool placeDoor = (i == 0 && ((x, y) == sectionsDoors[sectionNumber - 1]));
                        bool ladder = false;
                        if(i != 0 && sectionsLadders[i,sectionNumber-1] == 0)
                        {
                            sectionsLadders[i, sectionNumber - 1] = 1;
                            ladder = true;
                        }

                        Transform f = Instantiate(ladder ? parts.Ladder.transform : parts.Floor.transform, bldg.TransformPoint(x * 3f, i * 2.5f + sections[sectionNumber-1] +0.25f, y * 3), Quaternion.identity);
                        f.SetParent(bldg);
                        if (placeDoor)
                        {
                            Destroy(f.GetComponent<DetailSpawner>());
                        }
                        

                        bool north = false;
                        bool south = false;
                        bool east = false;
                        bool west = false;

                        if (x == 0 || floors[i,x-1,y] == 0)
                        {
                            placeSouthWall(f, placeDoor ? parts.GetDoor() : parts.GetWall());
                            placeDoor = false;
                            south = true;

                            if (i == 0)
                            {
                                Transform w = placeSouthWall(f, parts.GetFoundation(), -2.2f * foundationScale - 0.3f);
                                w.localScale = new Vector3(1, foundationScale, 1);
                            }
                        }
                        if (x == floors.GetLength(1)-1 || floors[i, x + 1, y] == 0)
                        {
                            placeNorthWall(f, placeDoor ? parts.GetDoor() : parts.GetWall());
                            placeDoor = false;
                            north = true;

                            if (i == 0)
                            {
                                Transform w = placeNorthWall(f, parts.GetFoundation(), -2.2f * foundationScale - 0.3f);
                                w.localScale = new Vector3(1, foundationScale, 1);
                            }
                        }
                        if (y == 0 || floors[i, x , y-1] == 0)
                        {
                            placeEastWall(f, placeDoor ? parts.GetDoor() : parts.GetWall());
                            placeDoor = false;
                            east = true;

                            if (i == 0)
                            {
                                Transform w = placeEastWall(f, parts.GetFoundation(), -2.2f * foundationScale - 0.3f);
                                w.localScale = new Vector3(1, foundationScale, 1);
                            }
                        }
                        
                        if (y == floors.GetLength(1) - 1 || floors[i, x, y + 1] == 0)
                        {
                            placeWestWall(f, placeDoor ? parts.GetDoor() : parts.GetWall());
                            placeDoor = false;
                            west = true;

                            if (i == 0)
                            {
                                Transform w = placeWestWall(f, parts.GetFoundation(), -2.2f * foundationScale - 0.3f);
                                w.localScale = new Vector3(1, foundationScale, 1);
                            }
                        }

                        if((i == floors.GetLength(0) - 1 || floors[i + 1, x, y] == 0) && !ruin)
                        {
                            if (north && south && east && west)
                            {
                                Transform c = Instantiate(parts.Roofs[2].transform, f.TransformPoint(0, 2f, 0), Quaternion.Euler(0, 0, 0));
                                c.SetParent(f);
                            }

                            else if(north && south)
                            {
                                Transform c = Instantiate(parts.Roofs[1].transform, f.TransformPoint(0, 2.5f, -3f), Quaternion.Euler(0, 90, 0));
                                c.SetParent(f);
                            }

                            else if (east && west)
                            {
                                Transform c = Instantiate(parts.Roofs[1].transform, f.TransformPoint(0, 2.5f, 0), Quaternion.Euler(0, 0, 0));
                                c.SetParent(f);
                            }
                            else if (east)
                            {
                                Transform c = Instantiate(parts.Roofs[0].transform, f.TransformPoint(-3f, 2.5f, 0), Quaternion.Euler(0, -90, 0));
                                c.SetParent(f);
                            }
                            else if (west)
                            {
                                Transform c = Instantiate(parts.Roofs[0].transform, f.TransformPoint(0, 2.5f, -3f), Quaternion.Euler(0, 90, 0));
                                c.SetParent(f);
                            }
                            else if (north)
                            {
                                Transform c = Instantiate(parts.Roofs[0].transform, f.TransformPoint(-3f, 2.5f, -3f), Quaternion.Euler(0, 180, 0));
                                c.SetParent(f);
                            }
                            else if (south)
                            {
                                Transform c = Instantiate(parts.Roofs[0].transform, f.TransformPoint(0, 2.5f, 0), Quaternion.Euler(0, 0, 0));
                                c.SetParent(f);
                            }

                            else
                            {
                                Transform c = Instantiate(parts.Roofs[3].transform, f.TransformPoint(0, 2.5f, 0), Quaternion.Euler(0, 0, 0));
                                c.SetParent(f);
                            }
                            Transform d = Instantiate(parts.Ceiling.transform, f.TransformPoint(0, 2.20f, 0), Quaternion.Euler(0, 0, 0));
                            d.SetParent(f);
                        }
                    }
                    else if (i == 0 && Random.Range(0f, 1f) < (ruin ? 0.01 : 0.005))
                    {
                        Vector3 loc = bldg.position;
                        loc.x += x * 3f + Random.Range(0f, 1.5f);
                        loc.z += y * 3f + Random.Range(0f, 1.5f);
                        loc.y = HeightNoise.getHeight(loc)[0];
                        Transform f = Instantiate(parts.GetRandom().transform, loc, Quaternion.identity);
                        f.SetParent(bldg);
                    }
                }
            }
        }
        
        Transform entityParent = GameObject.Find("EntityParent").transform;
        for(int i = 0; i< parts.spawnAttempts; i++)
        {
            GameObject type = parts.GetSpawnable();
            if(type != null)
            {
                Vector3 loc = bldg.transform.position +new Vector3(Random.Range(0, floors.GetLength(1) * 3), 0, Random.Range(0, floors.GetLength(2) * 3));
                GameObject newInstance = Instantiate(type, loc, Quaternion.Euler(0, 0, 0), entityParent);
                newInstance.transform.position = loc;
            }
        }

        Combine();
    }

    private Transform placeNorthWall(Transform parent, GameObject wall, float offset = 0)
    {
        Transform w = Instantiate(wall.transform, parent.TransformPoint(-0.5f, 0.3f+offset, -3f), Quaternion.Euler(0, -180, 0));
        w.SetParent(parent);
        return w;
    }

    private Transform placeSouthWall(Transform parent, GameObject wall, float offset = 0)
    {
        Transform w = Instantiate(wall.transform, parent.TransformPoint(-2.5f, 0.3f + offset, 0), Quaternion.Euler(0, 0, 0));
        w.SetParent(parent);
        return w;
    }

    private Transform placeEastWall(Transform parent, GameObject wall, float offset = 0)
    {
        Transform w = Instantiate(wall.transform, parent.TransformPoint(-3f, 0.3f + offset, -2.5f), Quaternion.Euler(0,-90f,0));
        w.SetParent(parent);
        return w;
    }

    private Transform placeWestWall(Transform parent, GameObject wall, float offset = 0)
    {
        Transform w = Instantiate(wall.transform, parent.TransformPoint(0, 0.3f + offset, -0.5f), Quaternion.Euler(0, 90f, 0));
        w.SetParent(parent);
        return w;
    }

    private void CatSecs()
    {
        for (int i = 0; i<floors.GetLength(1); i++)
        {
            for(int k = 0; k<floors.GetLength(2); k++)
            {
                if (floors[0, i, k] != 0)
                {
                    Vector3 loc = bldg.position;
                    loc.x += i * 3f;
                    loc.z += k * 3f;
                    if (sections[floors[0, i, k] - 1] < HeightNoise.getHeight(loc)[0])
                    {
                        sections[floors[0, i, k] - 1] = HeightNoise.getHeight(loc)[0];
                        sectionsDoors[floors[0, i, k] - 1] = (i, k);
                    }
                }
            }
        }
    }
   
}
