using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostInfo : MonoBehaviour
{
    [SerializeField] List<Item> Cost;
    [SerializeField] string name;

    public List<Item> GetCost()
    {
        return Cost;
    }

    public string GetName()
    {
        return name;
    }
}
