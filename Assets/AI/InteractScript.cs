using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractScript : MonoBehaviour
{
    public abstract void Interactive();

    public abstract void Respond(Item r);
}
