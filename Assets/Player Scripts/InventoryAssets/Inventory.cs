using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int slotCount = 30;
    [SerializeField] private UI_Inventory invUIScript;
    [SerializeField] private OtherInvenUI OtherInvUIScript;

    private List<Item> itemList;
    void Awake()
    {
        
        itemList = new List<Item>();
        //AddItem( new Item( ItemType.Stick, 1 ) );
        //AddItem(new Item(ItemType.Stick, 3));
        //AddItem(new Item(ItemType.Rock, 3));
        AddItem(new Item(ItemType.IronBar, 99));
        AddItem(new Item(ItemType.CopperBar, 99));
        AddItem(new Item(ItemType.Ruby, 3));
        AddItem(new Item(ItemType.Wood, 99));
        AddItem(new Item(ItemType.Axe, 1));
        //AddItem(new Item(ItemType.Sword, 1));
        AddItem(new Item(ItemType.Hammer, 1));
        AddItem(new Item(ItemType.Torch, 50));
        AddItem(new Item(ItemType.Furnace, 1));
    }

    void FixedUpdate()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void SetUI(OtherInvenUI o)
    {
        OtherInvUIScript = o;
    }

    public void UpdateSlots()
    {
        if(gameObject.name == "Player")
        {
            invUIScript.UpdateSlots(itemList);
        } else if(OtherInvUIScript != null)
        {
            OtherInvUIScript.UpdateSlots(itemList);
        }
    }
    public Item AddItem(Item newItem)
    {
        GatherQuestUpdate update = new GatherQuestUpdate(newItem.Dup());
        newItem = newItem.Dup();
        bool seen = false;
        for(int i = 0; i<itemList.Count; i++)
        {
            if(itemList[i].type == newItem.type)
            {
                int oldCount = itemList[i].count;
                itemList[i].count += Mathf.Min(newItem.count, Item.TypeMaxCount[itemList[i].type] - itemList[i].count);
                newItem.count -= Item.TypeMaxCount[itemList[i].type] - oldCount;
                seen = true;
            } else if (seen && itemList.Count < slotCount)
            {
                itemList.Insert(i, newItem);
                UpdateSlots();
                if (gameObject.name == "Player") QuestManager.PushUpdate(update);
                return null;
            }
            if(newItem.count <= 0)
            {
                UpdateSlots();
                if (gameObject.name == "Player") QuestManager.PushUpdate(update);
                return null;
            }
        }
        if (itemList.Count < slotCount)
        {
            itemList.Add(newItem);
            UpdateSlots();
            if (gameObject.name == "Player") QuestManager.PushUpdate(update);
            return null;
        }

        UpdateSlots();
        Item.SpawnItem(newItem, transform.position + transform.forward / 2);
        update.item.count -= newItem.count;

        if (gameObject.name == "Player") QuestManager.PushUpdate(update);

        return newItem;
    }

    void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (col.gameObject.tag == "Item" && gameObject.name == "Player")
        {
            List<Item> toAdd = col.gameObject.GetComponent<ItemBoxInfo>().GetItem();
            if (toAdd != null)
            {
                foreach (Item i in toAdd)
                {
                    AddItem(i);
                }
                Destroy(col.gameObject);
            }
        }
    }

    public void RemoveItems(List<Item> toRemove)
    {
        foreach(var it in toRemove)
        {
            RemoveItem(it);
        }
    }

    public void RemoveItem(Item toRemove)
    {
        Item item = toRemove.Dup();
        GatherQuestUpdate update = new GatherQuestUpdate(item.Dup());
        update.item.count *= -1;
        QuestManager.PushUpdate(update);

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].type == item.type)
            {
                itemList[i].count -= item.count;
                item.count = 0;
                if (itemList[i].count <= 0)
                {
                    item.count = -itemList[i].count;
                    itemList.RemoveAt(i);
                    i--;
                }
            }
            if (item.count < 1)
            {
                break;
            }
        }
        UpdateSlots();
    }

    public List<Item> GetItems()
    {
        return itemList;
    }

    public void Clear()
    {
        itemList.Clear();
    }
}
