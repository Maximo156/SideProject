using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    int count = 0;
    void FixedUpdate()
    {
        count++;
        if(count == 60)
        {
            transform.GetChild(0).gameObject.AddComponent<MeshCombiner>();
            //Destroy(gameObject.GetComponent<CountDown>());
        } 
        else if(count == 6*60)
        {
            transform.GetChild(1).gameObject.AddComponent<MeshCombiner>();
            Destroy(gameObject.GetComponent<CountDown>());
        }
    }
}
