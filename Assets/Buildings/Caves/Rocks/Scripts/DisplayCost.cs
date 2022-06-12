using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplayCost : MonoBehaviour, IPointerEnterHandler
{
    // Start is called before the first frame update
    [SerializeField] private GameObject ItemSlotReference;
    [SerializeField] private UnityEngine.EventSystems.EventSystem eventSystem;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject Container;
    [SerializeField] private GameObject Label;
    private List<(Item, List<Item>)> usableRecepies = new List<(Item, List<Item>)>();
    bool overCost = false;

    private void Start()
    {
        ClearChildren();
    }

    private void Update()
    {

        if (!eventSystem.IsPointerOverGameObject())
        {
            overCost = false;
        }
    }

    private void FixedUpdate()
    {
        Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hitInfo, 2);
        CostInfo c;
        if (hitInfo.transform != null &&  (c = hitInfo.transform.GetComponent<CostInfo>()) != null)
        {
            if (!Container.activeSelf)
            {
                Container.SetActive(true);
                Label.SetActive(true);

                Label.transform.GetChild(0).GetComponent<Text>().text = c.GetName();
                UpdateSlots(c.GetCost());
            }
        }
        else
        {
            Container.SetActive(false);
            Label.SetActive(false);
        }
    }
    public void UpdateSlots(List<Item> items)
    {
        ClearChildren();
        
        foreach (var item in items)
        {
                GameObject c = Instantiate(ItemSlotReference);
                c.transform.SetParent(Container.transform.GetChild(0));
                c = c.transform.GetChild(1).gameObject;
                c.name = "$" + (transform.childCount - 1);
                c.GetComponent<UnityEngine.UI.Image>().sprite = Item.ItemSprites[item.type];
                c.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                c.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = item.count > 1 ? "" + item.count : "";           
        }
    }

    private void ClearChildren()
    {
        for (int i = Container.transform.GetChild(0).childCount - 1; i >= 0; i--)
        {
            Destroy(Container.transform.GetChild(0).GetChild(i).gameObject);
        }
        Container.transform.GetChild(0).DetachChildren();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        overCost = true;
    }
}
