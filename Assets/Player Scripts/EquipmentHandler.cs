using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentHandler : MonoBehaviour
{
    GameObject sword;
    GameObject axe;
    GameObject hammer;
    GameObject Torch;
    // Start is called before the first frame update
    void Start()
    {
        sword = transform.Find("Sword").gameObject;
        axe = transform.Find("Axe").gameObject;
        hammer = transform.Find("Hammer").gameObject;
        Torch = transform.Find("Torch").gameObject;
        ClearChildren();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeHand(AttackInfo info)
    {
        ClearChildren();
        if(info.type == WeaponType.Sword)
        {
            sword.SetActive(true);
        } else if(info.type == WeaponType.Axe)
        {
            axe.SetActive(true);
        } else if(info.type == WeaponType.Hammer)
        {
            hammer.SetActive(true);
        }
        else if (info.type == WeaponType.Torch)
        {
            Torch.SetActive(true);
        }
    }

    private void ClearChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
