using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveMine : MonoBehaviour
{
    CaveControl controler;
    private void Start()
    {
        controler = transform.parent.gameObject.GetComponent<CaveControl>();
    }
    public void Mine(Vector3 pos)
    {
        controler.Mine(Vector3Int.FloorToInt(pos - transform.position));
    }
}
