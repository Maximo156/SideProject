using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Crafting : MonoBehaviour, IPointerEnterHandler
{
    // Start is called before the first frame update
    [SerializeField] private GameObject ItemSlotReference;
    [SerializeField] private UnityEngine.EventSystems.EventSystem eventSystem;
    [SerializeField] private Inventory inv;
    private List<(Item, List<Item>)> usableRecepies = new List<(Item, List<Item>)>();
    bool overCrafting = false;
    
    private void Start()
    {
        ClearChildren();
    }

    private void Update()
    {
        
        if (!eventSystem.IsPointerOverGameObject())
        {
            overCrafting = false;
        }
        if (overCrafting)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                CraftItem();
            }
        }
    }
    public void UpdateSlots(List<Item> items)
    {
        ClearChildren();
        usableRecepies.Clear();
        Dictionary<ItemType, int> found = new Dictionary<ItemType, int>();
        foreach (var n in items)
        {
            if(found.ContainsKey(n.type)) found[n.type] += n.count;
            else found[n.type] = n.count;
        }

        List<(Item, List<Item>)> Recipies = Item.Recipies;

        foreach (var recipe in Recipies)
        {
            if (Craftable(recipe.Item2, found))
            {
                usableRecepies.Add(recipe);
                GameObject c = Instantiate(ItemSlotReference);
                c.transform.SetParent(transform);
                c = c.transform.GetChild(1).gameObject;
                c.name = "$" + (transform.childCount - 1);
                c.GetComponent<UnityEngine.UI.Image>().sprite = Item.ItemSprites[recipe.Item1.type];
                c.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                c.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = recipe.Item1.count > 1 ? "" + recipe.Item1.count : "";

            }
        }
        

        Resize();
    }

    public bool Craftable(List<Item> needed, Dictionary<ItemType, int> found)
    {
        foreach(var n in needed)
        {
            if (!found.ContainsKey(n.type) || n.count > found[n.type]) return false;
        }
        return true;
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        transform.DetachChildren();
    }


    private void Resize()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(190.4509f, Mathf.Max(457.6393f, Mathf.Ceil(transform.childCount / 2f) * 75f + 30));
    }

    private void CraftItem()
    {
        string name;
        if (eventSystem.currentSelectedGameObject != null && (name = eventSystem.currentSelectedGameObject.name)[0] == '$')
        {
            int index = int.Parse(name.Replace("$", " ").Trim());
            if (index >= usableRecepies.Count) return;
            (Item item, List<Item> required) = usableRecepies[index];
            inv.RemoveItems(required);
            inv.AddItem(new Item(item.type, item.count));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        overCrafting = true;
    }
}
