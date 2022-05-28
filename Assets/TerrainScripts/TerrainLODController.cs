using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TerrainLODController : MonoBehaviour
{
    public objectInformation[] objectInfo;
    public static int LowDistance=4000;
    public static int MedDistance=300;
    public static int HighDistance = 100;

    public Transform player;

    private Dictionary<Vector3, objectInstance> objectInstances;
    private List<Vector3> toDelete;

    public void Start()
    {
        objectInstances = new Dictionary<Vector3, objectInstance>();
        toDelete = new List<Vector3>();
    }

    public void AddInstance(int type, Vector3 pos, Transform parent)
    {
        objectInformation info = objectInfo[type];
        objectInstances[pos] = new objectInstance(info.HighMesh, info.MedMesh, info.LowMesh, info.material, info.changableLOD, pos, parent);
    } 

    public void UpdateLOD()
    {
        foreach(var obj in objectInstances)
        {
            if (obj.Value.needsDelete())
            {
                toDelete.Add(obj.Key);
                continue;
            }
            obj.Value.updateLOD(player.position);
        }
        foreach (var key in toDelete)
        {
            objectInstances.Remove(key);
        }
        toDelete.Clear();
    }

    [Serializable]
    public class objectInformation{
        public Mesh HighMesh;
        public Mesh MedMesh;
        public Mesh LowMesh;
        public Material material;
        public bool changableLOD;
    }
    
    public class objectInstance
    {
        private Mesh HighMesh;
        private Mesh MedMesh;
        private Mesh LowMesh;
        private bool changableLOD;
        private GameObject instance;
        private MeshFilter mf;
        private MeshCollider mc;

        public objectInstance(Mesh HighMesh, Mesh MedMesh, Mesh LowMesh, Material mat, bool changableLOD, Vector3 pos, Transform parent)
        {
            this.HighMesh = HighMesh;
            this.MedMesh = MedMesh;
            this.LowMesh = LowMesh;
            this.changableLOD = changableLOD;
            instance = new GameObject("TerrainObjectInstance", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
            instance.transform.parent = parent;
            mf = instance.GetComponent<MeshFilter>();
            mc = instance.GetComponent<MeshCollider>();
            instance.GetComponent<MeshRenderer>().material = mat;
            instance.transform.position = pos;
        }

        public bool needsDelete()
        {
            return instance == null;
        }

        public void updateLOD(Vector3 pos)
        {
            if (changableLOD)
            {
                float dist = Vector3.Distance(pos, instance.transform.position);
                if (dist < HighDistance)
                {
                    mf.mesh = HighMesh;
                    mc.sharedMesh = MedMesh;
                } 
                else if (dist < MedDistance)
                {
                    mf.mesh = MedMesh;
                }
                else if (dist < LowDistance)
                {
                    mf.mesh = LowMesh;
                }
                else
                {
                    mf.mesh = null;
                }
            }
        }
    }
}
