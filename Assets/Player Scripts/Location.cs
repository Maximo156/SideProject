using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Location : MonoBehaviour
{
    // Start is called before the first frame update
    Transform player;
    Text t;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        t = GetComponent<Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        t.text = "Location:\n" + (int)player.position.x + "\n" + (int)player.position.z;
    }
}
