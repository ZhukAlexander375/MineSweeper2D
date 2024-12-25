using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _closeSettingsButton;
    [SerializeField] private Button _replayLevelButton;    
    [SerializeField] private Button _goHomeButton;

    [Header("Screens")]
    [SerializeField] private Canvas _pauseMenuScreen;
    [SerializeField] private Canvas _settingsScreen;
    [SerializeField] private Canvas _loseScreen;

    [Header("Texts")]
    [SerializeField] private TMP_Text _awardText;
    [SerializeField] private TMP_Text _flagsTexts;
    [SerializeField] private TMP_Text _gameModeText;
    [SerializeField] private TMP_Text _timerText;

    [Header("Objects")]
    [SerializeField] private GameObject _timerObject;
        
    private InfiniteGridManager _infiniteGridManager;

    private void Awake()
    {       
        _infiniteGridManager = FindObjectOfType<InfiniteGridManager>();
    }

    private void Start()
    {
        _pauseButton.onClick.AddListener(OpenPauseMenu);
        _continueButton.onClick.AddListener(ClosePauseMenu);
        _settingsButton.onClick.AddListener(OpenSettings);
        _closeSettingsButton.onClick.AddListener(CloseSettings);
        _replayLevelButton.onClick.AddListener(ReplayGame);        
        _goHomeButton.onClick.AddListener(ReturnToMainMenu);

        CheckGameMode();
        UpdateTexts();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameMode == GameMode.TimeTrial && TimeModeTimerManager.Instance != null)
        {
            float remainingTime = TimeModeTimerManager.Instance.GetRemainingTime();
            if (remainingTime >= 0)
            {
                _timerText.text = FormatTime(remainingTime);
            }
            ///
            ///
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:D2}:{seconds:D2}";
    }

    private void ReturnToMainMenu()
    {
        GameManager.Instance.CurrentStatisticController.StopTimer();        
        PlayerProgress.Instance.SavePlayerProgress();
        
        if (_infiniteGridManager.IsFirstClick)
        {
            _infiniteGridManager.SaveCurrentGame();
        }

        SceneLoader.Instance.LoadMainMenuScene();
    }

    private void ReplayGame()
    {
        _loseScreen.gameObject.SetActive(false);       
        GameManager.Instance.CurrentStatisticController.ResetStatistic();

        if (GameManager.Instance.CurrentStatisticController is TimeTrialStatisticController)
        {
            if (TimeModeTimerManager.Instance.IsTimerOver && !TimeModeTimerManager.Instance.IsTimerRunning)
            {
                TimeModeTimerManager.Instance.ResetModeTimer();
            }            
        }

        GameManager.Instance.ClearCurrentGame(GameManager.Instance.CurrentGameMode);
        GameManager.Instance.SetCurrentGameMode(GameManager.Instance.CurrentGameMode);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
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

    private void CloseSettings()
    {
        _settingsScreen.gameObject.SetActive(false);
        GameManager.Instance.CurrentStatisticController.StartTimer();
    }

    private void OpenLoseScreen(GameOverSignal signal)
    {
        if (signal.CurrentGameMode == GameManager.Instance.CurrentGameMode)
        {
            _loseScreen.gameObject.SetActive(true);
        }        
    }

    private void UpdateAwardUI(OnGameRewardSignal signal)
    {
        _awardText.text = PlayerProgress.Instance.TotalReward.ToString();
    }

    private void UpdateFlagUI(FlagPlacingSignal signal)
    {
        if (GameManager.Instance.CurrentStatisticController != null)
        {
            _flagsTexts.text = GameManager.Instance.CurrentStatisticController.PlacedFlags.ToString();
        }
        else
        {
            _flagsTexts.text = "0";
        }
    }

    private void UpdateTexts(LoadCompletedSignal signal)
    {
        UpdateTexts();
    }

    private void CheckGameMode()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case (GameMode.TimeTrial):
                _timerObject.SetActive(true);
                break;

            default:
                _timerObject.SetActive(false);
                break;
        }
    }

    private void UpdateTexts()
    {
        _awardText.text = PlayerProgress.Instance.TotalReward.ToString();
        _flagsTexts.text = GameManager.Instance.CurrentStatisticController.PlacedFlags.ToString();

        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.SimpleInfinite:
                _gameModeText.text = "Infinity";
                break;
            case GameMode.Hardcore:
                _gameModeText.text = "Hardcore";
                break;
            case GameMode.TimeTrial:
                _gameModeText.text = "Time";
                break;
        }
    }

    private void OnEnable()
    {
        SignalBus.Subscribe<OnGameRewardSignal>(UpdateAwardUI);
        SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagUI);
        SignalBus.Subscribe<LoadCompletedSignal>(UpdateTexts);
        SignalBus.Subscribe<GameOverSignal>(OpenLoseScreen);        
    }

    private void OnDisable()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(UpdateAwardUI);
        SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagUI);
        SignalBus.Unsubscribe<LoadCompletedSignal>(UpdateTexts);
        SignalBus.Unsubscribe<GameOverSignal>(OpenLoseScreen);
    }
}
