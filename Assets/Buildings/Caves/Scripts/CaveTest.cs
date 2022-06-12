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
        gameObject.transform.position = new Vector3(Random.Range(-300, 300), Random.Range(-300, 300), Random.Range(-300, 300));
        gameObject.GetComponent<CaveControl>().CaveActive();
        transform.position = Vector3.zero;
    }

}
