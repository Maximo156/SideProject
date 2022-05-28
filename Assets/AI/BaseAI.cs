using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI : MonoBehaviour
{
    public float turnSpeed;
    public float moveSpeed;

    public Vector2 targetPos;
    public Transform TargetObject = null;
    public int numChecks = 30;
    
    // Start is called before the first frame update
    private Quaternion _lookRotation;
    private Vector2 _direction;
    private bool obsticle;

    void Start()
    {
        
    }
    private int count;
    // Update is called once per frame
    void Update()
    {
        setHeight();
        if (TargetObject != null)
        {
            targetPos.x = TargetObject.position.x;
            targetPos.y = TargetObject.position.z;
        }
        
        if(count == 0)
            rotate();
        //count = (count + 1) % 10;
        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), targetPos);
        if (distanceToTarget > 0.5)
        {
            move();
        }

        
    }

    float rotate()
    {
        Vector2 transform2d = new Vector2(transform.position.x, transform.position.z);
        //find the vector pointing from our position to the target
        _direction = (targetPos - transform2d).normalized;
        if (!obsticle && GoodForward(new Vector3(_direction.x, 0, _direction.y), 3)) {
            

            //create the rotation we need to be in to look at the target
            Debug.DrawRay(transform.position, new Vector3(_direction.x, 0, _direction.y) * 1000, Color.white);
            _lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.y));
            //rotate us over time according to speed until we are in the required rotation
        }
        else if(!GoodForward(transform.forward, 3))
        {
            obsticle = !GoodForward(new Vector3(_direction.x, 0, _direction.y), Mathf.Infinity);
            Debug.DrawRay(transform.position, new Vector3(_direction.x, 0, _direction.y) * 1000, Color.yellow);
            for(int i = 0; true; i++)
            {
                float angle = (60 / numChecks) * i;
                Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
                if(GoodForward(rotatedVector, 3))
                {
                    _lookRotation = Quaternion.LookRotation(new Vector3(rotatedVector.x, 0, rotatedVector.y));
                    Debug.DrawRay(transform.position, rotatedVector * 1000, Color.yellow);
                    break;
                }

                rotatedVector = Quaternion.AngleAxis(-angle, Vector3.up) * (new Vector3(_direction.x, 0, _direction.y));
                if (GoodForward(rotatedVector, 3))
                {
                    _lookRotation = Quaternion.LookRotation(new Vector3(rotatedVector.x, 0, rotatedVector.y));
                    Debug.DrawRay(transform.position, rotatedVector * 1000, Color.yellow);

                    break;
                }
            }
        }
        else
        {
            obsticle = !GoodForward(new Vector3(_direction.x, 0, _direction.y), Mathf.Infinity);

            Debug.DrawRay(transform.position, new Vector3(_direction.x, 0, _direction.y) * 1000, Color.yellow);

            float angleBetweenTarget = Vector2.Angle(_direction, new Vector2(transform.forward.x, transform.forward.z));
            Vector3 last = transform.forward;
            for (int i = 0; true; i++)
            {
                float angle = (angleBetweenTarget / numChecks) * i;
                Vector3 rotatedVector = Quaternion.AngleAxis(-angle, Vector3.up) * transform.forward;
                Debug.DrawRay(transform.position, rotatedVector * 1000, Color.black);
                if (!GoodForward(rotatedVector, Mathf.Infinity))
                {
                    _lookRotation = Quaternion.LookRotation(last);
                    print("foundGood");
                    break;
                }
                last = rotatedVector;
            }
            
        }

        

        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * turnSpeed);

        return 0;
    }

    void move()
    {
        if(GoodForward( transform.forward, 2, 1))
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
    }

    void setHeight()
    {
        transform.position += new Vector3(0, HeightNoise.getHeight(transform.position)[0] - transform.position.y , 0);
    }

    bool GoodForward(Vector3 dir, float distance, float forwardOff = 0)
    {
        distance = Mathf.Min(distance, (targetPos - new Vector2(transform.position.x, transform.position.z)).magnitude - 1) - forwardOff;
        bool left = !Physics.Raycast(transform.position - transform.right/2.1f + transform.forward * forwardOff, dir, distance);
        Debug.DrawRay(transform.position - transform.right/2.1f, dir * 1000, left ? Color.blue : Color.red);
        bool middle = !Physics.Raycast(transform.position + transform.forward * forwardOff, dir, distance);
        bool right = !Physics.Raycast(transform.position + transform.right/2.1f + transform.forward * forwardOff, dir, distance);
        Debug.DrawRay(transform.position + transform.right/2.1f + transform.forward * forwardOff, dir * 1000, right ? Color.blue : Color.red);
        return left && right && middle;
    }

    void genPath(Vector2 start, float stepLength, Vector2 end)
    {

    }
}
