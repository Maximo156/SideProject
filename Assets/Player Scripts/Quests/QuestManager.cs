using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] static QuestUI questUI;
    [SerializeField] static Inventory inv;
    private static List<Quest> activeQuests = new List<Quest>();
    public static int questLimit = 10;

    // Start is called before the first frame update
    void Start()
    {
        questUI = GameObject.Find("QuestContainer").transform.GetChild(0).GetChild(0).GetComponent<QuestUI>();
        inv = GameObject.Find("Player").GetComponent<Inventory>();

        activeQuests.Add(new HuntQuest("skeleton", 1));
        questUI.UpdateSlots(activeQuests);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PushUpdate(QuestUpdate update)
    {
        foreach (var quest in activeQuests)
        {
            
            quest.Update(update);
        }
        questUI.UpdateSlots(activeQuests);
    }

    public static bool AddQuest(Quest q)
    {
        if (activeQuests.Count < questLimit)
        {

            if(q.GetQuestType() == QuestType.Gather)
            {
                foreach (Item i in inv.GetItems())
                {
                    GatherQuestUpdate update = new GatherQuestUpdate(i.Dup());
                    q.Update(update);
                }
            }

            q.Start();
            activeQuests.Add(q);
            questUI.UpdateSlots(activeQuests);
            return true;
        }
        else return false;
    }

    public static void Complete(int i)
    {
        List<Item> toRemove = activeQuests[i].Complete(out Item reward);
        if (reward != null)
        {
            if (toRemove != null)
            {
                inv.RemoveItems(toRemove);
            }
            inv.AddItem(reward);
            activeQuests.RemoveAt(i);
            questUI.UpdateSlots(activeQuests);
        }
    }


}
