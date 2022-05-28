using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public float minVelocity = 0.001f;
    public float velocityMultiplyer = 1f;
    Animator animator;
    Vector3 prevPos;
    bool walking = false;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = Vector3.Distance(transform.position, prevPos);
        if(dist > minVelocity)
        {
            walking = true;
        }
        else
        {
            walking = false;
        }

        prevPos = transform.position;
        animator.SetBool("Walking", walking);
        animator.SetFloat("AnimSpeed", velocityMultiplyer*dist);
    }
}
