using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DetailSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Vector3 center;
    [SerializeField] Vector3 size;
    [SerializeField] List<GameObject> possibleChildren = new List<GameObject>();
    [SerializeField] List<bool> canRotates = new List<bool>();
    [SerializeField] int maxChildren = 0;
    [SerializeField] float MinDist;
    float WaitTime = 4;
    Bounds bounds;
    float start;
    void Start()
    {
        float start = Time.time;
        bounds = new Bounds(center, size);
    }

    
    public void Update()
    {
        if (Time.time - start > WaitTime)
        {
            GenChildren();
            Destroy(gameObject.GetComponent<DetailSpawner>());
        }
    }

    public void Call() 
    {
        GenChildren();
        Destroy(gameObject.GetComponent<DetailSpawner>());
    }

    private void GenChildren()
    {

        Random.seed = transform.position.GetHashCode();
        int spawnCount = Mathf.Max(0, Random.Range(0, maxChildren+2) - 1);
        List<Vector3> used = new List<Vector3>();
        for(int i = 0; i< spawnCount; i++)
        {
            int index = Random.Range(0, possibleChildren.Count);
            (GameObject toInstanciate, bool canRotate)= (possibleChildren[index], canRotates[index]);
            Vector3 loc;
            do
            {
                loc = transform.TransformPoint(RandomPointInBounds(bounds));
            } while (used.Where(v => Vector3.Distance(v, loc) < MinDist).ToList().Count > 0);
            GameObject child = Instantiate(toInstanciate, transform.TransformPoint(RandomPointInBounds(bounds)), transform.rotation,transform);
            if (canRotate) child.transform.Rotate(Vector3.up, Random.Range(0, 360), Space.World);
        }
    }

    private static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
