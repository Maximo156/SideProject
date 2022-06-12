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
    protected Transform container;
    protected Transform details;

    protected bool built = false;

    protected int[,,] floors;


    public virtual bool isBuilding()
    {
        return false;
    }

    public virtual bool isDungeon()
    {
        return false;
    }

    public virtual float DistFromPlayer(Vector3 playerPos)
    {
        return Vector3.Distance(playerPos, new Vector3(xPos, playerPos.y, yPos));
    }
    public abstract void Build();


    public virtual bool PlayerInBounds(Vector3 playerPos)
    {
        int buffer = 30;
        Rect bounds = new Rect(xPos - buffer, yPos - buffer, 3 * floors.GetLength(1) + 2*buffer, 3 * floors.GetLength(1) + 2*buffer);
        return bounds.Contains(new Vector2(playerPos.x, playerPos.z));
    }
    float combineStart;
    public virtual void SetDetail(bool a)
    {
        if (details != null && built && Time.time - combineStart > 3)
        {
            details.gameObject.SetActive(a);
        }
    }

    protected virtual void Combine()
    {
        container.gameObject.AddComponent<CountDown>();
        combineStart = Time.time;
    }


    public virtual void DestoryGameObject()
    {

        if (built)
        {
            Object.Destroy(container.gameObject);
        }
        built = false;
    }
    public virtual int Forget()
    {

        if (built)
        {
            Object.Destroy(container.gameObject);
        }

        return seed;
    }

    public virtual void SetActive(bool a)
    {
        if (a && !built)
        {
            Build();
        }
        if (built)
        {
            container.gameObject.SetActive(a);
        }
    }
}
