using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ladderScript1 : MonoBehaviour

{

	public Transform chController;
	int inside = 0;
	public float speedUpDown = 3.2f;
	public PlayerScript FPSInput;
	private bool go = true;

	void Start()
	{
		FPSInput = GetComponent<PlayerScript>();
		inside = 0;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Ladder")
		{
			FPSInput.enabled = false;
			inside++;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.gameObject.tag == "Ladder")
		{
			FPSInput.enabled = true;
			inside -- ;
		}
	}

	void Update()
	{
		if (go)
		{
			if (inside == 0)
			{
				FPSInput.enabled = true;
			}
			else
			{
				FPSInput.enabled = false;
			}

			if (inside != 0 && Input.GetKey("w"))
			{
				transform.position += (Vector3.up * speedUpDown) * Time.deltaTime;
			}

			if (inside != 0 && Input.GetKey("s"))
			{
				transform.position += (Vector3.down * speedUpDown) * Time.deltaTime;
			}

			if (inside != 0 && Input.GetKey("a"))
			{
				transform.position += (Vector3.left * speedUpDown) * Time.deltaTime;
			}

			if (inside != 0 && Input.GetKey("d"))
			{
				transform.position += (Vector3.right * speedUpDown) * Time.deltaTime;
			}
		}
	}

	public void ToggleGo(bool on)
	{
		go = on;
	}

}