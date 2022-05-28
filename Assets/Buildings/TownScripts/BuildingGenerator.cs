using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingGenerator
{
    static int[,,] floor;
    static int count = 2;
    static int maxBuildingSize = 2;
    public static Building Generate(int xSize, int ySize, int stories, int baseRec, int recSizeMax, int seed)
    {
        Random.seed = seed;
        floor = new int[stories, xSize, ySize];
        for (int s = 0; s < stories; s++)
        {
            for (int i = 0; i < baseRec; i++)
            {
                int x = Random.Range(0, xSize - recSizeMax);
                int y = Random.Range(0, ySize - recSizeMax);
                int xEnd = Mathf.Min(xSize - 1, x + Random.Range(1, recSizeMax + 1));
                int yEnd = Mathf.Min(ySize - 1, y + Random.Range(1, recSizeMax + 1));

                bool changed = false;
                for (int xInd = x; xInd < xEnd; xInd++)
                {
                    for (int yInd = y; yInd < yEnd; yInd++)
                    {
                        if (s == 0 || floor[s - 1, xInd, yInd] != 0)
                        {

                            floor[s, xInd, yInd] = (s == 0) ? 1 : floor[s - 1, xInd, yInd];
                            changed = true;
                        }
                    }
                }
                
                if (!changed)
                {
                    i--;
                }
            }
            if (s == 0)
            {
                Categorize();
            }
        }
        

        return (new Building(floor, count - 1));
    }

    private static void Categorize()
    {
        for(int x = 0; x<floor.GetLength(1); x++)
        {
            for(int y = 0; y < floor.GetLength(2); y++){
                if(floor[0,x,y] == 1)
                {
                    floor[0, x, y] = count;
                    Propigate(x, y, 0);
                    count++;
                }
            }
        }
    }

    private static void Propigate(int x, int y, int depth)
    {
        if (x - 1 >= 0 && floor[0, x - 1, y] == 1)
        {
            floor[0, x - 1, y] = count;
            Propigate(x - 1, y, depth +1);
        }

        if (x + 1 < floor.GetLength(1) && floor[0, x + 1, y] == 1)
        {
            floor[0, x + 1, y] = count;
            Propigate(x + 1, y, depth + 1);
        }

        if (y - 1 >= 0 && floor[0, x, y - 1] == 1)
        {
            floor[0, x, y - 1] = count;
            Propigate(x , y - 1, depth + 1);
        }

        if (y + 1 < floor.GetLength(2) && floor[0, x , y + 1] == 1)
        {
            floor[0, x , y + 1] = count;
            Propigate(x, y + 1, depth + 1);
        }
    }
}
