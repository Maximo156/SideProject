using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainOnCollision : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject terrain;
    GameObject terrainAssets;
    GameObject light;
    CaveControl caveControl;
    void Start()
    {
        terrain = GameObject.Find("TerrainGenerator");
        terrainAssets = GameObject.Find("TerrainAssetManager");
        light = GameObject.Find("Sun");
        caveControl = transform.GetComponentInParent<CaveControl>();
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            light.SetActive(true);
            terrain.SetActive(true);
            terrainAssets.SetActive(true);
            caveControl.CaveInactive();
        }
    }
}
