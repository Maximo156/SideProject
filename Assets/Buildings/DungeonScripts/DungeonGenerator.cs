using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonGenerator
{
    static int[,,] floor;
    public static Dungeon Generate(int xSize, int ySize, int rooms, int maxRoomSize, int seed, int xPos, int yPos, int stories = 1)
    {
        Random.seed = seed;
        floor = new int[stories, xSize, ySize];
        (int, int)[,] roomLocs = new (int, int)[stories, rooms];

        for (int s = 0; s < stories; s++)
        {
            for (int i = 0; i < rooms; i++)
            {
                int x = Random.Range(0, xSize - maxRoomSize);
                int y = Random.Range(0, ySize - maxRoomSize);
                int xEnd = Mathf.Min(xSize - 1, x + Random.Range(3, maxRoomSize));
                int yEnd = Mathf.Min(ySize - 1, y + Random.Range(3, maxRoomSize));

                for (int xInd = x; xInd < xEnd; xInd++)
                {
                    for (int yInd = y; yInd < yEnd; yInd++)
                    {
                            floor[s, xInd, yInd] = 1;
                            roomLocs[s, i] = (x, y);
                    }
                }
            }
        }

        (int, int) doorPos = (0,0);
        float highest = HeightNoise.getHeight(new Vector3(0, 0, 0))[0];
        float lowest = highest;
        for (int x = 0; x<xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Vector3 loc = new Vector3(xPos + x * 3f, 0, yPos+ y * 3f);
                float h = HeightNoise.getHeight(loc)[0];
                if (highest < h)
                {
                    doorPos = (x, y);
                    highest =h;
                }
                lowest = Mathf.Min(lowest, h);
            }
        }

        genPaths(0, doorPos, roomLocs[0,0]);
        for (int s = 0; s < roomLocs.GetLength(0); s++)
        {
            for (int i = 0; i < roomLocs.GetLength(1) - 1; i++)
            {
                genPaths(s, roomLocs[s, i], roomLocs[s, i + 1]);
            }

            if (roomLocs.GetLength(1) > 2)
            {
                genPaths(s, roomLocs[s, 0], roomLocs[s, roomLocs.GetLength(1) - 1]);
            }
        }


        return new Dungeon(floor, stories, doorPos, highest, highest-lowest);
    }


    private static float Dist((int, int) start, (int, int) end)
    {
        return Mathf.Sqrt(Mathf.Pow(start.Item1 - end.Item1, 2) + Mathf.Pow(start.Item2 - end.Item2, 2));
    }

    private static void genPaths(int level, (int, int) start, (int,int) end)
    {

        int x = start.Item1;
        int y = start.Item2;
        bool movex = Random.Range(0, 1f) > 0.5;
        int count = 0;
        while (!(x == end.Item1 && y == end.Item2) && count < 20) 
        {
            int xDir = (x == end.Item1 ? 0 : (int)Mathf.Sign(end.Item1-x));
            int yDir = (y == end.Item2 ? 0 : (int)Mathf.Sign(end.Item2-y));


            movex = Random.Range(0, 1f) > 0.1 ? movex : !movex;

            movex = (xDir == 0 ? false : movex);
            movex = (yDir == 0 ? true : movex);


            if(xDir != 0 && floor[level, x + xDir, y] == 1)
            {
                movex = true;
            } 
            else if(yDir != 0 && floor[level, x, y+yDir] == 1)
            {
                movex = false;
            }


            floor[level, x, y] = 1;

            if (movex)
            {
                x += xDir;
            }
            else
            {
                y += yDir;
            }
        }
    }
}
