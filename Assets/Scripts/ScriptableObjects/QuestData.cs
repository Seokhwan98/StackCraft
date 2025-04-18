using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    public string questID;
    public string description;

    public Recipe questRecipe;
    public int idxInQuestList;
}
