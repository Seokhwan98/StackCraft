using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetProgressButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        for (var i = 1; i < 4; i++)
        {
            PlayerPrefs.DeleteKey($"Stage_{i}");
        }

        PlayerPrefs.Save();
        SceneManager.LoadScene("StageSelect");
    }
}
