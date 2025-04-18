using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class QuestUIController : MonoBehaviour
{
    
    public static QuestUIController Instance;
    
    [SerializeField] private GameObject togglePrefab;
    [SerializeField] private Transform questListParent;
    
    [SerializeField] private TMP_Text progressText;


    private int _hideQuestIdx; 
    private string _hideQuestDescription;
    
    
    private void Awake()
    {
        Instance = this;
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    public QuestItem FindQuestItem(int index)
    {
        var item = questListParent.GetChild(index);
        return item.gameObject.GetComponent<QuestItem>();
    }
    

    public void ChangeQuestItemUI(QuestData questData)
    {
        var item = FindQuestItem(questData.idxInQuestList);
        
        if (questData.questID == QuestInfo.GoalOpenQuestID)
        {
            var tmp = FindQuestItem(_hideQuestIdx);
            tmp.ShowGoal(_hideQuestDescription);
        }
        
        item.OnChange();
    }

    public void ChangeQuestProgress(int total, int completed)
    {
        progressText.text = $"({completed}/{total})";
    }
    
    
    public async UniTask LoadQuestsUI()
    {
        await QuestManager.Instance.Init();
        
        var quests = QuestManager.Instance.Quests;
        var flag = false;

        foreach (var quest in quests)
        {
            var go = Instantiate(togglePrefab, questListParent);
            var questUI = go.GetComponent<QuestItem>();
            
            if(quest.Key == QuestInfo.GoalOpenQuestID) flag = true;
            questUI.Init(quest.Value);
            if (flag)
            {
                if (quest.Key == QuestInfo.GameClearQuestID)
                {
                    _hideQuestIdx = quest.Value.idxInQuestList;
                    _hideQuestDescription = quest.Value.description;
                    questUI.HideGoal();
                }
            }
            
            Debug.Log($"Loaded: {quest.Value.description}");
        }
        
        Canvas.ForceUpdateCanvases(); 
    }
}