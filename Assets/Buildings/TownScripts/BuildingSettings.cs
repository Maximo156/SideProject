using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building Gen")]
public class BuildingSettings : ScriptableObject
{
    public GameObject floor;

    public GameObject Floor { get => floor;}
}
