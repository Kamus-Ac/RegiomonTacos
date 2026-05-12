using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{

    [SerializeField] private Image _timerImage;



    private void Update()
    {
        _timerImage.fillAmount = GameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
