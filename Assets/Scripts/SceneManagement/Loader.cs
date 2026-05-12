using UnityEngine;
using UnityEngine.SceneManagement;
public static class Loader
{
    public enum Scene
    {
        MainMenu,
        JuegoMain,
        Loading
    }

    private static Scene _targetScene;

    public static void Load(Scene targetScene)
    {
        Loader._targetScene = targetScene;

        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(_targetScene.ToString());
    }

}
