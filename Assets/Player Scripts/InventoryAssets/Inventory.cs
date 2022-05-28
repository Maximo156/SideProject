using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int slotCount = 30;
    [SerializeField] private UI_Inventory invUIScript;
    [SerializeField] private UnityEngine.EventSystems.EventSystem eventSystem;
    
    private bool menuOn = false;
    public bool overSelf = false;
    private List<Item> itemList;
    void Start()
    {
        
        itemList = new List<Item>();
        //AddItem( new Item( ItemType.Stick, 1 ) );
        //AddItem(new Item(ItemType.Stick, 3));
        //AddItem(new Item(ItemType.Rock, 3));
        AddItem(new Item(ItemType.Stick, 99));
        AddItem(new Item(ItemType.Wood, 99));
        AddItem(new Item(ItemType.Axe, 1));
        //AddItem(new Item(ItemType.Sword, 1));
        AddItem(new Item(ItemType.Hammer, 1));

    }

    void FixedUpdate()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //print(overSelf);
        if (!eventSystem.IsPointerOverGameObject())
        {
            overSelf = false;
        }
        menuOn = invUIScript.MenuOn();

        if (overSelf)
        {
            if (menuOn && Input.GetButtonDown("Fire1") && Input.GetKey(KeyCode.Q))
            {
                DropItem();
            }
            else if (menuOn && Input.GetButton("Fire1") && Input.GetKeyDown(KeyCode.Q))
            {
                DropItem();
            }
            else if (menuOn && Input.GetButtonDown("Fire1"))
            {
                Equip();
            }
        }
    }

    public Item AddItem(Item newItem)
    {
        GatherQuestUpdate update = new GatherQuestUpdate(newItem.Dup());
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
                invUIScript.UpdateSlots(itemList);
                QuestManager.PushUpdate(update);
                return null;
            }
            if(newItem.count <= 0)
            {
                invUIScript.UpdateSlots(itemList);
                QuestManager.PushUpdate(update);
                return null;
            }
        }
        if (itemList.Count < slotCount)
        {
            itemList.Add(newItem);
            invUIScript.UpdateSlots(itemList);
            QuestManager.PushUpdate(update);
            return null;
        }

        invUIScript.UpdateSlots(itemList);
        Item.SpawnItem(newItem, transform.position + transform.forward / 2);
        update.item.count -= newItem.count;

        QuestManager.PushUpdate(update);

        return newItem;
    }

    private void DropItem()
    {
        string name;
        if (eventSystem.currentSelectedGameObject != null && (name = eventSystem.currentSelectedGameObject.name)[0] == '$')
        {
            int index = int.Parse(name.Replace("$", " ").Trim());
            if (index >= itemList.Count) return;
            Item.SpawnItem(itemList[index], transform.position + transform.forward/2);
            GatherQuestUpdate update = new GatherQuestUpdate(itemList[index]);
            update.item.count *= -1;
            QuestManager.PushUpdate(update);

            itemList.RemoveAt(index);
            invUIScript.UpdateSlots(itemList);
        }
    }

    private void Equip()
    {
        string name;
        if (eventSystem.currentSelectedGameObject != null && (name = eventSystem.currentSelectedGameObject.name)[0] == '$')
        {
            int index = int.Parse(name.Replace("$", " ").Trim());
            if (index >= itemList.Count) return;
            invUIScript.SetAttack(itemList[index]);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (col.gameObject.tag == "Item")
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
            Item item = new Item(it.type, it.count);
            GatherQuestUpdate update = new GatherQuestUpdate(item.Dup());
            update.item.count *= -1;
            QuestManager.PushUpdate(update);

            for (int i = 0; i< itemList.Count; i++)
            {
                if(itemList[i].type == item.type)
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
                if(item.count < 1)
                {
                    break;
                }
            }
        }
    }

    public List<Item> GetItems()
    {
        return itemList;
    }
}
