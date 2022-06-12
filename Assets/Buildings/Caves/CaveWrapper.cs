using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveWrapper : BuildingUnifier
{
    private GameObject reference;

    private Transform parent;
    private Vector3 pos;

    public CaveWrapper(Transform p, Vector3 pos, GameObject r)
    {
        parent = p;
        this.pos = pos;
        reference = r;
        xPos = (int)pos.x;
        yPos = (int)pos.z;
    }
    public override void Build()
    {
        built = true;
        container = Object.Instantiate(reference, pos, Quaternion.identity, parent).transform;
        container.name = "Cave";
    }
    public override bool PlayerInBounds(Vector3 playerPos)
    {
        return true;
    }
}
