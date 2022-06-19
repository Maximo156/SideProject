using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Wood,
    Stick,
    Rock,
    Axe,
    Sword,
    Hammer,
    IronOre,
    CopperOre,
    GoldOre,
    Emerald,
    Ruby,
    Torch,
    Coal,
    Furnace,
    CopperBar, 
    IronBar,
    GoldBar
}
[Serializable]
public class Item 
{
    public static List<(Item, List<Item>)> Recipies = new List<(Item, List<Item>)>
    {
        {(new Item(ItemType.Wood, 1),  new List<Item>(){new Item(ItemType.Stick, 10) }) },
        {(new Item(ItemType.Torch, 1),  new List<Item>(){new Item(ItemType.Wood, 1), new Item(ItemType.Coal, 1) }) },
        {(new Item(ItemType.Sword, 1),  new List<Item>(){new Item(ItemType.Wood, 1), new Item(ItemType.Stick, 2), new Item(ItemType.Rock, 3) }) },
        {(new Item(ItemType.Axe, 1),  new List<Item>(){new Item(ItemType.Stick, 3), new Item(ItemType.Rock, 3) }) },
        {(new Item(ItemType.Furnace, 1),  new List<Item>(){new Item(ItemType.Rock, 20), new Item(ItemType.Coal, 5) }) },
    };
    public static void SpawnItem(Item item, Vector3 loc)
    {
        GameObject itembox = GameObject.Instantiate( Resources.Load<GameObject>("ItemBox"));
        itembox.GetComponent<Rigidbody>().isKinematic = false;
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
        {ItemType.Torch, 50},
        {ItemType.IronOre, 99 },
        {ItemType.CopperOre, 99 },
        {ItemType.Coal, 99 },
        {ItemType.CopperBar, 99 },
        {ItemType.IronBar, 99 },
        {ItemType.GoldBar, 99 },
        {ItemType.GoldOre, 99 },
        {ItemType.Emerald, 50 },
        {ItemType.Ruby, 50 },
        {ItemType.Furnace, 50 },
    };

    public static Dictionary<ItemType, Sprite> ItemSprites = new Dictionary<ItemType, Sprite>(){
        {ItemType.Wood, Resources.Load<Sprite>("Wood") },
        {ItemType.Stick, Resources.Load<Sprite>("Stick") },
        {ItemType.Rock, Resources.Load<Sprite>("Rock")},
        {ItemType.Axe, Resources.Load<Sprite>("Axe") },
        {ItemType.Sword, Resources.Load<Sprite>("Sword") },
        {ItemType.Hammer, Resources.Load<Sprite>("Hammer") },
        {ItemType.IronOre, Resources.Load<Sprite>("IronOre") },
        {ItemType.CopperOre, Resources.Load<Sprite>("CopperOre") },
        {ItemType.GoldOre, Resources.Load<Sprite>("GoldOre") },
        {ItemType.Emerald, Resources.Load<Sprite>("Emerald") },
        {ItemType.Ruby, Resources.Load<Sprite>("Ruby") },
        {ItemType.Torch, Resources.Load<Sprite>("Torch") },
        {ItemType.Coal, Resources.Load<Sprite>("Coal") },
        {ItemType.Furnace, Resources.Load<Sprite>("Furnace") },
        {ItemType.CopperBar, Resources.Load<Sprite>("CopperBar") },
        {ItemType.IronBar, Resources.Load<Sprite>("IronBar") },
        {ItemType.GoldBar, Resources.Load<Sprite>("GoldBar") },
    };

    public static Dictionary<ItemType, GameObject> PlaceableModels = new Dictionary<ItemType, GameObject>()
    {
        {ItemType.Torch, Resources.Load<GameObject>("Placeables/Torch") },
        {ItemType.Furnace, Resources.Load<GameObject>("Placeables/Furnace") },
    };

    public static Dictionary<ItemType, float> minNormalAngle = new Dictionary<ItemType, float>()
    {
        {ItemType.Torch,  0},
        {ItemType.Furnace, 70 },
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
        if (type == ItemType.Axe || type == ItemType.Sword || type == ItemType.Hammer || type == ItemType.Torch || type == ItemType.Furnace) return true;
        return false;
    }

    public bool Placable()
    {
        return type == ItemType.Torch || type == ItemType.Furnace;
    }

    public Item Dup()
    {
        return new Item(type, count);
    }
}
