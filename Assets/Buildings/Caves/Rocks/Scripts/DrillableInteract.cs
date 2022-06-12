using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CostInfo))]
public class DrillableInteract : InteractScript
{
    private GameObject player;
    private GameObject drill;
    private CostInfo cost;
    public void Start()
    {
        player = GameObject.Find("Player");
        drill = transform.GetChild(0).gameObject;
        drill.SetActive(false);
        cost = transform.GetComponent<CostInfo>();
    }
    public override bool Interactive()
    {
        Inventory inv = player.GetComponent<Inventory>();
        
        Dictionary<ItemType, int> found = new Dictionary<ItemType, int>();
        foreach (var n in inv.GetItems())
        {
            if (found.ContainsKey(n.type)) found[n.type] += n.count;
            else found[n.type] = n.count;
        }

        foreach (var n in cost.GetCost())
        {
            if (!found.ContainsKey(n.type) || n.count > found[n.type]) return false;
        }
        inv.RemoveItems(cost.GetCost());
        drill.SetActive(true);

        Destroy(transform.GetComponent<DrillableInteract>());
        Destroy(cost);
        return false;
    }

    public override void Respond(Item r)
    {
        throw new System.NotImplementedException();
    }
}
