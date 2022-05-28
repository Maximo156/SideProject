using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerQuestUI : MonoBehaviour
{
    [SerializeField] private GameObject QuestSlotReference;
    [SerializeField] private UnityEngine.EventSystems.EventSystem eventSystem;
    private List<Quest> quests = new List<Quest>();

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ClickQuest();
        }
    }
    public void UpdateSlots(List<Quest> quests)
    {
        ClearChildren();
        this.quests = new List<Quest>(quests);

        foreach (var quest in quests)
        {
            GameObject c = Instantiate(QuestSlotReference);
            c.transform.SetParent(transform);
            c = c.transform.GetChild(1).gameObject;
            c.name = "$" + (transform.childCount - 1);
            c.GetComponent<UnityEngine.UI.Text>().text = quest.GetString();
        }

        Resize();
    }


    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        transform.DetachChildren();
    }

    private void Resize()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(190.4509f, Mathf.Max(457.6393f, transform.childCount * (89+15) + 40));
    }

    private void ClickQuest()
    {
        string name;
        if (eventSystem.currentSelectedGameObject != null && (name = eventSystem.currentSelectedGameObject.name)[0] == '$')
        {
            int index = int.Parse(name.Replace("$", " ").Trim());
            if (QuestManager.AddQuest(quests[index]))
            {
                quests.RemoveAt(index);
                UpdateSlots(quests);
            }
        }
    }
}
