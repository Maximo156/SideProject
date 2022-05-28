using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    public GameObject uiPrefab;
    public Transform target;

    Transform ui;
    Image healthSlider;
    Transform cam;
    int activeTime = 5;
    float lastTimeActive = 0;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        ui = Instantiate(uiPrefab, GameObject.Find("WorldCanvas").transform).transform;
        healthSlider = ui.GetChild(0).GetComponent<Image>();
        ui.gameObject.SetActive(false);
        GetComponent<HealthScript>().OnHealthChange += OnHealthChange;
     }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Time.time - lastTimeActive > activeTime)
        {
            ui.gameObject.SetActive(false);
        }

        ui.position = target.position;
        ui.forward = -cam.forward;
    }

    void OnHealthChange(float max, float health)
    {
        ui.gameObject.SetActive(true);
        lastTimeActive = Time.time;
        healthSlider.fillAmount = health / max;
        if (health / max <= 0) Destroy(ui.gameObject);
    }
}
