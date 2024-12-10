using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private string _mainMenuScene;
    [SerializeField] private string _infiniteMinesweeperScene;
    [SerializeField] private string _classicMinesweeperScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMainMenuScene()
    {
        LoadScene(_mainMenuScene);
    }

    public void LoadInfiniteMinesweeperScene()
    {
        LoadScene(_infiniteMinesweeperScene);
        //Debug.Log($"LOAD SCENE: IsDownloadedInfiniteGame: {GameModesManager.Instance.IsDownloadedInfiniteGame}, IsNewInfiniteGame: {GameModesManager.Instance.IsNewInfiniteGame}");
    }

    public void LoadClassicMinesweeperScene()
    {
        LoadScene(_classicMinesweeperScene);
    }

    private void LoadScene(string sceneName)
    {
        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is not added to Build Settings!");
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
