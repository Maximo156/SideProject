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
        gameObject.GetComponent<CaveControl>().CaveActive();
        //gameObject.GetComponent<CaveControl>().test();
    }

}
