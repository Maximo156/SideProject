using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Inventory : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Canvas ParentCanvas;
    [SerializeField] private Canvas invUI;
    [SerializeField] private Canvas headsupUI;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject questPanel;
    [SerializeField] private GameObject InteractivePanel;
    [SerializeField] private GameObject Player;
    private Inventory inv;
    private ladderScript1 ladder;
    private PlayerScript player;
    private AttackScript attack;
    private bool menuOn = false;
    // Start is called before the first frame update
    private List<GameObject> slots = new List<GameObject>();
    void Start()
    {
        for(int i = 0; i< transform.childCount; i++)
        {
            slots.Add(transform.GetChild(i).GetChild(1).gameObject);
            slots[slots.Count - 1].name = "$"+i;
        }

        Transform playerTrans = GameObject.Find("Player").transform;
        ladder = playerTrans.GetComponent<ladderScript1>();
        player = playerTrans.GetComponent<PlayerScript>();
        attack = playerTrans.GetChild(1).GetComponent<AttackScript>();
        ladder.ToggleGo(!menuOn);
        player.ToggleGo(!menuOn);
        attack.ToggleGo(!menuOn);
        try
        {
            invUI.enabled = menuOn;
        }
        catch (UnassignedReferenceException e) { print(e); }
        craftingPanel.SetActive(false);
        questPanel.SetActive(false);
        InteractivePanel.SetActive(false);
        inv = player.GetComponent<Inventory>();
    }

    private void Update()
    {
        if (!menuOn && Input.GetButtonDown("Fire2"))
        {
            Interact();
        }
        if (Input.GetKeyDown("e"))
        {
            if (InteractivePanel.activeSelf)
            {
                InteractivePanel.SetActive(false);
                ladder.ToggleGo(true);
                player.ToggleGo(true);
                attack.ToggleGo(true);
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }

            menuOn = !menuOn;
            
            if (menuOn)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            ladder.ToggleGo(!menuOn);
            player.ToggleGo(!menuOn);
            attack.ToggleGo(!menuOn);

            try
            {
                invUI.enabled = menuOn;
                headsupUI.enabled = !menuOn;
            }
            catch (UnassignedReferenceException e) { }
            if (!menuOn)
            {
                craftingPanel.SetActive(false);
                questPanel.SetActive(false);
            }
        }
        if(menuOn && Input.GetKeyDown(KeyCode.C))
        {
            craftingPanel.SetActive(!craftingPanel.activeSelf);
            if (craftingPanel.activeSelf)
            {
                questPanel.SetActive(false);
            }
        }

        if (menuOn && Input.GetKeyDown(KeyCode.V))
        {
            questPanel.SetActive(!questPanel.activeSelf);
            if (questPanel.activeSelf)
            {
                craftingPanel.SetActive(false);
            }
        }
    }

    public void UpdateSlots(List<Item> items)
    {
        int i = 0;
        for (; i < items.Count && i < slots.Count; i++)
        {
            slots[i].GetComponent<UnityEngine.UI.Image>().sprite = Item.ItemSprites[items[i].type];
            slots[i].GetComponent<UnityEngine.UI.Image>().color = Color.white;
            slots[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = items[i].count > 1 ? "" + items[i].count : "";
        }

        for (; i < slots.Count; i++)
        {
            slots[i].GetComponent<UnityEngine.UI.Image>().color = Color.clear;
            slots[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "";
        }

        craftingPanel.transform.GetChild(0).GetComponentInChildren<Crafting>().UpdateSlots(items);
    }

    public void SetAttack(Item i)
    {
        attack.SetAttack(i);
    }


    public void Interact()
    {
        Physics.Raycast(Player.transform.position, Player.transform.forward * 2, out RaycastHit info);

        if (info.transform != null && info.distance < 2)
        {
            InteractScript interactObject = info.transform.gameObject.GetComponent<InteractScript>();
            if (interactObject != null)
            {
                OtherAI AI = info.transform.gameObject.GetComponent<OtherAI>();
                if(AI != null)
                {
                    AI.interacting = true;
                }
                interactObject.Interactive();
                ladder.ToggleGo(false);
                player.ToggleGo(false);
                attack.ToggleGo(false);
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public bool MenuOn()
    {
        return menuOn;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inv.overSelf = true;
    }
}
