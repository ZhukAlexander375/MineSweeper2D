using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ClassicGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingsOnPauseButton;
    [SerializeField] private Button _replayLevelButton;
    [SerializeField] private Button _goHomeButton;
    
    [Header("Texts")]
    [SerializeField] private TMP_Text _gameModeName;
    [SerializeField] private TMP_Text _minesCountText;
    [SerializeField] private TMP_Text _flagsText;

    [Header("Screens")]
    [SerializeField] private Canvas _pauseMenuScreen;
    [SerializeField] private Canvas _settingsScreen;
    [SerializeField] private Canvas _loseScreen;    
    [SerializeField] private Canvas _gameScreen;

    /*[Header("GridManager")]
    [SerializeField] private SimpleGridManager _gridManager;
    [SerializeField] private GameObject _sampleGrid;

    [Header("Settings For Custom Game")]
    [SerializeField] private int _minSize = 2;
    [SerializeField] private int _maxSize = 100;
    [SerializeField] private int _minMines = 1;
    [SerializeField] private int _maxMines = 5000;*/
    

    private SimpleGridManager _simpleGridManager;

    private void Awake()
    {        
        _simpleGridManager = FindObjectOfType<SimpleGridManager>();
    }

    private void Start()
    {
        _settingsButton.onClick.AddListener(OpenSettings);
        _pauseButton.onClick.AddListener(OpenPauseMenu);
        _continueButton.onClick.AddListener(ClosePauseMenu);
        _settingsOnPauseButton.onClick.AddListener(OpenSettings);
        _replayLevelButton.onClick.AddListener(ReplayGame);
        _goHomeButton.onClick.AddListener(ReturnToMainMenu);
        //_goHomeButtonOnPause.onClick.AddListener(ReturnToMainMenu);
                
        SetModeName();
    }

    private void OpenPauseMenu()
    {
        _pauseMenuScreen.gameObject.SetActive(true);
        GameManager.Instance.CurrentStatisticController.StopTimer();
    }

    private void ClosePauseMenu()
    {
        _pauseMenuScreen.gameObject.SetActive(false);
        GameManager.Instance.CurrentStatisticController.StartTimer();
    }

    private void OpenSettings()
    {
        _settingsScreen.gameObject.SetActive(true);
        GameManager.Instance.CurrentStatisticController.StopTimer();
    }

    private void ReplayGame()
    {
        _loseScreen.gameObject.SetActive(false);
        GameManager.Instance.ResetCurrentModeStatistic();
        GameManager.Instance.ClearCurrentGame(GameManager.Instance.CurrentGameMode);
        GameManager.Instance.SetCurrentGameMode(GameManager.Instance.CurrentGameMode);
        SceneLoader.Instance.LoadClassicMinesweeperScene();
    }


    private void SetModeName()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.ClassicEasy:
                _gameModeName.text = "Easy";
                break;

            case GameMode.ClassicMedium:
                _gameModeName.text = "Medium";
                break;

            case GameMode.ClassicHard:
                _gameModeName.text = "Hard";
                break;

            case GameMode.Custom:
                _gameModeName.text = "Custom";
                break;
        }
    }

    private void ReturnToMainMenu()
    {
        GameManager.Instance.CurrentStatisticController.StopTimer();
        PlayerProgress.Instance.SavePlayerProgress();

        if (_simpleGridManager.IsFirstClick)
        {            
            _simpleGridManager.SaveCurrentGame();
        }

        SceneLoader.Instance.LoadMainMenuScene();
    }

    private void OpenLoseScreen(GameOverSignal signal)
    {
        if (signal.CurrentGameMode == GameManager.Instance.CurrentGameMode)
        {
            if (signal.IsGameOver == true || signal.IsGameWin == true)
            {
                _loseScreen.gameObject.SetActive(true);
                _pauseButton.gameObject.SetActive(false);
                _pauseMenuScreen.gameObject.SetActive(false);
            }
            else
            {
                _loseScreen.gameObject.SetActive(false);
                _pauseButton.gameObject.SetActive(true);
            }
        }
    }

    private void UpdateFlagText(FlagPlacingSignal signal)
    {
        if (GameManager.Instance.CurrentStatisticController != null)
        {
            _flagsText.text = GameManager.Instance.CurrentStatisticController.PlacedFlags.ToString();
        }
        else
        {
            _flagsText.text = "0";
        }
    }

    private void UpdateBombText()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.ClassicEasy:
                SetMinesCount(0);
                break;

            case GameMode.ClassicMedium:
                SetMinesCount(1);
                break;

            case GameMode.ClassicHard:
                SetMinesCount(2);
                break;

            case GameMode.Custom:
                SetMinesCount(GameManager.Instance.CustomLevel);
                break;
        }
    }

    private void SetMinesCount(int levelIndex)
    {
        var levels = GameManager.Instance.PredefinedLevels;

        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            _minesCountText.text = levels[levelIndex].MineCount.ToString();
            //Debug.Log($"{levels[levelIndex].MineCount}");
        }
        else
        {
            Debug.LogError("Level index out of range");
        }
    }

    private void SetMinesCount(LevelConfig customLevel)
    {
        _minesCountText.text = customLevel.MineCount.ToString();
        //Debug.Log($"{customLevel.MineCount}");
    }

    private void UpdateTexts(LoadCompletedSignal signal)
    {
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        UpdateBombText();
        
        _flagsText.text = GameManager.Instance.CurrentStatisticController.PlacedFlags.ToString();

    }


    private void OnEnable()
    {        
        SignalBus.Subscribe<LoadCompletedSignal>(UpdateTexts);
        SignalBus.Subscribe<GameOverSignal>(OpenLoseScreen);
        SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagText);        

        UpdateBombText();
    }

    private void OnDisable()
    {          
        SignalBus.Unsubscribe<LoadCompletedSignal>(UpdateTexts);
        SignalBus.Unsubscribe<GameOverSignal>(OpenLoseScreen);
        SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagText);
    }
}
