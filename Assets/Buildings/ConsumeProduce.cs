using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Inventory))]
public class ConsumeProduce : MonoBehaviour
{
    [SerializeField] Item produce;
    [SerializeField] Item consume;

    [SerializeField] float ratePerSecond;

    float count = 0;
    float lastTime = -1;
    Inventory inv;

    Item toAdd;
    List<Item> toRemove = new List<Item>();

    bool producing = false;

    private void Start()
    {

        toRemove.Add(null);
        //StartCoroutine(Produce());
        inv = gameObject.GetComponent<Inventory>();
    }

    public bool Producing()
    {
        return producing;
    }

    private void FixedUpdate()
    {
        if(lastTime == -1) lastTime = Time.time;
        
        int countConsume = 0;
        foreach (var n in inv.GetItems())
        {
            if (n.type == consume.type) countConsume += n.count;
        }
        countConsume /= consume.count;
        if (countConsume > 0)
        {
            producing = true;
            count += (Time.time - lastTime) * ratePerSecond;
            
            int actConsume = Mathf.Min((int)count, countConsume);

            toAdd = produce.Dup();
            toAdd.count *= actConsume;

            toRemove[0] = consume.Dup();
            toRemove[0].count *= actConsume;

            inv.AddItem(toAdd);
            inv.RemoveItems(toRemove);
            count -= actConsume;
        }
        else
        {
            producing = false;
        }
        lastTime = Time.time;
    }

    /*
    private IEnumerator Produce()
    {
        
        while (true)
        {
            int countConsume = 0;
            foreach (var n in inv.GetItems())
            {
                if (n.type == consume.type) countConsume += n.count;
            }
            countConsume /= consume.count;
            if (countConsume > 0)
            {
                float timeChange = Time.time - lastTime;
                count += timeChange * ratePerSecond;

                int actConsume = Mathf.Min((int)count, countConsume); 

                toAdd = produce.Dup();
                toAdd.count *= actConsume;

                toRemove[0] = consume.Dup();
                toRemove[0].count *= actConsume;

                inv.AddItem(toAdd);
                inv.RemoveItems(toRemove);
                count -= actConsume;
            }
            print(count);
            lastTime = Time.time;
            yield return new WaitForSeconds(1);
        }
    }*/

}
