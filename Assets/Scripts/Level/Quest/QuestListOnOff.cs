using UnityEngine;

public class QuestListOnOff : MonoBehaviour
{
    [SerializeField] private GameObject questBar;
    private bool _isOpen = true;

    public void OnButtonClick()
    {
        _isOpen = !_isOpen;
        questBar.SetActive(_isOpen);
    }
}
