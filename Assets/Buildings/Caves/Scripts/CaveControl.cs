using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingCubesProject;

public class CaveControl : MonoBehaviour
{
    public Material material;
    GameObject cave;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CaveActive()
    {
        if (cave == null) MakeCave();
        else cave.SetActive(true);
    }

    public void CaveInactive()
    {
        if (cave != null) cave.SetActive(false);
    }


    private void MakeCave()
    {
        Random.seed = transform.position.GetHashCode();
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

        CreateMesh32(verts, normals, indices, new Vector3(-68.25f, -75.37f, -26.9f));

        this.cave.AddComponent<MeshCollider>();
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
        cave = go;
        //meshes.Add(go);
    }
}
