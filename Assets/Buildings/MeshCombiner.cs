using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshCombiner : MonoBehaviour
{

    public void Awake()
    {
        StartCoroutine(Combine()); 
    }


    IEnumerator Combine()
    {
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        yield return null;
        int count = 0;
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if(meshFilter == null || meshFilter.gameObject.GetComponent<HealthScript>() != null)  continue; 
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (!meshRenderer ||
                !meshFilter.sharedMesh ||
                meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }

            Matrix4x4 parent = gameObject.transform.worldToLocalMatrix;
            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].name);
                if (materialArrayIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    materialArrayIndex = materials.Count - 1;
                }
                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = parent * meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);

            }
            count++;
            if(count > 50)
            {
                count = 0;
                yield return null;
            }
        }
        // Get / Create mesh filter & renderer
        MeshFilter meshFilterCombine = gameObject.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
        {
            meshFilterCombine = gameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
        {
            meshRendererCombine = gameObject.AddComponent<MeshRenderer>();
        }

        // Combine by material index into per-material meshes
        // also, Create CombineInstance array for next step
        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
            yield return null;
        }
        

       
        // Assign materials
        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;
        

        // Combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);
        yield return null;

        // Destroy other meshes
        count = 0;
        foreach (Mesh oldMesh in meshes)
        {
            oldMesh.Clear();
            DestroyImmediate(oldMesh);
            count++;
            if (count > 1)
            {
                count = 0;
                yield return null;
            }
        }


        for (int i = 1; i < meshFilters.Length; i++)
        {
            if (meshFilters[i] == null ||  meshFilters[i].gameObject.GetComponent<HealthScript>() != null) continue;
            DestroyImmediate(meshFilters[i].gameObject);
            //meshFilters[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        //DetailSpawner[] s = GetComponentsInChildren<DetailSpawner>();
        /*foreach(var sp in s)
        {
            sp.Call();
        }*/
        gameObject.isStatic = true;
        Destroy(gameObject.GetComponent<MeshCombiner>());
    }

    private int Contains(ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).name == searchName)
            {
                return i;
            }
        }
        return -1;
    }

}

