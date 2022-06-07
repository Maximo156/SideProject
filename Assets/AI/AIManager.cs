using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float turnOffDistance = 100;
    [SerializeField] float despawnDistance = 4000;
    int count = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        count++;
        if (count > 60)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                float dist = Vector3.Distance(player.position, child.position);
                if (dist < turnOffDistance) child.gameObject.SetActive(true);
                else if (dist < despawnDistance) child.gameObject.SetActive(false);
                else Destroy(child.gameObject);
            }
            count = 0;
        }
    }
}
