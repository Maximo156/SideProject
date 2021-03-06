using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponType
{
    Sword,
    Axe,
    Hammer,
    Torch,
    Fist
}
public class AttackInfo
{
    public float baseDamage;
    public WeaponType type;
    public float multiplier;
    public float range;
}

public class AttackScript : MonoBehaviour
{
    [SerializeField] EquipmentHandler equipment;
    [SerializeField] Image ItemSlot;
    [SerializeField] Text ItemCount;
    [SerializeField] Sprite empty;
    private bool go = true;
    private static AttackInfo fist = new AttackInfo { baseDamage = 2, type = WeaponType.Fist, multiplier = 1, range = 1.25f };
    private static AttackInfo torch = new AttackInfo { baseDamage = 2, type = WeaponType.Torch, multiplier = 1, range = 1.25f };
    private static AttackInfo sword = new AttackInfo { baseDamage = 4, type = WeaponType.Sword, multiplier = 1, range = 2f };
    private static AttackInfo axe = new AttackInfo { baseDamage = 3, type = WeaponType.Axe, multiplier = 1, range = 1.75f};
    private static AttackInfo hammer = new AttackInfo { baseDamage = 3, type = WeaponType.Hammer, multiplier = 1, range = 1.75f};

    private AttackInfo currentWeapon = fist;
    private Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        ItemSlot.sprite = empty;
        ItemCount.text = "";
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (go)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
            }
        }
    }

    public void Attack()
    {
        Debug.DrawRay(cam.position, cam.forward*currentWeapon.range);
        Physics.Raycast(cam.position, cam.forward, out RaycastHit info, currentWeapon.range, Physics.AllLayers);

        if (info.transform != null && info.distance < currentWeapon.range)
        {
            HealthScript hitObject = info.transform.gameObject.GetComponent<HealthScript>();
            if (hitObject != null)
            {
                hitObject.Damage(currentWeapon);
                if (hitObject.Dying())
                {
                    QuestManager.PushUpdate(new HuntQuestUpdate(hitObject.tag));
                }
            }


            CaveMine minable = info.transform.gameObject.GetComponent<CaveMine>();
            if (minable != null)
            {
                minable.Mine(info.point);
            }
        }
    
    }

    public void ToggleGo(bool on)
    {
        go = on;
    }

    public void SetAttack(Item i)
    {
        if(i == null || !i.Holdable())
        {
            ItemSlot.sprite = empty;
            ItemCount.text = "";
            currentWeapon = fist;
        }
        else
        {
            ItemCount.text = i.count < 2 ? "" : i.count + "";
            ItemSlot.sprite = Item.ItemSprites[i.type];
        }

        if (i != null)
        {
            if (i.type == ItemType.Sword)
            {
                currentWeapon = sword;
            }
            else if (i.type == ItemType.Axe)
            {
                currentWeapon = axe;
            }
            else if (i.type == ItemType.Hammer)
            {
                currentWeapon = hammer;
            }
            else if (i.type == ItemType.Torch)
            {
                currentWeapon = torch;
            }
        }
        
        equipment.ChangeHand(currentWeapon);
    }

    

}
