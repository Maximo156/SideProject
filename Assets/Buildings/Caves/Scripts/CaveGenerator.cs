using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingCubesProject;

public static class CaveGenerator
{
    static VoxelArray cave;
    static int[,,] intermediate;
    static int xMax;
    static int yMax;
    static int zMax;
    static int maxDepth = 40;
    static int scale = 3;
    static float branchChance = 0.1f;
    public static VoxelArray GenerateCave(int x, int y, int z, out int[,,] o)
    {
        if (scale % 2 == 0) scale++;
        cave = new VoxelArray(x*scale, y*scale, z*scale);
        intermediate = new int[x,y,z];
        xMax = x - 2;
        yMax = y - 2;
        zMax = z - 2;


        Step(x-1, y - 2, z / 2);

        Scale();
        Smooth();
        Smooth();
        o = intermediate;
        return cave;
    }

    private static void Step(int x, int y, int z, int depth = 0, bool canBranch = true)
    {
        if (depth > maxDepth) return;
        intermediate[x, y, z] = 1;
        int xNext;
        int yNext;
        int zNext;
        int count = 0;
        bool notValidChoice;
        do
        {
            yNext = y;
            zNext = z;
            xNext = x;
            if (Random.Range(0f, 1f) > 0.5)
            {
                if (z != 0 && z != zMax + 1)
                    xNext = x + Random.Range(x <= 1 ? 0 : -1, x >= xMax ? 1 : 2);
                if (xNext == x && x != 0 && x != xMax + 1)
                    zNext = z + Random.Range(z <= 1 ? 0 : -1, z >= zMax ? 1 : 2);
            }
            else
            {
                if (x != 0 && x != xMax + 1)
                    zNext = z + Random.Range(z <= 1 ? 0 : -1, z >= zMax ? 1 : 2);
                if (zNext == z && z != 0 && z != zMax + 1)
                    xNext = x + Random.Range(x <= 1 ? 0 : -1, x >= xMax ? 1 : 2);
            }
            if (xNext != x || zNext != z)
            {
                yNext = y + Random.Range(y <= 1 ? 0 : -1, y >= yMax ? 1 : 2);
            }
            count++;
           notValidChoice = intermediate[xNext, yNext, zNext] == 1 || intermediate[xNext, yNext + 1, zNext] == 1 || intermediate[xNext, yNext - 1, zNext] == 1 || yNext - 2 < 0 ? false : intermediate[xNext, yNext - 2, zNext] == 1 || yNext + 2 > yMax + 1 ? false : intermediate[xNext, yNext + 2, zNext] == 1;
        } while (count < 20 && (intermediate[xNext, yNext, zNext] == 1 || intermediate[xNext, yNext+1, zNext] == 1 || intermediate[xNext, yNext-1, zNext] == 1));
        if (count == 20) return;
        Step(xNext, yNext, zNext, ++depth, canBranch);
        if(canBranch && Random.Range(0f, 1f) < branchChance)
        {
            Step(xNext, yNext, zNext, ++depth, false);
        }
    }

    private static void Scale()
    {
        for (int i = 0; i < intermediate.GetLength(0); i++)
        {
            for (int k = 0; k < intermediate.GetLength(1); k++)
            {
                for (int m = 0; m < intermediate.GetLength(2); m++)
                {
                    if (intermediate[i, k, m] == 1)
                    {
                        int halfScale = Mathf.FloorToInt(scale / 2);
                        int x = i * scale + halfScale;
                        int y = k * scale + halfScale;
                        int z = m * scale + halfScale;
                        cave[x, y, z] = 1;
                        for (int a = Mathf.Max(x - halfScale, 0); a <= Mathf.Min(x + halfScale, cave.Width - 1); a++)
                        {
                            for (int b = Mathf.Max(y - halfScale, 0); b <= Mathf.Min(y + halfScale, cave.Height - 1); b++)
                            {
                                for (int c = Mathf.Max(z - halfScale, 0); c <= Mathf.Min(z + halfScale, cave.Depth - 1); c++)
                                {
                                    cave[a, b, c] = 1;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static void Smooth() { 
            for (int i = 0; i<cave.Width; i++)
            {
                for (int k = 0; k<cave.Height; k++)
                {
                    for (int m = 0; m<cave.Depth; m++)
                    {
                        //o += cave[i, k, m];
                        float sum = 0;
                        int count = 0;
                        /*
                        for (int a = Mathf.Max(i - 1, 0); a <= Mathf.Min(i + 1, cave.Width - 1); a++)
                        {
                            for (int b = Mathf.Max(k - 1, 0); b <= Mathf.Min(k + 1, cave.Height -1); b++)
                            {
                                for (int c = Mathf.Max(m - 1, 0); c <= Mathf.Min(m + 1, cave.Depth - 1); c++)
                                {
                                    sum += cave[a, b, c];
                                    count++;
                                }
                            }
                        }*/
                        sum += i == cave.Width - 1 ? 0 : cave[i + 1, k, m];
                        sum += i == 0 ? 0 : cave[i - 1, k, m];
                        sum += k == cave.Height - 1 ? 0 : cave[i, k + 1, m];
                        sum += k == 0 ? 0 : cave[i, k - 1, m];
                        sum += m == cave.Depth - 1 ? 0 : cave[i, k, m + 1];
                        sum += m == 0 ? 0 : cave[i, k, m - 1];
                        sum += cave[i, k, m] * 2;
                        count = 8;

                        cave[i, k, m] = sum / count< 0.01 ? 0 : sum / count;
                    }
                }
            }
        }

}
