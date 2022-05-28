using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public event System.Action<float, float> OnHealthChange;
    public float maxHealth = 10;
    protected float health   = 10;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public virtual void Damage(AttackInfo info)
    {
        health -= info.baseDamage * info.multiplier;
        if(OnHealthChange != null)
        {
            OnHealthChange(maxHealth, health);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        ItemDrop();
        Destroy(gameObject);
    }

    public virtual bool Dying()
    {
        return health <= 0;
    }

    protected void ItemDrop(string name = "")
    {
        if (name == "")
            name = gameObject.name;
        if(name.Contains("("))
            name = name.Remove(name.IndexOf('('));
        Vector3 pos = transform.position;
        if (name.ToLower().Contains("tree"))
        {
            foreach (Item i in Item.DropTables["Tree"])
            {
                Item.SpawnItem(i, pos);
            }
        }
        else if (name.ToLower().Contains("rock"))
        {

            foreach (Item i in Item.DropTables[name])
            {
                Item.SpawnItem(i, pos);
            }
        }
        else if (name.ToLower().Contains("bush"))
        {
            foreach (Item i in Item.DropTables[name])
            {
                Item.SpawnItem(i, pos);
            }
        }
        else if (name.ToLower().Contains("skeleton"))
        {
            int index = 0;
            foreach (Item i in Item.DropTables["Skeleton"])
            {
                if (Random.Range(0f, 1f) < Item.DropRates["Skeleton"][index])
                    Item.SpawnItem(i, pos);
                index++;
            }
        }
    }
}
