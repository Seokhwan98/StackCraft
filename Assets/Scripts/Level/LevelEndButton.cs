using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        QuestManager.GameClear();
    }
}
