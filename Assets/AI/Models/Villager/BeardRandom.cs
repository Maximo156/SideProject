using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeardRandom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i<transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        int random = Random.Range(0, transform.childCount + 1);
        if(random != transform.childCount)
        {
            transform.GetChild(random).gameObject.SetActive(true);
        }
    }

}
