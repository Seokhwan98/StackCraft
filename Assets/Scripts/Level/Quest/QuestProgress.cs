using UnityEngine;

public class QuestProgress
{
    public string QuestID;
    public bool IsCompleted;

    public QuestProgress(string questID)
    {
        QuestID = questID;
        IsCompleted = false;
    }
}
