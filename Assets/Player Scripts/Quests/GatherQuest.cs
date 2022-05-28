using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherQuestUpdate : QuestUpdate
{
    public Item item { get; set; }
    override public QuestType GetQuestType()
    {
        return QuestType.Gather;
    }
    public GatherQuestUpdate(Item i)
    {
        item = i;
    }
}
public class GatherQuest : Quest
{

    private ItemType item = 0;
    private int count = 0;
    private int target = 0;

    public GatherQuest(ItemType type, int count)
    {
        item = type;
        target = count;
    }

    public override bool Success()
    {
        return count >= target;
    }

    public override QuestType GetQuestType()
    {
        return QuestType.Gather;
    }

    public override void Update(QuestUpdate update)
    {
        if(update.GetQuestType() == QuestType.Gather)
        {
            GatherQuestUpdate u = (GatherQuestUpdate)update;
            if (u.item.type == item) count += u.item.count;
        }
    }

    public override List<Item> Complete(out Item rewardOut)
    {
        if (Success())
        {
            rewardOut = reward;
        }
        else
        {
            rewardOut = null;
        }
        return new List<Item>() { new Item(item, target) };
    }

    public override string GetString()
    {
        if (!Success()) return "Gather Quest\nPlease Collect " + item + "\nCurrent Amount: " + count + "\nNeeded Amount: " + target + "\nReward: " + reward.count + "x " + reward.type;
        else return "Claim Reward: " + reward.count + "x " + reward.type;
    }
}
