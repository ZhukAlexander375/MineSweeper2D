using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoader : MonoBehaviour
{
    private ScenesConfig _scenesConfig;

    [Inject]
    public void Construct(ScenesConfig scenesConfig)
    {
        _scenesConfig = scenesConfig;
    }

    public void LoadScene(SceneType sceneType)
    {
        string sceneName = _scenesConfig.GetSceneName(sceneType);
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"Scene '{sceneType}' не найдена в SceneSettings!");
            return;
        }

        LoadSceneInternal(sceneName);
    }

    private void LoadSceneInternal(string sceneName)
    {
        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' не добавлена в Build Settings!");
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
