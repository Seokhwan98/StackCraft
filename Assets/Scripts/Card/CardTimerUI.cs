using UnityEngine;
using UnityEngine.UI;

public class CardTimerUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private float _value = 0f;

    public void SetValue(float val)
    {
        _value = Mathf.Clamp01(val);
        fillImage.fillAmount = _value;
    }
}
