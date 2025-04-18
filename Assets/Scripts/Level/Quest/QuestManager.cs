using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
   public static QuestManager Instance {get; private set;}

   public GameObject questList;

   public int TotalQuestCnt {get; private set;}
   public int CompletedQuestCnt {get; private set;}

   public event Action<int, int> OnChangeQuestProgress;
   
   public event Action<QuestData> OnChangeQuestItemUI;

   public readonly Dictionary<string, QuestData> Quests = new();
   public readonly Dictionary<string, QuestProgress> Progresses = new();

   private void Awake()
   {
      Instance = this;
      if (Instance != this)
      {
         Destroy(this);
      }
   }

   private void Start()
   {
      OnChangeQuestProgress += QuestUIController.Instance.ChangeQuestProgress;
      OnChangeQuestItemUI += QuestUIController.Instance.ChangeQuestItemUI;

      RecipeManager.Instance.OnRecipeFinished += CheckQuestComplete;
   }

   public async UniTask Init()
   {

      if (StageInfo.SelectedLevel != null)
      {
         var label = StageInfo.SelectedLevel.displayName + " Quests";
         Debug.Log(label);
         var check = LoadQuests(label);
      
         await LoadQuests(label);
      }
      else
      {
         await LoadQuests("Stage 1 Quests");
      }
   }

   private async UniTask LoadQuests(string stageLabel)
   {
      var handle = Addressables.LoadAssetsAsync<QuestData>(stageLabel, null);
      
      if (handle.Status == AsyncOperationStatus.Failed)
      {
         Debug.LogError($"로드 실패: {handle.OperationException} {stageLabel}");
      }
      
      var data = await handle.ToUniTask();

      for (var i = 0; i < data.Count; i++)
      {
         var quest = data[i];
         quest.idxInQuestList = i;
         Quests[quest.questID] = quest;
         Progresses[quest.questID] = new QuestProgress(quest.questID);
      } 
      
      TotalQuestCnt = Quests.Count;
      CompletedQuestCnt = 0;
      
      OnChangeQuestProgress?.Invoke(TotalQuestCnt, CompletedQuestCnt);

      Debug.Log($"총 퀘스트 {TotalQuestCnt}개 로드 완료");
   }

   public void CheckQuestComplete(Recipe recipe)
   {
      foreach (var quest in Quests.Where(quest => quest.Value.questRecipe == recipe))
      {
         ChangeComplete(quest.Value);
      }
   }

   public void ChangeComplete(QuestData quest)
   {
      string questID = quest.questID;
      int idxInQuestList = quest.idxInQuestList;
      
      if (!Progresses.TryGetValue(questID, out var progress)) return;
      if (progress.IsCompleted) return;
      
      progress.IsCompleted = true;
      CompletedQuestCnt++;
      
      OnChangeQuestProgress?.Invoke(TotalQuestCnt, CompletedQuestCnt);
      OnChangeQuestItemUI?.Invoke(quest);
      
      switch (questID)
      {
         case QuestInfo.GameClearQuestID:
            GameClear();
            break;
      }
   }

   public static void GameClear()
   {
      var next = StageInfo.SelectedLevel.levelIndex + 1;
      
      if (next < 4)
      {
         PlayerPrefs.SetInt($"Stage_{next}", 1);
         PlayerPrefs.Save();
      }
      else
      {
         for (var i = 1; i <= 4; i++)
         {
            PlayerPrefs.DeleteKey("Stage_" + next);
         }
      }

      SceneManager.LoadScene("StageSelect");

   }

   public bool IsCompleted(string questID)
   {
      return Progresses.TryGetValue(questID, out var progress) && progress.IsCompleted;
   }

   
}
