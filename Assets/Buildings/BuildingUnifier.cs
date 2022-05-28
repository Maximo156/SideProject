using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingUnifier
{
    protected int xPos = 0;
    protected int yPos = 0;
    protected int seed;

    protected BuildingManager manager;

    protected Transform bldg;

    protected bool built = false;


    public virtual bool isBuilding()
    {
        return false;
    }

    public virtual bool isDungeon()
    {
        return false;
    }

    public abstract float DistFromPlayer(Vector3 pos);

    public abstract bool SetActive(bool a);

    public abstract void DestoryGameObject();

    public abstract int Forget();

    public abstract void Build();
}
