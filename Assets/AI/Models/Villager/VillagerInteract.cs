using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerInteract : InteractScript
{
    // Start is called before the first frame update
    private List<Quest> potentialQuests = new List<Quest>();
    private int questAmount = 10;
    private GameObject InteractiveContainer;
    private float lastQuestUpdate = 0;
    private float secondsBetweenUpdate = 30;
    void Start()
    {
        GetNewQuests();

        InteractiveContainer = GameObject.Find("UI").transform.Find("InteractiveContainer").gameObject;
    }

    private void GetNewQuests()
    {
        potentialQuests.Clear();
        int amount = Random.Range(0, questAmount);
        for (int i = 0; i < amount; i++)
        {
            Quest potQuest = Quest.GetQuest();
            if (!potQuest.Success())
            {
                potentialQuests.Add(potQuest);
            }
            else
                i--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastQuestUpdate > secondsBetweenUpdate)
        {
            lastQuestUpdate = Time.time;
            GetNewQuests();
        }
    }

    public override bool Interactive(Item inHand)
    {
        InteractiveContainer.SetActive(true);
        InteractiveContainer.transform.Find("PotentialQuests").GetChild(0).GetComponent<VillagerQuestUI>().UpdateSlots(potentialQuests);
        return true;
    }

    public override void Respond(Item r)
    {
        throw new System.NotImplementedException();
    }
}
