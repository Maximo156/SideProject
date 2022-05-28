using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntQuestUpdate : QuestUpdate
{
    public string tag { get; set; }
    override public QuestType GetQuestType()
    {
        return QuestType.Hunt;
    }
    public HuntQuestUpdate(string tag)
    {
        this.tag = tag;
    }
}
public class HuntQuest : Quest
{
    string tag;
    int count = 0;
    int needed;
    public HuntQuest(string tag, int count)
    {
        this.tag = tag;
        this.needed = count;
    }

    public override QuestType GetQuestType()
    {
        return QuestType.Hunt;
    }


    public override bool Success()
    {
        return count >= needed;
    }

    public override void Update(QuestUpdate update)
    {
        if(update.GetQuestType() == QuestType.Hunt)
        {
            if (((HuntQuestUpdate)update).tag.ToLower() == tag.ToLower()) count++;
        }
    }

    public override string GetString()
    {
        if (!Success()) return "Hunting Quest\nKill " + tag + "s\nCurrent Kills: " + count + "\nNeeded Kills: " + needed + "\nReward: " + reward.count + "x " + reward.type;
        else return "Claim Reward: " + reward.count + "x " + reward.type;
    }
}
