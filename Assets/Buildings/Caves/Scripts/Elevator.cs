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

    public float caveLight;
    public float ambientLight;

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

    public IEnumerator ChangeColor(bool dir)
    {
        float curLight = dir ? caveLight : ambientLight;
        while (curLight <= ambientLight && curLight >= caveLight)
        {
            curLight += (dir ? 1 : -1 )*(ambientLight - caveLight) / (start - end) * speed * Time.deltaTime;
            RenderSettings.ambientLight = new Color(curLight/256, curLight/256, curLight/256);
            yield return null;
        }

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
            StartCoroutine(ChangeColor(Mathf.Abs(start - pos) > Mathf.Abs(end - pos)));
        }
    }

}
