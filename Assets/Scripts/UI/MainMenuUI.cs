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
    //[SerializeField] private Button _classicGameMenuButton;
    //[SerializeField] private Button _episodeGameMenuButton;

    [Header("Buttons")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _playButton;
    

    [Header("Screens")]
    [SerializeField] private Canvas[] _mainMenuScreens;
    [SerializeField] private Canvas _settingsScreen;
    [SerializeField] private Canvas _infinityModeMenuScreen;
    [SerializeField] private Canvas _classicModeMenuScreen;
    

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

        _playButton.onClick.AddListener(OpenInfinityModeMenuScreen);
        _easyLvlButton.onClick.AddListener(NewClassicEasyGame);
        _mediumLvlButton.onClick.AddListener(NewClassicMediumGame);
        _hardLvlButton.onClick.AddListener(NewClassicHardGame);
        //_classicGameMenuButton.onClick.AddListener(OpenClassicGame);
        //_episodeGameMenuButton.onClick.AddListener(OpenEpisodeGame);
        _settingsButton.onClick.AddListener(OpenSettingsScreen);
        _continueGameButton.onClick.AddListener(ContinueLastSession);

        _chooseInfinitymodeButton.onClick.AddListener(OpenInfinityModeMenuScreen);
        _newClassicGameButton.onClick.AddListener(OpenClassicModeMenuScreen);

        //UpdateLastSessionStatistic(new LoadCompletedSignal());
        //SignalBus.Subscribe<PlayerProgressLoadCompletedSignal>(UpdateLastSessionStatistic);
        //UpdateLastSessionStatistic(new PlayerProgressLoadCompletedSignal());
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
            GameManager.Instance.SetCurrentGameMode(GameManager.Instance.LastSessionGameMode);
            SceneLoader.Instance.LoadInfiniteMinesweeperScene();
        }
        else
        {
            GameManager.Instance.SetCurrentGameMode(GameMode.SimpleInfinite);
            SceneLoader.Instance.LoadInfiniteMinesweeperScene();
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
        GameManager.Instance.SetCurrentGameMode(GameMode.Hardcore);
        GameManager.Instance.ClearCurrentGame(GameMode.Hardcore);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
        //Debug.Log($"{GameManager.Instance.CurrentGameMode}");

        PlayerProgress.Instance.SetFirstTimePlayed();
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

        PlayerProgress.Instance.SetFirstTimePlayed();
    }

    private void ContinuedTimeTrialGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.TimeTrial);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
    }

    private void NewClassicEasyGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.ClassicEasy);
        GameManager.Instance.ResetCurrentModeStatistic();
        SceneLoader.Instance.LoadClassicMinesweeperScene();

        PlayerProgress.Instance.SetFirstTimePlayed();
    }

    private void NewClassicMediumGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.ClassicMedium);
        GameManager.Instance.ResetCurrentModeStatistic();
        SceneLoader.Instance.LoadClassicMinesweeperScene();

        PlayerProgress.Instance.SetFirstTimePlayed();
    }

    private void NewClassicHardGame()
    {
        GameManager.Instance.SetCurrentGameMode(GameMode.ClassicHard);
        GameManager.Instance.ResetCurrentModeStatistic();
        SceneLoader.Instance.LoadClassicMinesweeperScene();

        PlayerProgress.Instance.SetFirstTimePlayed();
    }

    private void OpenInfinityModeMenuScreen() 
    {        
        _infinityModeMenuScreen.gameObject.SetActive(true);
    }

    private void OpenClassicModeMenuScreen()
    {
        _classicModeMenuScreen.gameObject.SetActive(true);
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
            _difficultyLevelText.text = "Difficulty level " + GetGameModeDifficulty(GameManager.Instance.LastClassicSessionMode);            
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

            default:
                return "Choose mode";
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

    private void PlayButtonInteractable(PlayerProgressLoadCompletedSignal signal)
    {
        if (PlayerProgress.Instance != null)
        {
            _playButton.gameObject.SetActive(!PlayerProgress.Instance.IsFirstTimePlayed);
            _containerLastSession.SetActive(PlayerProgress.Instance.IsFirstTimePlayed);
        }        
    }


    private void OnEnable()
    {
        SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastSessionStatistic);        
        UpdateLastSessionStatistic(new GameManagerLoadCompletedSignal());

        SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastClassicSessionStatistic);
        UpdateLastClassicSessionStatistic(new GameManagerLoadCompletedSignal());

        SignalBus.Subscribe<PlayerProgressLoadCompletedSignal>(PlayButtonInteractable);
        PlayButtonInteractable(new PlayerProgressLoadCompletedSignal());
    }

    private void OnDisable()
    {
        SignalBus.Unsubscribe<GameManagerLoadCompletedSignal>(UpdateLastSessionStatistic);
        SignalBus.Unsubscribe<PlayerProgressLoadCompletedSignal>(PlayButtonInteractable);
    }
}
