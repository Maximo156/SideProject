using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestType
{
    Gather,
    Hunt
}
public abstract class QuestUpdate
{
    public abstract QuestType GetQuestType();
}
public abstract class Quest
{
    private static List<string> enemyTags = new List<string>()
    {
        "Skeleton"
    };
    public static Quest GetQuest()
    {
        int amount = Enum.GetNames(typeof(QuestType)).Length;
        switch (UnityEngine.Random.Range(0, amount))
        {
            case 0:
                var items = Enum.GetValues(typeof(ItemType));
                ItemType i = (ItemType)items.GetValue(UnityEngine.Random.Range(0, items.Length));
                GatherQuest toReturnGather = new GatherQuest(i, UnityEngine.Random.Range(0, Item.TypeMaxCount[i]));
                return toReturnGather;
            case 1:
                string enemy = enemyTags[UnityEngine.Random.Range(0, enemyTags.Count)];
                HuntQuest toReturnHunt = new HuntQuest(enemy, UnityEngine.Random.Range(0, 15));
                return toReturnHunt;
        }
        return null;
    }

    public static List<Quest> options = new List<Quest>()
    {

    };

    int difficulty;
    protected Item reward = new Item(ItemType.Axe, 1);

    public abstract bool Success();

    public abstract QuestType GetQuestType();

    public abstract void Update(QuestUpdate update);

    public virtual List<Item> Complete(out Item rewardOut)
    {
        if (Success())
        {
            rewardOut = reward;
        }
        else
        {
            rewardOut = null;
        }
        return null;
    }

    public virtual void Start()
    {
        return;
    }

    public abstract string GetString();
}
