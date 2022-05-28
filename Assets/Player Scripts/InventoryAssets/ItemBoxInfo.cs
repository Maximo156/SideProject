using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxInfo : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Item> items = new List<Item>();
    private float time;
    private Rigidbody rigid = null;
    public void SetItem(Item i)
    {
        items.Add(new Item(i.type, i.count));
        time = Time.time;
        rigid = gameObject.GetComponent<Rigidbody>();
        rigid.isKinematic = false;
    }

    private int counter = 0;
    private void FixedUpdate()
    {
        if(rigid != null)
        {
            if (counter < 1) counter++;
            else
            {
                rigid.isKinematic = true;
                rigid = null;
            }
        }
    }

    public List<Item> GetItem()
    {
        if (Time.time - time > 3){
            return items;
        }
        return null;
    }

    public List<Item> Items()
    {
        return items;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Item")
        {
            if(col.gameObject.GetHashCode() < gameObject.GetHashCode())
            {
                List<Item> toAdd = col.gameObject.GetComponent<ItemBoxInfo>().Items();
                items.AddRange(toAdd);
                Destroy(col.gameObject);
            }
        }
    }
}
