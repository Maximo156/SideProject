using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject terrain;
    GameObject terrainAssets;
    GameObject player;
    GameObject light;
    CaveControl caveControl;
    float start;
    float end;
    bool moving = false;
    [SerializeField] float speed = 0.2f;
    [SerializeField] float PlayerOffset = 1.2f;

    void Start()
    {
        start = transform.position.y;
        end = start - 19.6f;
        terrain = GameObject.Find("TerrainGenerator");
        terrainAssets = GameObject.Find("TerrainAssetManager");
        player = GameObject.Find("Player");
        light = GameObject.Find("Sun");
        caveControl = transform.GetComponentInParent<CaveControl>();
    }

    //true = up
    //false = down
    public IEnumerator Move(bool dir)
    {
        while(transform.position.y <= start && transform.position.y >= end)
        {
            transform.position = transform.position + new Vector3(0, (dir ? speed : -speed), 0) * Time.deltaTime;

            float px = player.transform.position.x;
            float py = player.transform.position.z;
            float x = transform.position.x;
            float y = transform.position.z;

            if (px < x && px > x - 3 && py < y && py > y - 3) player.transform.position = new Vector3(px, transform.position.y + PlayerOffset,py);
            yield return null;
        }
        transform.position =  new Vector3(transform.position.x, (dir ? start : end), transform.position.z);
        moving = false;
        yield break;
    }


    void OnTriggerEnter(Collider col)
    {
        if (!moving && col.gameObject.tag == "Player")
        {
            moving = true;
            light.SetActive(false);
            terrain.SetActive(false);
            terrainAssets.SetActive(false);
            caveControl.CaveActive();
            float pos = transform.position.y;
            StartCoroutine(Move(Mathf.Abs(start - pos) > Mathf.Abs(end - pos)));
        }
    }

}
