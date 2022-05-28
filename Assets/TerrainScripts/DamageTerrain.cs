using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTerrain : HealthScript
{
    private void Start()
    {
        base.maxHealth = 1;
        health = maxHealth;
    }
    
    // Start is called before the first frame update
    public override void Die()
    {
        
        if(transform.parent.name == "TerrainAssetManager")
        {
            transform.parent.GetComponent<TerrainAssetManager>().BreakObject(transform.position, 10);
            ItemDrop(transform.parent.GetComponent<TerrainAssetManager>().GetName(gameObject.name));
            health = maxHealth;
        }
        else
        {
            base.Die();
        }
    }

    public override void Damage(AttackInfo info)
    {
        string name = gameObject.name.ToLower();
        if (transform.parent.name == "TerrainAssetManager")
        {
            name = transform.parent.GetComponent<TerrainAssetManager>().GetName(name).ToLower();
        }
        if (name.Contains("rock"))
        {
            if (info.type == WeaponType.Hammer)
                health -= info.baseDamage * info.multiplier;
        }
        else if (name.Contains("tree"))
        {
            if (info.type == WeaponType.Axe)
                health -= info.baseDamage * info.multiplier;
        }
        else
        {
            health -= info.baseDamage * info.multiplier;
        }

        if (health <= 0)
        {
            Die();
        }
    }

}
