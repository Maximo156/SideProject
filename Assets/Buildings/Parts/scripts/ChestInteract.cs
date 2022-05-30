using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Inventory))]
public class ChestInteract : InteractScript
{
    private Inventory inv;
    private GameObject InteractiveContainerUI;
    private OtherInvenUI InteractiveContainer;
    private UI_Inventory MainInventoryUI;
    void Start()
    {
        InteractiveContainerUI = GameObject.Find("UI").transform.Find("OtherInven").gameObject;
        InteractiveContainer = InteractiveContainerUI.transform.GetChild(0).GetChild(0).GetComponent<OtherInvenUI>();
        MainInventoryUI = GameObject.Find("InventoryContainer").GetComponent<UI_Inventory>();
        inv = gameObject.GetComponent<Inventory>();
    }
    public override void Interactive()
    {
        InteractiveContainerUI.SetActive(true);
        InteractiveContainerUI.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = gameObject.name;
        InteractiveContainer.UpdateSlots(inv.GetItems());

        Inventory playerInv = MainInventoryUI.OpenInventory(this);

        InteractiveContainer.SetReferences(inv, playerInv);
    }

    public override void Respond(Item response)
    {
        inv.AddItem(response);
        InteractiveContainer.UpdateSlots(inv.GetItems());
    }

}
