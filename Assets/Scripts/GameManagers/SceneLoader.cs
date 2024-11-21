using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private SceneAsset _mainMenuScene;
    [SerializeField] private SceneAsset _infiniteMinesweeperScene;
    [SerializeField] private SceneAsset _classicMinesweeperScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadMainMenuScene()
    {
        LoadScene(_mainMenuScene.name);
    }

    public void LoadInfiniteMinesweeperScene()
    {
        LoadScene(_infiniteMinesweeperScene.name);
    }

    public void LoadClassicMinesweeperScene()
    {
        LoadScene(_classicMinesweeperScene.name);
    }

    private void LoadScene(string sceneName)
    {
        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.Log($"Scene '{sceneName}' is not added to Build Settings!");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string loadedSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (loadedSceneName == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}
