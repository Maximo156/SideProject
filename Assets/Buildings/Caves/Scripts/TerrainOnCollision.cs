using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainOnCollision : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject terrain;
    GameObject terrainAssets;
    void Start()
    {
        terrain = GameObject.Find("TerrainGenerator");
        terrainAssets = GameObject.Find("TerrainAssetManager");
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            terrain.SetActive(true);
            terrainAssets.SetActive(true);
        }
    }
}
