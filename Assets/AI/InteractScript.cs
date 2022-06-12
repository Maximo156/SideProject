using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractScript : MonoBehaviour
{
    public abstract bool Interactive();

    public abstract void Respond(Item r);

    public virtual void Close()
    {

    }
}
