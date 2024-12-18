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
    [SerializeField] private Button _timeTrialGameButton;
    [SerializeField] private Button _episodeGameMenuButton;    
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _continueGameButton;

    [Header("Screens")]
    [SerializeField] private Canvas[] _mainMenuScreens;
    [SerializeField] private Canvas _settingsScreen;

    [SerializeField] private TMP_Text _timeSpentText;
    [SerializeField] private TMP_Text _cellsOpenText;
    [SerializeField] private TMP_Text _flagsPlacedText;
        
    
    private void Awake()
    {
        //_sceneLoader = SceneLoader.Instance;
    }

    private void Start()
    {           
        _classicGameMenuButton.onClick.AddListener(OpenClassicGame);
        _infinityGameButton.onClick.AddListener(OpenInfinityGame);
        _hardcoreGameMenuButton.onClick.AddListener(OpenHardcoreGame);
        _timeTrialGameButton.onClick.AddListener(OpenTimeTrialGame);
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
        GameManager.Instance.SetCurrentGameMode(GameMode.SimpleInfinite);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
        //Debug.Log($"{GameManager.Instance.CurrentGameMode}");
    }

    private void OpenClassicGame()
    {
        SceneLoader.Instance.LoadClassicMinesweeperScene();
    }

    private void OpenHardcoreGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.Hardcore);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
        //Debug.Log($"{GameManager.Instance.CurrentGameMode}");
    }

    private void OpenTimeTrialGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.TimeTrial);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
        //Debug.Log($"{GameManager.Instance.CurrentGameMode}");
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
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameModeData != null)
        {
            _flagsPlacedText.text = "Flags Placed: " + GameManager.Instance.CurrentGameModeData.GetPlacedFlags().ToString();
            _cellsOpenText.text = "Cells Open: " + GameManager.Instance.CurrentGameModeData.GetOpenedCells().ToString();
        }
        else
        {
            _flagsPlacedText.text = "Flags Placed: 0";
            _cellsOpenText.text = "Cells Open: 0";
        }
    }
}
