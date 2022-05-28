using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClothesRandomizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SkinnedMeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

}
