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
        StageInfoPanel.Instance?.SetInfo(levelData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StageInfoPanel.Instance?.Clear();
    }

    public void OnButtonClick()
    {
        if(!_isUnlocked) return;
        StageInfo.SelectedLevel = levelData;
       UnityEngine.SceneManagement.SceneManager.LoadScene(levelData.sceneName);
    }
    
}