using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyIKFootLocator : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform body = default;
    public Transform hip;
    public int stepAngle;
    [SerializeField] IKFootLocation otherFoot = default;
    [SerializeField] float speed = 1;
    [SerializeField] float stepDistance = 4;
    [SerializeField] float stepLength = 4;
    [SerializeField] float stepHeight = 1;
    [SerializeField] Vector3 footOffset = default;

    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    private void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1f;
    }

    // Update is called once per frame

    void Update()
    {
        float scale = transform.root.localScale.x;
        transform.position = currentPosition;
        transform.up = currentNormal;


        if (Vector3.Angle(newPosition, Vector3.down) > -stepAngle && lerp > 1)
        {
            Ray ray = new Ray(hip.position, Quaternion.AngleAxis(stepAngle, hip.right) * Vector3.down);

            Debug.DrawRay(ray.origin, ray.direction*10);

            Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value);
            lerp = 0;
            newPosition = info.point;
            newNormal = info.normal;
            newPosition.y = HeightNoise.getHeight(newPosition)[0];
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(newPosition, 0.5f);
       // Gizmos.DrawSphere(currentPosition, 0.5f);
    }



    public bool IsMoving()
    {
        return lerp < 1;
    }

    public Vector3 CurPos()
    {
        return currentPosition;
    }
}
