using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestScrollHandler : MonoBehaviour, IScrollHandler
{
    public static QuestScrollHandler Instance;
    public bool IsPointerOver { get; private set; }
    
    
    private ScrollRect _scrollRect;

    private void Awake()
    {
        Instance = this;
        if (Instance != this)
        {
            Destroy(this);
        }
        
        IsPointerOver = false;
        
        _scrollRect = GetComponent<ScrollRect>();
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (IsPointerOver)
        {
            _scrollRect.OnScroll(eventData);
        }
    }
    
    public void OnPointerEnter(BaseEventData data)
    {
        IsPointerOver = true;
    }
    
    public void OnPointerExit(BaseEventData data)
    {
        IsPointerOver = false;
    }

}