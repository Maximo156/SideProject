using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Wood,
    Stick,
    Rock,
    Axe,
    Sword,
    Hammer
}

public class Item 
{
    public static List<(Item, List<Item>)> Recipies = new List<(Item, List<Item>)>
    {
        {(new Item(ItemType.Wood, 1),  new List<Item>(){new Item(ItemType.Stick, 10) }) },
        {(new Item(ItemType.Sword, 1),  new List<Item>(){new Item(ItemType.Wood, 1), new Item(ItemType.Stick, 2), new Item(ItemType.Rock, 3) }) },
        {(new Item(ItemType.Axe, 1),  new List<Item>(){new Item(ItemType.Stick, 3), new Item(ItemType.Rock, 3) }) },
    };
    public static void SpawnItem(Item item, Vector3 loc)
    {
        loc.y = HeightNoise.getHeight(loc)[0] + 0.1f;
        GameObject itembox = GameObject.Instantiate( Resources.Load<GameObject>("ItemBox"));
        itembox.transform.position = loc;
        itembox.GetComponent<ItemBoxInfo>().SetItem(item);
    }
    public static Dictionary<ItemType, int> TypeMaxCount = new Dictionary<ItemType, int>(){
        {ItemType.Wood, 99 },
        {ItemType.Stick, 99 },
        {ItemType.Rock, 99 },
        {ItemType.Axe, 1 },
        {ItemType.Sword, 1 },
        {ItemType.Hammer, 1 },
    };

    public static Dictionary<ItemType, Sprite> ItemSprites = new Dictionary<ItemType, Sprite>(){
        {ItemType.Wood, Resources.Load<Sprite>("Wood") },
        {ItemType.Stick, Resources.Load<Sprite>("Stick") },
        {ItemType.Rock, Resources.Load<Sprite>("Rock")},
        {ItemType.Axe, Resources.Load<Sprite>("Axe") },
        {ItemType.Sword, Resources.Load<Sprite>("Sword") },
        {ItemType.Hammer, Resources.Load<Sprite>("Hammer") },
    };

    public static Dictionary<string, List<Item>> DropTables = new Dictionary<string, List<Item>>()
    {
        {"Rock_1",  new List<Item>(){new Item(ItemType.Rock, 2) }   },
        {"Rock_2",  new List<Item>(){new Item(ItemType.Rock, 4) }   },
        {"Rock_3",  new List<Item>(){new Item(ItemType.Rock, 4) }   },
        {"Tree",  new List<Item>(){new Item(ItemType.Wood, 3), new Item(ItemType.Stick, 10) }   },
        {"Bush",  new List<Item>(){new Item(ItemType.Stick, 10) }   },
        {"Skeleton",  new List<Item>(){new Item(ItemType.Sword, 1) }   },
    };

    public static Dictionary<string, List<float>> DropRates = new Dictionary<string, List<float>>()
    {
        {"Skeleton",  new List<float>(){0.5f }   },
    };

    public ItemType type;
    public int count;

    public Item( ItemType type, int count)
    {
        this.type = type;
        this.count = count; 
    }

    public bool Holdable()
    {
        if (type == ItemType.Axe || type == ItemType.Sword || type == ItemType.Hammer) return true;
        return false;
    }

    public Item Dup()
    {
        return new Item(type, count);
    }
}
