using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillAnimations : MonoBehaviour
{
    private GameObject particles;
    private GameObject drill;
    private ConsumeProduce consumeProduce;
    // Start is called before the first frame update
    void Start()
    {
        particles = transform.GetChild(0).GetChild(1).gameObject;
        drill = transform.GetChild(0).GetChild(0).gameObject;
        consumeProduce = gameObject.GetComponent<ConsumeProduce>();
    }

    
    Vector3 rot = new Vector3(0, 0, 180);
    // Update is called once per frame
    void FixedUpdate()
    {
        if (consumeProduce.Producing())
        {
            particles.SetActive(true);
            drill.transform.Rotate(rot * Time.deltaTime);
        }
        else particles.SetActive(false);
    }
}
