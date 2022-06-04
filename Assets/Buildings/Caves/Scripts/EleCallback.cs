using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EleCallback : MonoBehaviour
{
    Elevator e;
    private void Start()
    {
        e = transform.GetChild(0).GetComponent<Elevator>();
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            StartCoroutine(e.Move(false));
        }
    }
}
