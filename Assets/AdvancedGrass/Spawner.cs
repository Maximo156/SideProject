using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
public class ObjData
{
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;
    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(pos, rot, scale);
        }
    }
    public ObjData(Vector3 pos, Vector3 scale, Quaternion rot)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}*/

public class Spawner : MonoBehaviour
{
    public Transform player;
    public float scale;
    public float offset = 0.2f;
    public int instances;
    public Vector3 maxPos;
    public Mesh instanceMesh;
    public Material instanceMaterial;
    public bool instanced = false;
    public int subMeshIndex = 0;

    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    private int cachedInstanceCount = -1;
    private int cachedSubMeshIndex = -1;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;

    // Start is called before the first frame update
    void Start()
    {
        print(Matrix4x4.TRS(new Vector3(2, 4, 6), Quaternion.Euler(0, 0, 0), new Vector3(3, 3, 3)));
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        //UpdateBuffers();
    }

    // Update is called once per frame
    void Update()
    {
            // Update starting position buffer
        //if (cachedInstanceCount != instances || cachedSubMeshIndex != subMeshIndex)
            //UpdateBuffers();

        // Pad input
        /*if (Input.GetAxisRaw("Horizontal") != 0.0f)
            instances = (int)Mathf.Clamp(instances + Input.GetAxis("Horizontal") * 40000, 1.0f, 5000000.0f);*/

        // Render
        Graphics.DrawMeshInstancedIndirect(instanceMesh, subMeshIndex, instanceMaterial, new Bounds(player.position, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
        
    }

    float last = 0;
    void FixedUpdate()
    {
        if (Time.time - last < 10)
            return;
        last = Time.time;
        // Ensure submesh index is in range
        if (instanceMesh != null)
            subMeshIndex = Mathf.Clamp(subMeshIndex, 0, instanceMesh.subMeshCount - 1);

        // Positions
        if (positionBuffer != null)
            positionBuffer.Release();
        positionBuffer = new ComputeBuffer(instances, 3*sizeof(float));
        Vector3[] positions = new Vector3[instances];

        for (int i = 0; i <instances; i++)
        {
            Vector3 Position = new Vector3(Random.Range(-maxPos.x, maxPos.x), 0, Random.Range(-maxPos.z, maxPos.z));
            Position += player.position;
            Position.y = HeightNoise.getHeight(Position)[0]+ offset;
            //print(Position);
            positions[i] = Position;
        }
        positionBuffer.SetData(positions);
        instanceMaterial.SetFloat("_scale", scale);
        instanceMaterial.SetBuffer("positionBuffer", positionBuffer);

        // Indirect args
        if (instanceMesh != null)
        {
            args[0] = (uint)instanceMesh.GetIndexCount(subMeshIndex);
            args[1] = (uint)instances;
            args[2] = (uint)instanceMesh.GetIndexStart(subMeshIndex);
            args[3] = (uint)instanceMesh.GetBaseVertex(subMeshIndex);
        }
        else
        {
            args[0] = args[1] = args[2] = args[3] = 0;
        }
        argsBuffer.SetData(args);

        cachedInstanceCount =instances;
        cachedSubMeshIndex = subMeshIndex;
    }

}

