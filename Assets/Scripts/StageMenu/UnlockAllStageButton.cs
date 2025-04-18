using UnityEngine;
using UnityEngine.SceneManagement;

public class UnlockAllStageButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        const int totalStages = 4;

        for (var i = 0; i < totalStages; i++)
        {
            PlayerPrefs.SetInt($"Stage_{i}", 1);
        }

        PlayerPrefs.Save();
        SceneManager.LoadScene("StageSelect");
    }
}
