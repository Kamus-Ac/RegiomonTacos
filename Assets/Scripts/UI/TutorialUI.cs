using UnityEngine;

public class TutorialUI : MonoBehaviour
{

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Show();
    }


    private void Hide(){
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if(GameManager.Instance.IsGamePlaying())
        {
            Hide();
        }
    }

}
