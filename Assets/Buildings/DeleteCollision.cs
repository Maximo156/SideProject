using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteCollision : MonoBehaviour
{
	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Ladder")
		{
			Destroy(transform.gameObject);
		}
	}

	int counter = 0;
	public void FixedUpdate()
    {
		if(counter < 60)
        {
			counter++;
        }
        else
        {
			Destroy(gameObject.GetComponent<Rigidbody>());
			Destroy(gameObject.GetComponent<DeleteCollision>());
		}
    }
}
