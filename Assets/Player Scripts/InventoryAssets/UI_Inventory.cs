using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Inventory : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Canvas ParentCanvas;
    [SerializeField] private GameObject invUI;
    [SerializeField] private GameObject headsupUI;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject questPanel;
    [SerializeField] private GameObject InteractivePanel;
    [SerializeField] private GameObject OtherInvenPanel;
    [SerializeField] private GameObject Player;
    [SerializeField] private UnityEngine.EventSystems.EventSystem eventSystem;

    Transform playerTrans;
    Transform cameraTrans;
    private bool overSelf = false;
    private Inventory inv;
    private ladderScript1 ladder;
    private PlayerScript player;
    private AttackScript attack;
    private bool menuOn = false;
    private InteractScript currentInteraction = null;
    public static Item inHand;
    // Start is called before the first frame update
    private List<GameObject> slots = new List<GameObject>();
    void Start()
    {
        for(int i = 0; i< transform.childCount; i++)
        {
            slots.Add(transform.GetChild(i).GetChild(1).gameObject);
            slots[slots.Count - 1].name = "$"+i;
        }

        playerTrans = Player.transform;
        cameraTrans = Camera.main.transform;
        ladder = playerTrans.GetComponent<ladderScript1>();
        player = playerTrans.GetComponent<PlayerScript>();
        attack = playerTrans.GetChild(1).GetComponent<AttackScript>();
        ladder.ToggleGo(!menuOn);
        player.ToggleGo(!menuOn);
        attack.ToggleGo(!menuOn);
        try
        {
            invUI.transform.localScale = new Vector3(1, 1, 1) * (menuOn ? 1 : 0);
        }
        catch (UnassignedReferenceException e) { print(e); }
        craftingPanel.SetActive(false);
        questPanel.SetActive(false);
        InteractivePanel.SetActive(false);
        OtherInvenPanel.SetActive(false);
        inv = player.GetComponent<Inventory>();
    }

    private void Update()
    {
        if (!eventSystem.IsPointerOverGameObject())
        {
            overSelf = false;
        }
        if (!menuOn && Input.GetButtonDown("Fire2"))
        {
            Interact();
        }
        if (Input.GetKeyDown("e"))
        {
            if(currentInteraction != null) currentInteraction.Close();
            OpenInventory(null);
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


        if (overSelf)
        {
            if (menuOn && ((Input.GetButtonDown("Fire1") && Input.GetKey(KeyCode.Q)) || (Input.GetButton("Fire1") && Input.GetKeyDown(KeyCode.Q))))
            {
                DropItem();
            }
            else if (menuOn && Input.GetButtonDown("Fire1"))
            {
                Equip();
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
        Physics.Raycast(cameraTrans.position, cameraTrans.forward, out RaycastHit info, 2, Physics.AllLayers);
        if (info.transform != null)
        {
            InteractScript interactObject = info.transform.gameObject.GetComponent<InteractScript>();
            if (interactObject != null)
            {
                OtherAI AI = info.transform.gameObject.GetComponent<OtherAI>();
                if(AI != null)
                {
                    AI.interacting = true;
                }
                if (interactObject.Interactive(inHand))
                {
                    ladder.ToggleGo(false);
                    player.ToggleGo(false);
                    attack.ToggleGo(false);
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            else if(info.transform.gameObject.GetComponent<MeshCollider>() != null)
            {
                if (inHand != null && inHand.Placable())
                {
                    Vector3 dirOld = info.normal;
                    Vector3 dir = new Vector3(dirOld.x, dirOld.y, dirOld.z);
                    dir.y = 0;
                    dir = Vector3.Normalize(dir);

                    if (Vector3.Angle(dir, dirOld) > Item.minNormalAngle[inHand.type])
                    {
                        GameObject t = Instantiate(Item.PlaceableModels[inHand.type], info.point, Quaternion.identity, info.transform);
                        t.name = Item.PlaceableModels[inHand.type].name;
                        t.transform.forward = dir;
                        if ((inHand.count -= 1) < 1)
                        {
                            inHand = null;
                        }
                        SetAttack(inHand);
                    }
                }
            }
        }
    }

    public bool MenuOn()
    {
        return menuOn;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        overSelf = true;
    }


    public void DropItem()
    {
        string name;
        if (eventSystem.currentSelectedGameObject != null && (name = eventSystem.currentSelectedGameObject.name)[0] == '$')
        {
            int index = int.Parse(name.Replace("$", " ").Trim());
            List<Item> itemList = inv.GetItems();
            if (index >= itemList.Count) return;
            Item.SpawnItem(itemList[index], playerTrans.position + playerTrans.forward / 2);
            GatherQuestUpdate update = new GatherQuestUpdate(itemList[index]);
            update.item.count *= -1;
            QuestManager.PushUpdate(update);

            itemList.RemoveAt(index);
            UpdateSlots(itemList);
        }
    }

    public void Equip()
    {
        string name;
        if (eventSystem.currentSelectedGameObject != null && (name = eventSystem.currentSelectedGameObject.name)[0] == '$')
        {
            List<Item> itemList = inv.GetItems();
            int index = int.Parse(name.Replace("$", " ").Trim());
            if (index >= itemList.Count)
            {
                if (inHand != null) itemList.Add(inHand);
                inHand = null;
                SetAttack(inHand);
                UpdateSlots(itemList);
            }
            else if (currentInteraction == null)
            {
                
                if(inHand != null) itemList.Add(inHand);
                if (itemList[index].Holdable())
                {
                    inHand = itemList[index];
                    itemList.RemoveAt(index);
                }
                else
                {
                    inHand = null;
                }
                SetAttack(inHand);
                UpdateSlots(itemList);
            }
            else
            {
                currentInteraction.Respond(itemList[index]);
                itemList.RemoveAt(index);
                UpdateSlots(itemList);
            }
        }
    }

    public Inventory OpenInventory(InteractScript interaction)
    {
        currentInteraction = interaction;

        UpdateSlots(inv.GetItems());
        if (InteractivePanel.activeSelf)
        {
            InteractivePanel.SetActive(false);
            ladder.ToggleGo(true);
            player.ToggleGo(true);
            attack.ToggleGo(true);
            Cursor.lockState = CursorLockMode.Locked;
            return inv;
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
            invUI.transform.localScale = new Vector3(1, 1, 1) * (menuOn ? 1 : 0);
            headsupUI.SetActive(!menuOn);
        }
        catch (UnassignedReferenceException e) { }
        if (!menuOn)
        {
            craftingPanel.SetActive(false);
            questPanel.SetActive(false);
            OtherInvenPanel.SetActive(false);
        }
        return inv;
    }
}
