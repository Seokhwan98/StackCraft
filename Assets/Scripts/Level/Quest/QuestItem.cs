using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private TMP_Text label;
    
    public void Init(QuestData data)
    {
        label.text = data.description;
        toggle.isOn = QuestManager.Instance.IsCompleted(data.questID);
        toggle.interactable = false;
    }

    public void OnChange()
    {
        toggle.isOn = true;
        label.text = $"<s>{label.text}</s>";
        label.alpha = 0.5f;
    }

    public void HideGoal()
    {
        label.text = "???";
    }

    public void ShowGoal(string description)
    {
        label.text = $"{description}";
    }
    
}
