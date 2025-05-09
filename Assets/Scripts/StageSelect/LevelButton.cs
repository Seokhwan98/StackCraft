using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    [SerializeField] private LevelData levelData;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject lockIcon;

    public event Action<LevelData> OnHovered;
    
    private bool _isUnlocked;
    
    public void Init()
    {
        label.text = levelData.displayName;
        _isUnlocked = PlayerPrefs.GetInt($"Stage_{levelData.levelIndex}", levelData.unlockedByDefault ? 1 : 0) == 1;

        button.interactable = _isUnlocked;
        lockIcon.SetActive(!_isUnlocked);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHovered?.Invoke(levelData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHovered?.Invoke(null);
    }

    public void OnButtonClick()
    {
        if(!_isUnlocked) return;
        StageInfo.SelectedLevel = levelData;

        var allCanvas = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var canvas in allCanvas)
        {
            if (canvas.renderMode != RenderMode.ScreenSpaceCamera)
            {
                canvas.enabled = false;
            }
        }
        
        var sceneTransition = FindFirstObjectByType<SceneTransition>();
        sceneTransition.onFadeOutTransitionDone.AddListener(LoadScene);
        sceneTransition.StartFadeOutTransition();
    }

    private void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelData.sceneName);
    }
}