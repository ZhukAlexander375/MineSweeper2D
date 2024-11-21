using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _infinityGameButton;
    [SerializeField] private Button _classicGameMenuButton;
    [SerializeField] private Button _hardcoreGameMenuButton;
    [SerializeField] private Button _episodeGameMenuButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _themeButton;
    [SerializeField] private Button _settingsButton;

    [Header("Screens")]
    [SerializeField] private Canvas _classicGameScreen;
    [SerializeField] private Canvas _episodeGameScreen;
    [SerializeField] private Canvas _shopScreen;
    [SerializeField] private Canvas _themeScreen;
    [SerializeField] private Canvas _settingsScreen;

    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = FindObjectOfType<SceneLoader>();
    }

    private void Start()
    {
        _infinityGameButton.onClick.AddListener(OpenInfinityGame);
        _classicGameMenuButton.onClick.AddListener(OpenClassicGame);
        _hardcoreGameMenuButton.onClick.AddListener(OpenHardcoreGame);
        _episodeGameMenuButton.onClick.AddListener(OpenEpisodeGame);
        _shopButton.onClick.AddListener(OpenShopScreen);
        _themeButton.onClick.AddListener(OpenThemeScreen);
        _settingsButton.onClick.AddListener(OpenSettingsScreen);
    }


    private void OpenInfinityGame()
    {
        _sceneLoader.LoadInfiniteMinesweeperScene();
    }

    private void OpenClassicGame()
    {
        _sceneLoader.LoadClassicMinesweeperScene();
    }

    private void OpenHardcoreGame()
    {

    }

    private void OpenEpisodeGame()
    {

    }

    private void OpenShopScreen()
    {
        _shopScreen.gameObject.SetActive(true);
    }

    private void OpenThemeScreen()
    {
        _themeScreen.gameObject.SetActive(true);
    }

    private void OpenSettingsScreen()
    {
        _settingsScreen.gameObject.SetActive(true);
    }

}
