using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceProduce : MonoBehaviour
{
    static Dictionary<ItemType, ItemType> smeltRecepies = new Dictionary<ItemType, ItemType>()
    {
        {ItemType.IronOre, ItemType.IronBar },
        {ItemType.CopperOre, ItemType.CopperBar },
        {ItemType.GoldOre, ItemType.GoldBar },
    };

    [SerializeField] float timePer;
    // Start is called before the first frame update
    Consume consume;
    Inventory inv;
    GameObject particles;
    float nextTime;
    // Start is called before the first frame update
    void Start()
    {
        inv = gameObject.GetComponent<Inventory>();
        consume = gameObject.GetComponent<Consume>();
        particles = transform.GetChild(0).GetChild(0).gameObject;
    }

    public void FixedUpdate()
    {
        if (!consume.Producing()) 
        { 
            nextTime = Time.time + timePer;
            particles.SetActive(false);
        }
        else
        {
            particles.SetActive(true);
        }
        if (Time.time > nextTime)
        {
            foreach (var n in inv.GetItems())
            {
                if (smeltRecepies.TryGetValue(n.type, out ItemType made))
                {
                    inv.RemoveItem(new Item(n.type, 1));
                    inv.AddItem(new Item(made, 1));
                    nextTime = Time.time + timePer;
                    break;
                }
            }
        }
    }
}
