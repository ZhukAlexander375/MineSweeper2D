using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassicGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _easyLvlButton;
    [SerializeField] private Button _mediumLvlButton;
    [SerializeField] private Button _hardLvlButton;
    [SerializeField] private Button _customLvlButton;
    [SerializeField] private Button _backToMainMenuButton;
    [SerializeField] private Button _backToMenuButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text _gameModeName;

    [Header("Screens")]
    [SerializeField] private Canvas _classicGameMenuScreen;
    [SerializeField] private Canvas _gameScreen;

    [Header("GridManager")]
    [SerializeField] private SampleGridManager _gridManager;
    [SerializeField] private GameObject _sampleGrid;

    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = FindObjectOfType<SceneLoader>();        
    }

    private void Start()
    {
        _easyLvlButton.onClick.AddListener(StartEasyLevel);
        _mediumLvlButton.onClick.AddListener(StartMediumLevel);
        _hardLvlButton.onClick.AddListener(StartHardLevel);
        _backToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
        _backToMenuButton.onClick.AddListener(ReturnToClassicGameMenu);
    }

    private void StartEasyLevel()
    {
        OpenGameScreen("Easy");
        _gridManager.gameObject.SetActive(true);        
        _gridManager.StartLevel(0);
    }

    private void StartMediumLevel()
    {
        OpenGameScreen("Medium");
        _gridManager.gameObject.SetActive(true);
        _gridManager.StartLevel(1);
    }

    private void StartHardLevel()
    {
        OpenGameScreen("Hard");
        _gridManager.gameObject.SetActive(true);
        _gridManager.StartLevel(2);
    }

    private void OpenGameScreen(string gameModeName)
    {
        _classicGameMenuScreen.gameObject.SetActive(false);
        _gameScreen.gameObject.SetActive(true);
        _gameModeName.text = gameModeName;
    }

    private void ReturnToMainMenu()
    {
        _sceneLoader.LoadMainMenuScene();
    }

    private void ReturnToClassicGameMenu()
    {
        _classicGameMenuScreen.gameObject.SetActive(true);
        _gameScreen.gameObject.SetActive(false);
        _gridManager.gameObject.SetActive(false);
    }
}
