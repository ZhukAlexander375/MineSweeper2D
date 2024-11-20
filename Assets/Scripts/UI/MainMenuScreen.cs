using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] private Button _infinityGameButton;
    [SerializeField] private Button _classicGameButton;
    [SerializeField] private Button _hardcoreGameButton;
    [SerializeField] private Button _episodeGameButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _themeButton;
    [SerializeField] private Button _settingsButton;

    private void Start()
    {
        _infinityGameButton.onClick.AddListener(OpenInfinityGame);
        _classicGameButton.onClick.AddListener(OpenClassicGame);
        _hardcoreGameButton.onClick.AddListener(OpenHadrcoreGame);
        _episodeGameButton.onClick.AddListener(OpenEpisodeGame);
        _shopButton.onClick.AddListener(OpenShopScreen);
        _themeButton.onClick.AddListener(OpenThemeScreen);
        _settingsButton.onClick.AddListener(OpenSettingsScreen);
    }


    private void OpenInfinityGame()
    {

    }

    private void OpenClassicGame()
    {

    }

    private void OpenHadrcoreGame()
    {

    }

    private void OpenEpisodeGame()
    {

    }

    private void OpenShopScreen()
    {

    }

    private void OpenThemeScreen()
    {

    }

    private void OpenSettingsScreen()
    {

    }

}
