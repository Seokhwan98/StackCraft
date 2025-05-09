using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionUI: MonoBehaviour
{
    public enum DescriptionType
    {
        None,
        Confirm,
        YesOrNo
    }
    
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    public void SetDescription(DescriptionType descriptionType, string message, Action yesCallback = null, Action noCallback = null)
    {
        SetText(message);
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        if (yesCallback != null)
        {
            yesButton.onClick.AddListener(() => { yesCallback.Invoke(); ChangeToDefaultUI();});
        }
        else
        {
            yesButton.onClick.AddListener(ChangeToDefaultUI);
        }
        
        
        if (noCallback != null)
        {
            noButton.onClick.AddListener(() => { noCallback.Invoke(); ChangeToDefaultUI(); });
        }
        else
        {
            noButton.onClick.AddListener(ChangeToDefaultUI);
        }
        
        switch (descriptionType)
        {
            case DescriptionType.Confirm:
                noButton.gameObject.SetActive(false);
                break;
            case DescriptionType.YesOrNo:
                noButton.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(descriptionType), descriptionType, null);
        }
    }
    
    
    private void Awake()
    {
        Debug.Assert(descriptionText);
        Debug.Assert(yesButton);
        Debug.Assert(noButton);
    }

    private void ChangeToDefaultUI()
    {
        UIManager.Instance.ChangeUI(UIManager.defaultUI);
    }
    
    private void SetText(string message)
    {
        var text = TMPUtils.GetEllipsizedTextWithAutoSize(descriptionText, message);
        
        
        descriptionText.text = text;
    }
}