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

    //[SerializeField] private Button _easyLvlButton;
    //[SerializeField] private Button _mediumLvlButton;
    //[SerializeField] private Button _hardLvlButton;
    //[SerializeField] private Button _customLvlButton;
    [SerializeField] private Button _startCustomGameButton;

    //[SerializeField] private Button _backToMainMenuButton;
    //[SerializeField] private Button _backToMenuButton;   


    [Header("InputFields")]
    [SerializeField] private InputFieldHandler _inputWidth;
    [SerializeField] private InputFieldHandler _inputHeight;
    [SerializeField] private InputFieldHandler _inputMines;

    [Header("Texts")]
    [SerializeField] private TMP_Text _gameModeName;

    [Header("Screens")]
    [SerializeField] private Canvas _pauseMenuScreen;
    [SerializeField] private Canvas _settingsScreen;
    [SerializeField] private Canvas _loseScreen;    
    [SerializeField] private Canvas _gameScreen;
    [SerializeField] private Canvas _customGameSettingsScreen;

    [Header("GridManager")]
    [SerializeField] private SimpleGridManager _gridManager;
    [SerializeField] private GameObject _sampleGrid;

    [Header("Settings For Custom Game")]
    [SerializeField] private int _minSize = 2;
    [SerializeField] private int _maxSize = 100;
    [SerializeField] private int _minMines = 1;
    [SerializeField] private int _maxMines = 5000;
    [SerializeField] private ClassicGameController _classicGameController;

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

        //_easyLvlButton.onClick.AddListener(() => StartLevel("Easy", 0));
        //_mediumLvlButton.onClick.AddListener(() => StartLevel("Medium", 1));
        //_hardLvlButton.onClick.AddListener(() => StartLevel("Hard", 2));
        //_customLvlButton.onClick.AddListener(OpenCustomGameSettingsScreen);
        _startCustomGameButton.onClick.AddListener(StartCustomLevel);

        SetModeName();
    }

    private void OpenPauseMenu()
    {
        _pauseMenuScreen.gameObject.SetActive(true);
    }

    private void ClosePauseMenu()
    {
        _pauseMenuScreen.gameObject.SetActive(false);
    }

    private void OpenSettings()
    {
        _settingsScreen.gameObject.SetActive(true);
    }

    private void ReplayGame()
    {
        _loseScreen.gameObject.SetActive(false);        

        // GameManager.Instance.ClearCurrentGame(GameManager.Instance.CurrentGameMode);
        GameManager.Instance.SetCurrentGameMode(GameManager.Instance.CurrentGameMode);
        GameManager.Instance.ResetCurrentModeStatistic();
        SceneLoader.Instance.LoadClassicMinesweeperScene();
    }

    private void StartLevel(string modeName, int levelIndex)
    {
        OpenGameScreen(modeName);
        _gridManager.gameObject.SetActive(true);
        _classicGameController.StartPresetLevel(levelIndex);
    }

    private void OpenCustomGameSettingsScreen()
    {        
        _customGameSettingsScreen.gameObject.SetActive(true);

    }

    private void StartCustomLevel()
    {
        OpenGameScreen("Custom");
        _gridManager.gameObject.SetActive(true);

        int width = ClassicGameMinSize.ClampValue(ParseInput(_inputWidth.InputField.text, ClassicGameMinSize.MinWidth), ClassicGameMinSize.MinWidth, ClassicGameMinSize.MaxWidth);
        int height = ClassicGameMinSize.ClampValue(ParseInput(_inputHeight.InputField.text, ClassicGameMinSize.MinHeight), ClassicGameMinSize.MinHeight, ClassicGameMinSize.MaxHeight);
        int maxMines = ClassicGameMinSize.MaxMines(width, height);
        int mines = ClassicGameMinSize.ClampValue(
                    ParseInput(_inputMines.InputField.text, ClassicGameMinSize.MinMines(width, height)),
                    ClassicGameMinSize.MinMines(width, height),
                    maxMines
                    );

        var customSettings = new ClassicGameSettings
        {
            Width = width,
            Height = height,
            Mines = mines
        };
        ClearInputFields();
        _classicGameController.StartCustomLevel(customSettings);

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
        }
    }

    private void OpenGameScreen(string gameModeName)
    {        
        _customGameSettingsScreen.gameObject.SetActive(false);
        _gameScreen.gameObject.SetActive(true);
        _gameModeName.text = gameModeName;
    }

    private void ClearInputFields()
    {
        _inputWidth.ClearInputField();
        _inputHeight.ClearInputField();
        _inputMines.ClearInputField();
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
            _loseScreen.gameObject.SetActive(true);
        }
    }

    private void ReturnToClassicGameMenu()
    {
        ClearInputFields();
        _pauseMenuScreen.gameObject.SetActive(false);       
        _customGameSettingsScreen.gameObject.SetActive(false);
        _gameScreen.gameObject.SetActive(false);
        _gridManager.gameObject.SetActive(false);
    }

    private int ParseInput(string input, int defaultValue)
    {
        if (int.TryParse(input, out int parsedValue))
        {
            return parsedValue;
        }

        return defaultValue;
    }

    private void OnEnable()
    {
        //SignalBus.Subscribe<OnGameRewardSignal>(UpdateAwardUI);
        //SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagUI);
        //SignalBus.Subscribe<LoadCompletedSignal>(UpdateTexts);
        SignalBus.Subscribe<GameOverSignal>(OpenLoseScreen);
    }

    private void OnDisable()
    {
        //SignalBus.Unsubscribe<OnGameRewardSignal>(UpdateAwardUI);
        //SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagUI);
        //SignalBus.Unsubscribe<LoadCompletedSignal>(UpdateTexts);
        SignalBus.Unsubscribe<GameOverSignal>(OpenLoseScreen);
    }
}
