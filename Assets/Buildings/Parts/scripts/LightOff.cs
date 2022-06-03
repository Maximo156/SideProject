using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOff : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float minDist = 30;
    Transform player;
    GameObject child;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        child = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(player.position, child.transform.position) < minDist) child.SetActive(true);
        else child.SetActive(false);
    }
}
