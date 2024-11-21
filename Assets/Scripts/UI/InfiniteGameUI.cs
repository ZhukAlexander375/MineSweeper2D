using UnityEngine;
using UnityEngine.UI;

public class InfiniteGameUI : MonoBehaviour
{
    [SerializeField] private Button _backToMenuButton;

    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = FindObjectOfType<SceneLoader>();
    }

    private void Start()
    {
        _backToMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void ReturnToMainMenu()
    {        
        _sceneLoader.LoadMainMenuScene();
    }
}
