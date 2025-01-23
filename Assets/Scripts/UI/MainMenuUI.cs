using System;
using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons for infinity modes")]
    [SerializeField] private Button _chooseInfinitymodeButton;
    [SerializeField] private Button _infinityNewGameButton;
    [SerializeField] private Button _infinityContinuedGameButton;
    [SerializeField] private Button _hardcoreNewGameMenuButton;
    [SerializeField] private Button _hardcoreContinuedGameMenuButton;
    [SerializeField] private Button _timeTrialNewGameButton;
    [SerializeField] private Button _timeTrialContinuedGameButton;

    [Header("Buttons for classic modes")]
    [SerializeField] private Button _newClassicGameButton;
    [SerializeField] private Button _easyLvlButton;
    [SerializeField] private Button _mediumLvlButton;
    [SerializeField] private Button _hardLvlButton;
    [SerializeField] private Button _customLvlButton;
    [SerializeField] private Button _continueClassicGameButton;
    //[SerializeField] private Button _classicGameMenuButton;
    //[SerializeField] private Button _episodeGameMenuButton;

    [Header("Buttons")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _bonusButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text[] _awardTexts;
    [SerializeField] private TMP_Text _hardcoreNewGameText;
    [SerializeField] private TMP_Text _timeTrialNewGameText;

    [Header("Screens")]
    [SerializeField] private Canvas[] _mainMenuScreens;
    [SerializeField] private Canvas _settingsScreen;
    [SerializeField] private Canvas _infinityModeMenuScreen;
    [SerializeField] private Canvas _classicModeMenuScreen;
    [SerializeField] private Canvas _customGameSettingsScreen;
    [SerializeField] private Canvas _bonusScreen;

    [Header("Home Mode Screen Texts")]
    [SerializeField] private TMP_Text _lastModeText;
    [SerializeField] private TMP_Text _timeSpentText;
    [SerializeField] private TMP_Text _cellsOpenText;
    [SerializeField] private TMP_Text _flagsPlacedText;

    [Header("Ñlassic Modes Menu Screen Texts")]
    //[SerializeField] private TMP_Text _lastClassicSessionText;
    [SerializeField] private TMP_Text _difficultyLevelText;   
    [SerializeField] private TMP_Text _classicSessionCellsOpenText;
    [SerializeField] private TMP_Text _classicSessionFlagsPlacedText;
    [SerializeField] private TMP_Text _classicSessionTimeSpentText;

    [Header("Objects")]
    [SerializeField] private GameObject _containerLastSession;
    [SerializeField] private GameObject _startTutorialObject;

    private void Awake()
    {
        //SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastSessionStatistic);
        //SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastClassicSessionStatistic);
        //SignalBus.Subscribe<PlayerProgressLoadCompletedSignal>(PlayButtonInteractable);

        //SignalBus.Subscribe<OnGameRewardSignal>(UpdateAwardText);
        //SignalBus.Subscribe<OnGameRewardSignal>(CheckNewGameButtonsInteractable);    
        //_sceneLoader = SceneLoader.Instance;

        SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastSessionStatistic);
        SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastClassicSessionStatistic);
    }

    private void Start()
    {
        _infinityNewGameButton.onClick.AddListener(NewInfinityGame);
        _infinityContinuedGameButton.onClick.AddListener(ContinuedInfinityGame);
        _hardcoreNewGameMenuButton.onClick.AddListener(NewHardcoreGame);
        _hardcoreContinuedGameMenuButton.onClick.AddListener(ContinuedHardcoreGame);
        _timeTrialNewGameButton.onClick.AddListener(NewTimeTrialGame);
        _timeTrialContinuedGameButton.onClick.AddListener(ContinuedTimeTrialGame);

        _playButton.onClick.AddListener(OpenChooseModeScreen);
        _easyLvlButton.onClick.AddListener(NewClassicEasyGame);
        _mediumLvlButton.onClick.AddListener(NewClassicMediumGame);
        _hardLvlButton.onClick.AddListener(NewClassicHardGame);
        _customLvlButton.onClick.AddListener(OpenCustomGameSettingsScreen);
        //_episodeGameMenuButton.onClick.AddListener(OpenEpisodeGame);
        _settingsButton.onClick.AddListener(OpenSettingsScreen);
        _continueGameButton.onClick.AddListener(ContinueLastSession);
        _continueClassicGameButton.onClick.AddListener(ContinueLastClassicSession);
        _bonusButton.onClick.AddListener(OpenBonusScreen);

        _chooseInfinitymodeButton.onClick.AddListener(OpenInfinityModeMenuScreen);
        _newClassicGameButton.onClick.AddListener(OpenClassicModeMenuScreen);

        UpdateNewGameButtonsText();
        ContinueClassicButtonInteractable();        
        CheckNewGameButtonsInteractable(new OnGameRewardSignal());
        UpdateLastSessionStatistic(new GameManagerLoadCompletedSignal());
        UpdateLastClassicSessionStatistic(new GameManagerLoadCompletedSignal());
        //UpdateLastSessionStatistic(new LoadCompletedSignal());        
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

        _infinityModeMenuScreen.gameObject.SetActive(false);
        _classicModeMenuScreen.gameObject.SetActive(false);
    }

    private void ContinueLastSession()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentStatisticController != null)
        {
            if (GameManager.Instance.LastSessionGameMode == GameMode.SimpleInfinite ||
                GameManager.Instance.LastSessionGameMode == GameMode.Hardcore ||
                GameManager.Instance.LastSessionGameMode == GameMode.TimeTrial)
            {
                GameManager.Instance.SetCurrentGameMode(GameManager.Instance.LastSessionGameMode);
                SceneLoader.Instance.LoadInfiniteMinesweeperScene();
            }
            else if (GameManager.Instance.LastSessionGameMode == GameMode.ClassicEasy ||
                GameManager.Instance.LastSessionGameMode == GameMode.ClassicMedium ||
                GameManager.Instance.LastSessionGameMode == GameMode.ClassicHard ||
                GameManager.Instance.LastSessionGameMode == GameMode.Custom)
            {
                GameManager.Instance.SetCurrentGameMode(GameManager.Instance.LastSessionGameMode);
                SceneLoader.Instance.LoadClassicMinesweeperScene();
            }
        }
        else
        {
            GameManager.Instance.SetCurrentGameMode(GameMode.SimpleInfinite);
            SceneLoader.Instance.LoadInfiniteMinesweeperScene();
        }
    }

    private void ContinueLastClassicSession()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentStatisticController != null)
        {
            GameManager.Instance.SetCurrentGameMode(GameManager.Instance.LastClassicSessionMode);
            SceneLoader.Instance.LoadClassicMinesweeperScene();
        }        
    }

    private void NewInfinityGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.SimpleInfinite);
        GameManager.Instance.ClearCurrentGame(GameMode.SimpleInfinite);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
        //Debug.Log($"{GameManager.Instance.CurrentGameMode}");

        PlayerProgress.Instance.SetFirstTimePlayed();
    }

    private void ContinuedInfinityGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.SimpleInfinite);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
    }

    private void NewHardcoreGame()
    {
        if (PlayerProgress.Instance.CheckAwardCount(GameManager.Instance.HardcoreTimeModeCost))
        {            
            SignalBus.Fire(new OnGameRewardSignal(0, -GameManager.Instance.HardcoreTimeModeCost));

            GameManager.Instance.SetCurrentGameMode(GameMode.Hardcore);
            GameManager.Instance.ClearCurrentGame(GameMode.Hardcore);
            SceneLoader.Instance.LoadInfiniteMinesweeperScene();
            //Debug.Log($"{GameManager.Instance.CurrentGameMode}");

            PlayerProgress.Instance.SetFirstTimePlayed();
        }       
    }

    private void ContinuedHardcoreGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.Hardcore);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
    }

    private void NewTimeTrialGame()
    {
        if (PlayerProgress.Instance.CheckAwardCount(GameManager.Instance.HardcoreTimeModeCost))
        {
            SignalBus.Fire(new OnGameRewardSignal(0, -GameManager.Instance.HardcoreTimeModeCost));

            GameManager.Instance.SetCurrentGameMode(GameMode.TimeTrial);
            GameManager.Instance.ClearCurrentGame(GameMode.TimeTrial);
            SceneLoader.Instance.LoadInfiniteMinesweeperScene();
            //Debug.Log($"{GameManager.Instance.CurrentGameMode}");

            PlayerProgress.Instance.SetFirstTimePlayed();
        }        
    }

    private void ContinuedTimeTrialGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.TimeTrial);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
    }

    private void NewClassicEasyGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.ClassicEasy);
        GameManager.Instance.ClearCurrentGame(GameMode.ClassicEasy);
        //GameManager.Instance.ResetCurrentModeStatistic();
        SceneLoader.Instance.LoadClassicMinesweeperScene();

        PlayerProgress.Instance.SetFirstTimePlayed();
    }

    private void NewClassicMediumGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.ClassicMedium);
        GameManager.Instance.ClearCurrentGame(GameMode.ClassicMedium);
        //GameManager.Instance.ResetCurrentModeStatistic();
        SceneLoader.Instance.LoadClassicMinesweeperScene();

        PlayerProgress.Instance.SetFirstTimePlayed();
    }

    private void NewClassicHardGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.ClassicHard);
        GameManager.Instance.ClearCurrentGame(GameMode.ClassicHard);
        //GameManager.Instance.ResetCurrentModeStatistic();
        SceneLoader.Instance.LoadClassicMinesweeperScene();

        PlayerProgress.Instance.SetFirstTimePlayed();
    }

    private void OpenCustomGameSettingsScreen()
    {
        _customGameSettingsScreen.gameObject.SetActive(true);
    }

    private void OpenChooseModeScreen()
    {
        SetActiveScreen(1);
    }

    private void OpenInfinityModeMenuScreen() 
    {        
        _infinityModeMenuScreen.gameObject.SetActive(true);
    }

    private void OpenClassicModeMenuScreen()
    {
        _classicModeMenuScreen.gameObject.SetActive(true);
        ContinueClassicButtonInteractable();
    }

    private void OpenBonusScreen()
    {
        _bonusScreen.gameObject.SetActive(true);
    }

    private void OpenEpisodeGame()
    {

    }

    private void OpenSettingsScreen()
    {
        _settingsScreen.gameObject.SetActive(true);
    }


    private void UpdateLastSessionStatistic(GameManagerLoadCompletedSignal signal)
    {
        StartCoroutine(UpdateLastSession(signal));
    }

    private IEnumerator UpdateLastSession(GameManagerLoadCompletedSignal signal)
    {
        yield return null;
        
        if (GameManager.Instance != null && GameManager.Instance.CurrentStatisticController != null)
        {
            _lastModeText.text = GetGameModeName(GameManager.Instance.LastSessionGameMode);
            _timeSpentText.text = "Time spent: " + FormatTime(GameManager.Instance.CurrentStatisticController.TotalPlayTime);
            _flagsPlacedText.text = "Flags Placed: " + GameManager.Instance.CurrentStatisticController.PlacedFlags.ToString();
            _cellsOpenText.text = "Cells Open: " + GameManager.Instance.CurrentStatisticController.OpenedCells.ToString();
        }
        else
        {
            _lastModeText.text = "Choose mode";
            _timeSpentText.text = "Time spent: 0";
            _flagsPlacedText.text = "Flags Placed: 0";
            _cellsOpenText.text = "Cells Open: 0";
        }
    }

    private void UpdateLastClassicSessionStatistic(GameManagerLoadCompletedSignal signal)
    {
        if (GameManager.Instance != null && GameManager.Instance.ClassicStats != null)
        {
            //_lastClassicSessionText.text = "Last session " + GetGameModeName(GameManager.Instance.LastClassicSessionMode);
            _difficultyLevelText.text = "Difficulty level: " + GetGameModeDifficulty(GameManager.Instance.LastClassicSessionMode);            
            _classicSessionCellsOpenText.text = "Cells Open: " + GameManager.Instance.ClassicStats.OpenedCells.ToString();
            _classicSessionFlagsPlacedText.text = "Flags Placed: " + GameManager.Instance.ClassicStats.PlacedFlags.ToString();
            _classicSessionTimeSpentText.text = "Time spent: " + FormatTime(GameManager.Instance.ClassicStats.TotalPlayTime);            
        }
    }

    private string GetGameModeName(GameMode lastMode)
    {
        switch (lastMode)
        {
            case GameMode.SimpleInfinite:
                return "Infinity";

            case GameMode.Hardcore:
                return "Hardcore";

            case GameMode.TimeTrial:
                return "Time";

            case GameMode.ClassicEasy:
                return "Classic easy";

            case GameMode.ClassicMedium:
                return "Classic medium";

            case GameMode.ClassicHard:
                return "Classic hard";

            case GameMode.Custom:
                return "Custom level";

            default:
                return "Choose mode";
        }        
    }

    private string GetGameModeDifficulty(GameMode lastMode)
    {
        switch (lastMode)
        {           
            case GameMode.ClassicEasy:
                return "Easy";

            case GameMode.ClassicMedium:
                return "Medium";

            case GameMode.ClassicHard:
                return "Hard";

            case GameMode.Custom:
                return "Custom";

            default:
                return "";
        }
    }

    private string FormatTime(float totalSeconds)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600) / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);

        if (hours > 0)
        {
            return $"{hours} h. {minutes} min. {seconds} sec.";
        }
        else if (minutes > 0)
        {
            return $"{minutes} min. {seconds} sec.";
        }
        else
        {
            return $"{seconds} sec.";
        }
    }

    private void ContinueClassicButtonInteractable()
    {
        _continueClassicGameButton.interactable = SaveManager.Instance.HasClassicGameSave();
        SignalBus.Fire(new ThemeChangeSignal(ThemeManager.Instance.CurrentTheme, ThemeManager.Instance.CurrentThemeIndex));
    }

    private void PlayButtonInteractable(PlayerProgressLoadCompletedSignal signal)
    {
        if (PlayerProgress.Instance != null)
        {
            _startTutorialObject.gameObject.SetActive(!PlayerProgress.Instance.IsFirstTimePlayed);
            _containerLastSession.SetActive(PlayerProgress.Instance.IsFirstTimePlayed);


            UpdateAwardText(new OnGameRewardSignal());              ///KOSTYLISCHE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
    }

    private void UpdateAwardText(OnGameRewardSignal signal)
    {
        StartCoroutine(UpdateAwardTextDelayed(signal));        
    }

    private IEnumerator UpdateAwardTextDelayed(OnGameRewardSignal signal)
    {
        yield return null;
        foreach (var text in _awardTexts)
        {
            text.text = PlayerProgress.Instance.TotalReward.ToString();
        }
    }

    private void UpdateNewGameButtonsText()
    {
        _hardcoreNewGameText.text = $"New Game \n<sprite=0> {GameManager.Instance.HardcoreTimeModeCost}";
        _timeTrialNewGameText.text = $"New Game \n<sprite=0> {GameManager.Instance.HardcoreTimeModeCost}";
    }

    private void CheckNewGameButtonsInteractable(OnGameRewardSignal signal)
    {
        _hardcoreNewGameMenuButton.interactable = PlayerProgress.Instance.CheckAwardCount(GameManager.Instance.HardcoreTimeModeCost); 
        _timeTrialNewGameButton.interactable = PlayerProgress.Instance.CheckAwardCount(GameManager.Instance.HardcoreTimeModeCost); 
    }


    private void OnEnable()
    {
        //SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastSessionStatistic);        
        //UpdateLastSessionStatistic(new GameManagerLoadCompletedSignal());

        //SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastClassicSessionStatistic);
        //UpdateLastClassicSessionStatistic(new GameManagerLoadCompletedSignal());

        SignalBus.Subscribe<PlayerProgressLoadCompletedSignal>(PlayButtonInteractable);
        PlayButtonInteractable(new PlayerProgressLoadCompletedSignal());

        SignalBus.Subscribe<OnGameRewardSignal>(UpdateAwardText);
        SignalBus.Subscribe<OnGameRewardSignal>(CheckNewGameButtonsInteractable);        
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<GameManagerLoadCompletedSignal>(UpdateLastSessionStatistic);
        SignalBus.Unsubscribe<PlayerProgressLoadCompletedSignal>(PlayButtonInteractable);
        SignalBus.Unsubscribe<OnGameRewardSignal>(UpdateAwardText);
        SignalBus.Unsubscribe<OnGameRewardSignal>(CheckNewGameButtonsInteractable);
    }
}
