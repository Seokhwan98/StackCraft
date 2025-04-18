using UnityEngine;
using TMPro;

public class StageInfoPanel : MonoBehaviour
{
    public static StageInfoPanel Instance;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        Instance = this;
    }

    public void SetInfo(LevelData data)
    {
        titleText.text = data.displayName;
        descriptionText.text = data.description;
    }
    
    public void Clear()
    {
        titleText.text = "";
        descriptionText.text = "";
    }
}
