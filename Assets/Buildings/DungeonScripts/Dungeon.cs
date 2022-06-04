using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Object;

public class Dungeon : BuildingUnifier
{

    DungeonParts parts;
    int[,,] floors;
    bool[] firstOfLevel;
    int levels;
    (int, int) doorPos;
    float height;
    float heightDiff;

    override
    public bool isDungeon()
    {
        return false;
    }

    override
    public float DistFromPlayer(Vector3 playerPos)
    {
        return Vector3.Distance(playerPos, new Vector3(xPos, 0, yPos));
    }

    public Dungeon(int[,,] floors, int levels, (int, int) doorPos, float h, float heightDiff)
    {
        this.floors = floors;
        this.levels = levels;
        this.firstOfLevel = new bool[levels];
        this.doorPos = doorPos;
        height = h;
        this.heightDiff = heightDiff/3;
    }

    public void Initialize(int x, int y, DungeonParts parts, int seed, BuildingManager manager)
    {
        xPos = x;
        yPos = y;
        this.seed = seed;
        this.manager = manager;
        this.parts = parts;
        bldg = new GameObject("Dungeon").transform;
        bldg.gameObject.AddComponent<MeshRenderer>();
        bldg.gameObject.AddComponent<MeshFilter>();
        bldg.SetParent(manager.transform);
        bldg.position = new Vector3(xPos, height, yPos);
    }

    override
    public void Build()
    {
        if (!built)
        {
            bldg = new GameObject("Dungeon").transform;
            bldg.gameObject.AddComponent<MeshRenderer>();
            bldg.gameObject.AddComponent<MeshFilter>();
            bldg.SetParent(manager.transform);
            bldg.position = new Vector3(xPos, height, yPos);
        }

        Random.seed = seed;
        built = true;


        for (int i = 0; i < floors.GetLength(0); i++)
        {
            int count = 0;
            for (int x = (i%2==0?0 : floors.GetLength(1)-1); (i % 2 == 0 ? x< floors.GetLength(1) : x>0);)
            {
                for (int y = 0; y < floors.GetLength(2); y++)
                {
                    if (floors[i, x, y] != 0)
                    {
                        bool placeSpecial = false;
                        if((i==0|| floors[i - 1, x, y] != 0) && firstOfLevel[i] == false)
                        {
                            firstOfLevel[i] = true;
                            placeSpecial = true;
                        }

                        Transform f = Instantiate((placeSpecial && i != 0) ? parts.Ladder.transform : parts.Floor.transform, bldg.TransformPoint(x * 3f, i * 2.8f, y * 3), Quaternion.identity);
                        Transform c = Instantiate(parts.Ceiling.transform, bldg.TransformPoint(x * 3f, i * 2.8f + 2.5f, y * 3), Quaternion.identity);
                        f.SetParent(bldg);
                        c.SetParent(bldg);

                        

                        if (x == 0 || floors[i, x - 1, y] == 0)
                        {
                            placeSouthWall(f, (x == 0 && i == 0 && doorPos == (x, y)) ? parts.Door : parts.Walls);

                            
                        }
                        if (x == floors.GetLength(1) - 1 || floors[i, x + 1, y] == 0)
                        {
                            placeNorthWall(f, (x == floors.GetLength(1) - 1 && i == 0 && doorPos == (x, y)) ? parts.Door : parts.Walls);

                            
                        }
                        if (y == 0 || floors[i, x, y - 1] == 0)
                        {
                            placeEastWall(f, (y == 0 && i == 0 && doorPos == (x, y))  ? parts.Door : parts.Walls);

                            
                        }

                        if (y == floors.GetLength(2) - 1 || floors[i, x, y + 1] == 0)
                        {
                            placeWestWall(f, (y == floors.GetLength(2) - 1 && i == 0 && doorPos == (x, y)) ? parts.Door : parts.Walls);                            
                        }
                    }
                    int additionalFloors = (int)heightDiff + 2;
                    if (i == 0)
                    {
                        if (x == floors.GetLength(1) - 1)
                        {
                            Transform w = Instantiate(parts.Walls, bldg.TransformPoint(x * 3f -0.5f, (x,y) == doorPos ? 2.2f : -2.8f* additionalFloors, y * 3-3), Quaternion.Euler(0, -180, 0)).transform;
                            w.parent = bldg;
                            w.localScale = new Vector3(1, 3.2f/2.5f* (floors.GetLength(0) + ((x, y) == doorPos ? 0 : additionalFloors)) - ((x, y) == doorPos ? 1 : 0), 1);
                            
                            count++;
                        }

                        if (x == 0)
                        {
                            Transform w = Instantiate(parts.Walls, bldg.TransformPoint(x * 2.5f -2.5f, (x, y) == doorPos ? 2.2f : -2.8f * additionalFloors, y * 3), Quaternion.Euler(0, 0, 0)).transform;
                            w.parent = bldg;
                            w.localScale = new Vector3(1, 3.2f / 2.5f * (floors.GetLength(0) + ((x, y) == doorPos ? 0 : additionalFloors)) - ((x, y) == doorPos ? 1 : 0), 1);
                            count++;
                        }

                        if (y == 0)
                        {
                            Transform w = Instantiate(parts.Walls, bldg.TransformPoint(x * 3f - 3, (x, y) == doorPos ? 2.2f : -2.8f * additionalFloors, y * 3 - 2.5f), Quaternion.Euler(0, -90, 0)).transform;
                            w.parent = bldg;
                            w.localScale = new Vector3(1, 3.2f / 2.5f * (floors.GetLength(0) + ((x, y) == doorPos ? 0 : additionalFloors)) - ((x, y) == doorPos ? 1 : 0), 1);
                            count++;
                        }

                        if (y == floors.GetLength(2) - 1)
                        {
                            Transform w = Instantiate(parts.Walls, bldg.TransformPoint(x * 3f , (x, y) == doorPos ? 2.2f : -2.8f * additionalFloors, y * 3 -0.5f), Quaternion.Euler(0, 90, 0)).transform;
                            w.parent = bldg;
                            w.localScale = new Vector3(1, 3.2f / 2.5f * (floors.GetLength(0) + ((x, y) == doorPos ? 0 : additionalFloors)) - ((x, y) == doorPos ? 1 : 0), 1);
                            count++;
                        }
                        
                    }
                    
                }
                if (i % 2 == 0) x++;
                else x--;
            }
            if (i >= 2 && i % 2 == 0)
            {
                int offset = Random.Range(0, floors.GetLength(1));
                Transform o1 = Instantiate(parts.GetOuter(), bldg.TransformPoint(floors.GetLength(1) * 3, i * 2.8f, offset * 3 - 3f), Quaternion.Euler(0, 90, 0)).transform;
                o1.SetParent(bldg);

                offset = Random.Range(0, floors.GetLength(1));
                Transform o2 = Instantiate(parts.GetOuter(), bldg.TransformPoint(-6, i * 2.8f, offset * 3), Quaternion.Euler(0, -90, 0)).transform;
                o2.SetParent(bldg);

                offset = Random.Range(0, floors.GetLength(1));
                Transform o3 = Instantiate(parts.GetOuter(), bldg.TransformPoint(offset * 3 - 3 , i * 2.8f,  -6), Quaternion.Euler(0, -180, 0)).transform;
                o3.SetParent(bldg);

                offset = Random.Range(0, floors.GetLength(1));
                Transform o4 = Instantiate(parts.GetOuter(), bldg.TransformPoint(offset * 3, i * 2.8f, floors.GetLength(1) * 3), Quaternion.Euler(0, 0, 0)).transform;
                o4.SetParent(bldg);
            }
        }
        Transform roof = Instantiate(parts.roof, bldg.TransformPoint(floors.GetLength(1) * 3 -1.5f, floors.GetLength(0) * 2.7f, floors.GetLength(1)  * 3 -1.5f), Quaternion.Euler(0, 0, 0)).transform;
        roof.localScale = new Vector3(floors.GetLength(1)+1, floors.GetLength(1)/2, floors.GetLength(1)+1);
        roof.SetParent(bldg);
        Combine();
    }
    

