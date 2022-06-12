using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDamage : HealthScript
{
    public int HitsToBreak;
    // Start is called before the first frame update
    private void Start()
    {
        maxHealth = HitsToBreak;
        health = maxHealth;
    }

    public override void Damage(AttackInfo info)
    {
        if (info.type == WeaponType.Hammer)
            health -= 1;

        if (health <= 0)
        {
            Die();
        }
    }

    protected override void ItemDrop(string name = "")
    {
        int index = 0;
        Vector3 pos = transform.position;
        foreach (Item i in dropables)
        {
            if (Random.Range(0f, 1f) < chances[index])
                Item.SpawnItem(i, pos);
            index++;
        }
    }
}
