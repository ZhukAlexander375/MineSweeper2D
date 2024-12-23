using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _infinityNewGameButton;
    [SerializeField] private Button _infinityContinuedGameButton;    
    [SerializeField] private Button _hardcoreNewGameMenuButton;
    [SerializeField] private Button _hardcoreContinuedGameMenuButton;
    [SerializeField] private Button _timeTrialNewGameButton;
    [SerializeField] private Button _timeTrialContinuedGameButton;

    [SerializeField] private Button _classicGameMenuButton;
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
        _infinityNewGameButton.onClick.AddListener(NewInfinityGame);
        _infinityContinuedGameButton.onClick.AddListener(ContinuedInfinityGame);
        _hardcoreNewGameMenuButton.onClick.AddListener(NewHardcoreGame);
        _hardcoreContinuedGameMenuButton.onClick.AddListener(ContinuedHardcoreGame);
        _timeTrialNewGameButton.onClick.AddListener(NewTimeTrialGame);
        _timeTrialContinuedGameButton.onClick.AddListener(ContinuedTimeTrialGame);

        _classicGameMenuButton.onClick.AddListener(OpenClassicGame);
        _episodeGameMenuButton.onClick.AddListener(OpenEpisodeGame);        
        _settingsButton.onClick.AddListener(OpenSettingsScreen);
        _continueGameButton.onClick.AddListener(NewInfinityGame);

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

    private void NewInfinityGame() 
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.SimpleInfinite);
        GameManager.Instance.ClearCurrentGame(GameMode.SimpleInfinite);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
        //Debug.Log($"{GameManager.Instance.CurrentGameMode}");
    }

    private void ContinuedInfinityGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.SimpleInfinite);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
    }

    private void NewHardcoreGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.Hardcore);
        GameManager.Instance.ClearCurrentGame(GameMode.Hardcore);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
        //Debug.Log($"{GameManager.Instance.CurrentGameMode}");
    }

    private void ContinuedHardcoreGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.Hardcore);        
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
    }

    private void NewTimeTrialGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.TimeTrial);
        GameManager.Instance.ClearCurrentGame(GameMode.TimeTrial);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
        //Debug.Log($"{GameManager.Instance.CurrentGameMode}");
    }

    private void ContinuedTimeTrialGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.TimeTrial);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
    }

    private void OpenClassicGame()
    {
        SceneLoader.Instance.LoadClassicMinesweeperScene();
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
        if (GameManager.Instance != null)
        {
            //_flagsPlacedText.text = "Flags Placed: " + GameManager.Instance.CurrentGameModeData.GetPlacedFlags().ToString();
            //_cellsOpenText.text = "Cells Open: " + GameManager.Instance.CurrentGameModeData.GetOpenedCells().ToString();
        }
        else
        {
            _flagsPlacedText.text = "Flags Placed: 0";
            _cellsOpenText.text = "Cells Open: 0";
        }
    }
}
