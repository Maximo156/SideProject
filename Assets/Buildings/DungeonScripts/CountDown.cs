using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour
{

    int counter = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        if(counter < 60)
        {
            counter++;
            return;
        }
        gameObject.AddComponent<MeshCombiner>();
        Destroy(gameObject.GetComponent<CountDown>());
    }
}
