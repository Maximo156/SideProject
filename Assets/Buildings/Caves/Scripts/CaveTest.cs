using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingCubesProject;

public class CaveTest : MonoBehaviour
{
    public Material material;
    // Start is called before the first frame update
    void Start()
    {
        VoxelArray cave = CaveGenerator.GenerateCave(20, 20, 20, out int[,,] o);

        Marching marching = new MarchingCubes();

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        marching.Surface = 0.2f;

        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        marching.Generate(cave.Voxels, verts, indices);


        //var position = new Vector3(-width / 2, -height / 2, -depth / 2);

        CreateMesh32(verts, normals, indices, Vector3.zero);
        
        for(int i = 0; i<o.GetLength(0); i++)
        {
            for (int k = 0; k < o.GetLength(1); k++)
            {
                for (int m = 0; m < o.GetLength(2); m++)
                {
                    if (o[i, k, m] >= 0.5)
                    {
                        Transform c = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                        c.position = new Vector3(i, k, m);
                        c.parent = transform;
                    }
                        
                }
            }
        }
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

        GameObject go = new GameObject("Mesh");
        go.transform.parent = transform;
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<Renderer>().material = material;
        go.GetComponent<MeshFilter>().mesh = mesh;
        go.transform.localPosition = position;

        //meshes.Add(go);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
