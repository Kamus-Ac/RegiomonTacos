using UnityEngine;

public class SelectedBlockVisual : MonoBehaviour
{

    [SerializeField] private BaseBlock _baseBlock;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        Player.Instance.OnSelectedBlockChanged += Player_OnSelectedBlockChanged;
        //HideCounter();
    }


    private void Player_OnSelectedBlockChanged(object sender, Player.OnSelectedBlockChangedEventArgs e)
    {
        if (e.selectedBlock == _baseBlock)
        {
            ShowCounter();
        }
        else
        {
            HideCounter();
        }
    }


    private void ShowCounter()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }

    }
    private void HideCounter()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}
