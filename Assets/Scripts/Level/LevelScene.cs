using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LevelScene : MonoBehaviour
{
    [SerializeField] private RectTransform scrollContent;
    
    private async void Start()
    {
        try
        {
            await UniTask.Yield();
        
            await QuestUIController.Instance.LoadQuestsUI();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent);
            
        }
        catch (Exception e)
        {
            throw ; // TODO 예외 처리
        }
    }
}
