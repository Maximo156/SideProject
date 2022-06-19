using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Consume))]
public class GenericProduce : MonoBehaviour
{
    [SerializeField] Item toProduce;
    [SerializeField] float timePer;

    Consume consume;
    Inventory inv;
    float nextTime;
    // Start is called before the first frame update
    void Start()
    {
        inv = gameObject.GetComponent<Inventory>();
        consume = gameObject.GetComponent<Consume>();
    }

    public void FixedUpdate()
    {
        if (!consume.Producing()) nextTime = Time.time + timePer;
        if (Time.time > nextTime)
        {
            inv.AddItem(toProduce);
            nextTime = Time.time + timePer;
        }
    }

}
