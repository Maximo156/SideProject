using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingCubesProject;
using System.Threading;

public class CaveControl : MonoBehaviour
{
    public List<GameObject> Objects;
    public List<float> Chances;
    public Material material;
    GameObject cave;
    BoxCollider post;
    VoxelArray caveVox;
    CaveGenerator.GenInfo info;

    public void Start()
    {
        //post = gameObject.GetComponent<BoxCollider>();
        //post.enabled = false;
    }

    public void CaveActive()
    {

        if (cave == null)
        {
            genCave();
            MakeCave();
            StartCoroutine(Spawn());

        }
        else
        {
            cave.SetActive(true);
            //post.enabled = true;
        }
    }

    public void CaveInactive()
    {
        if (cave != null)
        {
            //post.enabled = false;
            cave.SetActive(false);
        }
    }

    public void genCave()
    {
        Random.seed = transform.position.GetHashCode();
        caveVox = CaveGenerator.GenerateCave(30, 30, 30, out info, 70);
    }

    private void MakeCave()
    {
        StartCoroutine(MakeCaveRoutine());
    }

    bool meshBaked = false;
    private IEnumerator MakeCaveRoutine()
    {
        Marching marching = new MarchingCubes();

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        marching.Surface = 0.2f;
        yield return StartCoroutine(marching.Generate(caveVox.Voxels, verts, indices));

        CreateMesh32(verts, normals, indices, new Vector3(-98.03f, -105.91f, -41.89f));
        yield return null;

        startMechColliderThread(cave.GetComponent<MeshFilter>().mesh.GetInstanceID());
        yield return new WaitWhile(() => !meshBaked);
        cave.AddComponent<MeshCollider>();
    }

    private void startMechColliderThread(int id)
    {
        ThreadStart threadStart = delegate
        {
            AddMeshCollider(id);
        };
        new Thread(threadStart).Start();
    }

    private void AddMeshCollider(int id)
    {
        Physics.BakeMesh(id, false);
        meshBaked = true;
    }

    private void CreateMesh32(List<Vector3> verts, List<Vector3> normals, List<int> indices, Vector3 position)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);

        if (normals.Count > 0)
            mesh.SetNormals(normals);
        else
            mesh.RecalculateNormals();

        mesh.RecalculateBounds();

        GameObject go = new GameObject("CaveMesh");
        go.transform.parent = transform;
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<Renderer>().material = material;
        go.GetComponent<MeshFilter>().mesh = mesh;
        go.transform.localPosition = position;
        go.AddComponent<CaveMine>();
        cave = go;
    }

    public void test()
    {
        for(int i = 1; i< caveVox.Width-1; i++)
        {
            for(int k =1; k<caveVox.Height-1; k++)
            {
                for(int m = 1; m<caveVox.Depth-1; m++)
                {
                    caveVox[i, k, m] = 1;
                }
            }
        }
        MakeCave();
    }

    public void Mine(Vector3Int pos)
    {
        if (pos.x == 0 || pos.y == 0 || pos.z == 0) return;
        GameObject old = cave;
        caveVox[pos.x, pos.y, pos.z] = 0;
        for (int a = Mathf.Max(pos.x - 1, 0); a <= Mathf.Min(pos.x + 1, caveVox.Width - 1); a++)
        {
            for (int b = Mathf.Max(pos.y - 1, 0); b <= Mathf.Min(pos.y + 1, caveVox.Height - 1); b++)
            {
                for (int c = Mathf.Max(pos.z - 1, 0); c <= Mathf.Min(pos.z + 1, caveVox.Depth - 1); c++)
                {
                    caveVox[a, b, c] = 0.5f;
                }
            }
        }

        caveVox[pos.x, pos.y, pos.z] = 1;
        MakeCave();
        Destroy(old);
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            if (cave == null || cave.GetComponent<MeshCollider>() == null) yield return null;
            else break;
        }
        for(int i = info.xRange.Item1; i<info.xRange.Item2; i++)
        {
            for (int k = info.yRange.Item1; k < info.yRange.Item2; k++)
            {
                for (int m = info.zRange.Item1; m < info.zRange.Item2; m++)
                {
                    if (info.intermediate[i, k, m] == 1)
                    {
                        Physics.Raycast(cave.transform.position + new Vector3(i, k, m) * info.scale, new Vector3(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2)), out RaycastHit hitInfo, 10);
                        if (hitInfo.transform != null && hitInfo.transform.name == "CaveMesh")
                        {
                            int overlapTestBoxSize = 1;
                            Vector3 overlapTestBoxScale = new Vector3(overlapTestBoxSize, overlapTestBoxSize, overlapTestBoxSize);
                            Collider[] collidersInsideOverlapBox = new Collider[2];
                            int numberOfCollidersFound = Physics.OverlapBoxNonAlloc(hitInfo.point, overlapTestBoxScale, collidersInsideOverlapBox);
                            if (numberOfCollidersFound < 2)
                            {
                                float c = Random.Range(0f, 1f);
                                float t = Chances[0];
                                int index = 0;
                                while (t < c)
                                {
                                    index++;
                                    t += Chances[Mathf.Min(Chances.Count-1, index)];
                                }
                                Transform prim = Instantiate(Objects[Mathf.Min(Chances.Count - 1, index)]).transform;
                                
                                prim.position = hitInfo.point + hitInfo.normal * 0.2f;
                                prim.up = hitInfo.normal;
                                prim.parent = transform;
                                yield return null;
                            }
                        }
                    }
                }
            }
        }
        yield break;
    }
}
