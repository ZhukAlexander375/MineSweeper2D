using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _infinityGameButton;
    [SerializeField] private Button _classicGameMenuButton;
    [SerializeField] private Button _hardcoreGameMenuButton;
    [SerializeField] private Button _episodeGameMenuButton;    
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _continueGameButton;

    [Header("Screens")]
    [SerializeField] private Canvas[] _mainMenuScreens;
    [SerializeField] private Canvas _settingsScreen;

    [SerializeField] private TMP_Text _timeSpentText;
    [SerializeField] private TMP_Text _cellsOpenText;
    [SerializeField] private TMP_Text _flagsPlacedText;

    private SceneLoader _sceneLoader;
    private PlayerProgress _playerProgress;

    private void Awake()
    {
        //_sceneLoader = SceneLoader.Instance;
    }

    private void Start()
    {
        _sceneLoader = SceneLoader.Instance;
        _playerProgress = PlayerProgress.Instance;

        _infinityGameButton.onClick.AddListener(OpenInfinityGame);
        _classicGameMenuButton.onClick.AddListener(OpenClassicGame);
        _hardcoreGameMenuButton.onClick.AddListener(OpenHardcoreGame);
        _episodeGameMenuButton.onClick.AddListener(OpenEpisodeGame);        
        _settingsButton.onClick.AddListener(OpenSettingsScreen);
        _continueGameButton.onClick.AddListener(OpenInfinityGame);

        StartCoroutine(DelayedInitialization());        
    }

    public void SelectMenu(int index)
    {
        SetActiveScreen(index);
    }

    private void SetActiveScreen(int index)
    {
        for (int i = 0; i < _mainMenuScreens.Length; i++)
        {
            _mainMenuScreens[i].gameObject.SetActive(i == index);
        }
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

    private void OpenSettingsScreen()
    {
        _settingsScreen.gameObject.SetActive(true);
    }

    private IEnumerator DelayedInitialization()     ///ZENJECT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    {
        yield return null;
        UpdateLastSessionStatistic();
    }

    private void UpdateLastSessionStatistic()
    {
        //_timeSpentText.text = _playerProgress.;
        _cellsOpenText.text = "Cells Open: " + _playerProgress.OpenedCells.ToString();
        _flagsPlacedText.text = "Checkboxes placed: " + _playerProgress.PlacedFlags.ToString();
    }

}
