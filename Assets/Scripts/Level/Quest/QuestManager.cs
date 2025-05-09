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
   public event Action<int, int> QuestProgressChanged;
   public event Action<QuestData> QuestCompleted;
   
   public static QuestManager Instance {get; private set;}

   public QuestData[] fallbackQuestList;

   private int TotalQuestCnt {get; set;}
   private int CompletedQuestCnt {get; set;}
   
   public readonly Dictionary<string, QuestData> Quests = new();
   private readonly Dictionary<string, QuestProgress> _progresses = new();

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
      RecipeManager.Instance.OnRecipeFinished += CheckRecipe;
      BattleManager.Instance.CheckStageClear += OnCheckStageClear;
   }

   public async UniTask Init()
   {
      if (StageInfo.SelectedLevel != null)
      {
         var label = StageInfo.SelectedLevel.sceneName + " Quests";
         Debug.Log(label);
        
         await LoadQuests(label);
      }
      else
      {
         var activeScene = SceneManager.GetActiveScene();
         var tmp = activeScene.name.Split(" ");
         
         var handle = Addressables.LoadAssetsAsync<LevelData>("Level", null);
      
         if (handle.Status == AsyncOperationStatus.Failed)
         {
            Debug.LogError($"로드 실패: {handle.OperationException} Level: {activeScene.name}");
         }
         
         var data = await handle.ToUniTask();
         
         StageInfo.SelectedLevel = data[int.Parse(tmp[1])-1];
         
         await LoadQuests($"{activeScene.name} Quests");
         
         Debug.Log($"{StageInfo.SelectedLevel.levelIndex} Quests");
         Debug.Log($"{activeScene.name} Quests");
      }
   }

   private async UniTask LoadQuests(string stageLabel)
   {
      var data = await LoadQuestsWithFallback(stageLabel);

      for (var i = 0; i < data.Count; i++)
      {
         var quest = data[i];
         quest.idxInQuestList = i;
         Quests[quest.questID] = quest;
         _progresses[quest.questID] = new QuestProgress(quest.questID);
      } 
      
      TotalQuestCnt = Quests.Count - 1;
      CompletedQuestCnt = 0;
      
      QuestProgressChanged?.Invoke(TotalQuestCnt, CompletedQuestCnt);
   }
   
   private async UniTask<IList<QuestData>> LoadQuestsWithFallback(string stageLabel)
   {
      var handle = Addressables.LoadAssetsAsync<QuestData>(stageLabel, null);
      try
      {
         var result = await handle.ToUniTask();
         return result;
      }
      catch (Exception e)
      {
         Debug.Log($"로드 실패: {e.Message} {stageLabel}");
         return fallbackQuestList;
      }
   }
   
   public static void GameClear(bool isClear)
   {
      if (StageInfo.SelectedLevel == null)
      {
         Debug.Log("Test 중");
      }
      else
      {
         if (isClear)
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
         }
         else
         {
            Debug.Log("클리어 실패");
            UIManager.Instance.OpenConfirmMessage(Global.StageFailedMessageText,
               () => SceneManager.LoadScene("StageSelect"));
            return;
         }
      }
      
      UIManager.Instance.OpenConfirmMessage(Global.StageClearMessageText,
         () => SceneManager.LoadScene("StageSelect"));
   }
   
   private void OnCheckStageClear()
   {
      var allCards = GameTableManager.Instance.cardsOnTable;
      if (allCards == null || allCards.Count == 0) 
         return;
    
      var hasPlayer = allCards.Any(c => c.cardData.cardType == CardType.Person);
      var hasEnemy  = allCards.Any(c => c.cardData.cardType == CardType.Enemy);
    
      if (!hasPlayer)
      {
         Debug.Log("💀 게임 오버!");
         ChangeComplete(Quests[QuestInfo.GameOverQuestID]);
         return;
      }

      if (hasEnemy) return;
      
      Debug.Log("🎉 스테이지 클리어!");
      ChangeComplete(Quests[QuestInfo.GameClearQuestID]);
   }

   public bool IsCompleted(string questID)
   {
      return _progresses.TryGetValue(questID, out var progress) && progress.IsCompleted;
   }

   private void CheckRecipe(Recipe recipe)
   {
      if (!recipe) return;
      foreach (var quest in Quests.Where(quest => quest.Value.questRecipe?.recipeName == recipe.recipeName))
      {
         ChangeComplete(quest.Value);
      }
   }

   private void ChangeComplete(QuestData quest)
   {
      var questID = quest.questID;

      if (!_progresses.TryGetValue(questID, out var progress)) return;
      if (progress.IsCompleted) return;
      
      progress.IsCompleted = true;
      CompletedQuestCnt++;

      if (questID == QuestInfo.GameOverQuestID)
      {
         GameClear(false);
         return;
      }
      
      QuestProgressChanged?.Invoke(TotalQuestCnt, CompletedQuestCnt);
      QuestCompleted?.Invoke(quest);
      
      if(questID ==  QuestInfo.GameClearQuestID) GameClear(true);
   }
}
