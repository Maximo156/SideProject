using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Inventory))]
public class ChestInteract : InteractScript
{
    [SerializeField] private GameObject ItemSlotReference;
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
        InteractiveContainerUI.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Chest";
        InteractiveContainer.UpdateSlots(inv.GetItems());
        MainInventoryUI.OpenInventory();
    }

}
