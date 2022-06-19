using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Inventory))]
public class Consume : MonoBehaviour
{
    static Dictionary<ItemType, float> burnTimes = new Dictionary<ItemType, float>()
    {
        {ItemType.Stick, 2},
        {ItemType.Wood, 5},
        {ItemType.Coal, 10},
    };

    Inventory inv;

    bool producing = false;
    float nextTime = 0;

    private void Start()
    {
        inv = gameObject.GetComponent<Inventory>();
    }

    public void FixedUpdate()
    {
        if (Time.time > nextTime)
        {
            foreach (var n in inv.GetItems())
            {
                if (burnTimes.TryGetValue(n.type, out float burnTime))
                {

                    inv.RemoveItem(new Item(n.type, 1));
                    producing = true;
                    nextTime = Time.time + burnTime;
                    return;

                }
            }
            producing = false;
        }
    }

    public bool Producing()
    {
        return producing;
    }

}

