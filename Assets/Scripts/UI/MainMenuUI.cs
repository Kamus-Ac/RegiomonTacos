using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button quitBtn;


    private void Awake()
    {
        playBtn.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.JuegoMain);
        });

        quitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        //Asegurarse de que el juego no esté pausado al volver al menú principal
        Time.timeScale = 1f;

    }
}

