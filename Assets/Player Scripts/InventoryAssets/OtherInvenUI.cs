using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OtherInvenUI : MonoBehaviour, IPointerEnterHandler
{
    // Start is called before the first frame update
    [SerializeField] private GameObject ItemSlotReference;
    [SerializeField] private UnityEngine.EventSystems.EventSystem eventSystem;
    bool overOtherInven = false;
    private Inventory curInv;
    private Inventory playerInv;

    private void Start()
    {
        ClearChildren();
    }

    public void SetReferences(Inventory cur, Inventory player)
    {
        curInv = cur;
        playerInv = player;
    }

    private void Update()
    {

        if (!eventSystem.IsPointerOverGameObject())
        {
            overOtherInven = false;
        }
        if (overOtherInven)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Click();
            }
        }
    }
    public void UpdateSlots(List<Item> items)
    {
        ClearChildren();


        foreach (var item in items)
        {
                GameObject c = Instantiate(ItemSlotReference);
                c.transform.SetParent(transform);
                c = c.transform.GetChild(1).gameObject;
                c.name = "$" + (transform.childCount - 1);
                c.GetComponent<UnityEngine.UI.Image>().sprite = Item.ItemSprites[item.type];
                c.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                c.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = item.count > 1 ? "" + item.count : "";
        }


        Resize();
    }


    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        transform.DetachChildren();
    }

    public void Click()
    {
        string name;
        if (eventSystem.currentSelectedGameObject != null && (name = eventSystem.currentSelectedGameObject.name)[0] == '$')
        {
            List<Item> itemList = curInv.GetItems();
            int index = int.Parse(name.Replace("$", " ").Trim());
            if (index >= itemList.Count) return;
            
            playerInv.AddItem(itemList[index]);
            itemList.RemoveAt(index);
            UpdateSlots(itemList);
        }
    }


    private void Resize()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(190.4509f, Mathf.Max(457.6393f, Mathf.Ceil(transform.childCount / 2f) * 75f + 30));
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        overOtherInven = true;
    }
}