    private void Combine()
    {
        bldg.gameObject.AddComponent<CountDown>();
    }
    override
    public bool SetActive(bool a)
    {
        bool wasBuild = built;
        if (a && !built)
        {
            Build();
        }
        if (built)
        {
            bldg.gameObject.SetActive(a);
        }
        return wasBuild;
    }

    override
    public void DestoryGameObject()
    {
        
        if (built)
        {
            Destroy(bldg.gameObject);
        }
        built = false;
    }

    override
    public int Forget()
    {
        
        if (built)
        {
            Destroy(bldg.gameObject);
        }
        
        return seed;
    }

    private Transform placeNorthWall(Transform parent, GameObject wall, float offset = 0)
    {
        Transform w = Instantiate(wall.transform, parent.TransformPoint(-0.5f, 0.3f + offset, -3f), Quaternion.Euler(0, -180, 0));
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
        Transform w = Instantiate(wall.transform, parent.TransformPoint(-3f, 0.3f + offset, -2.5f), Quaternion.Euler(0, -90f, 0));
        w.SetParent(parent);
        return w;
    }

    private Transform placeWestWall(Transform parent, GameObject wall, float offset = 0)
    {
        Transform w = Instantiate(wall.transform, parent.TransformPoint(0, 0.3f + offset, -0.5f), Quaternion.Euler(0, 90f, 0));
        w.SetParent(parent);
        return w;
    }
    

}
