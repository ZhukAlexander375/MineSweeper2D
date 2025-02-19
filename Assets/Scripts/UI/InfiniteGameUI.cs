using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _replayLevelButton;    
    [SerializeField] private Button _goHomeButton;
    [SerializeField] private Button _backLastClickButton;

    [Header("Screens")]
    [SerializeField] private Canvas _pauseMenuScreen;
    [SerializeField] private Canvas _settingsScreen;
    [SerializeField] private Canvas _loseScreen;
    [SerializeField] private Canvas _wrongClickPopupScreen;

    [Header("Texts")]
    [SerializeField] private TMP_Text _awardText;
    [SerializeField] private TMP_Text _flagsTexts;
    [SerializeField] private TMP_Text _gameModeText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _replayLevelText;

    [Header("Objects")]
    [SerializeField] private GameObject _timerObject;

    //[Header("Toggle")]
    //[SerializeField] private Toggle _testToggle;

    private InfiniteGridManager _infiniteGridManager;
    private Tween _fadeTween;
    private bool _isShowing;    

    private void Awake()
    {       
        _infiniteGridManager = FindObjectOfType<InfiniteGridManager>();

        SignalBus.Subscribe<OnGameRewardSignal>(CheckReplayLevelButtonInteractable);
    }

    private void Start()
    {
        _pauseButton.onClick.AddListener(OpenPauseMenu);
        _continueButton.onClick.AddListener(ClosePauseMenu);
        _settingsButton.onClick.AddListener(OpenSettings);        
        _replayLevelButton.onClick.AddListener(ReplayGame);        
        _goHomeButton.onClick.AddListener(ReturnToMainMenu);
        _backLastClickButton.onClick.AddListener(BackToLastClickButton);

        CheckGameMode();
        UpdateTexts();
        CheckReplayLevelButtonInteractable(new OnGameRewardSignal());

        //_testToggle.onValueChanged.AddListener(OnToggleChanged);
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

        GameManager.Instance.ResetCurrentModeStatistic();
        GameManager.Instance.ClearCurrentGame(GameManager.Instance.CurrentGameMode);
        GameManager.Instance.SetCurrentGameMode(GameManager.Instance.CurrentGameMode);

        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.Hardcore:
            case GameMode.TimeTrial:
                if (PlayerProgress.Instance.CheckAwardCount(GameManager.Instance.HardcoreTimeModeCost))
                {
                    SignalBus.Fire(new OnGameRewardSignal(0, -GameManager.Instance.HardcoreTimeModeCost));
                    SceneLoader.Instance.LoadInfiniteMinesweeperScene();
                }
                break;

            case GameMode.SimpleInfinite:            
                SceneLoader.Instance.LoadInfiniteMinesweeperScene();
                break;
        }        
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

    private void OpenLoseScreen(GameOverSignal signal)
    {
        if (signal.CurrentGameMode == GameManager.Instance.CurrentGameMode)
        {
            if (signal.IsGameOver == true)
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

    private void BackToLastClickButton()
    {
        _infiniteGridManager.ReturnCameraToLastClick();
    }

    private void UpdateAwardUI(OnGameRewardSignal signal)
    {
        _awardText.text = PlayerProgress.Instance.TotalReward.ToString();
    }

    private void UpdateFlagText(FlagPlacingSignal signal)
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

    private void SetReplayButton()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case (GameMode.Hardcore):
            case (GameMode.TimeTrial):
                _replayLevelText.text = $"Replay level \n<sprite=0> {GameManager.Instance.HardcoreTimeModeCost}";
                break;

            default:
                _replayLevelText.text = $"Replay level";
                break;
        }
    }

    private void CheckReplayLevelButtonInteractable(OnGameRewardSignal signal)
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.SimpleInfinite:
                _replayLevelButton.interactable = true;
                break;

            case GameMode.Hardcore:
            case GameMode.TimeTrial:

                _replayLevelButton.interactable = PlayerProgress.Instance.CheckAwardCount(GameManager.Instance.HardcoreTimeModeCost);
                break;
        }

        SignalBus.Fire(new ThemeChangeSignal(ThemeManager.Instance.CurrentTheme, ThemeManager.Instance.CurrentThemeIndex));
    }

    /*private void OnToggleChanged(bool isOn)
    {
        //Debug.Log(isOn ? "ON" : "OFF");
        SignalBus.Fire(new OnVisibleMinesSignal(isOn));
    }*/

    public void ShowPopup(WrongÑlickSignal signal)
    {
        if (_isShowing) return; // Åñëè popup óæå àêòèâåí, èãíîðèðóåì âûçîâ

        _isShowing = true;
        _wrongClickPopupScreen.gameObject.SetActive(true);
        StartCoroutine(HidePopupAfterDelay(3f));
    }

    private IEnumerator HidePopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

       _wrongClickPopupScreen.gameObject.SetActive(false);
       _isShowing = false;
    }

    private void OnEnable()
    {
        SignalBus.Subscribe<OnGameRewardSignal>(UpdateAwardUI);
        SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagText);
        SignalBus.Subscribe<LoadCompletedSignal>(UpdateTexts);
        SignalBus.Subscribe<GameOverSignal>(OpenLoseScreen);
        SignalBus.Subscribe<WrongÑlickSignal>(ShowPopup);

        SetReplayButton();
        
    }
    private void OnDisable()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(UpdateAwardUI);
        SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagText);
        SignalBus.Unsubscribe<LoadCompletedSignal>(UpdateTexts);
        SignalBus.Unsubscribe<GameOverSignal>(OpenLoseScreen);
        SignalBus.Unsubscribe<OnGameRewardSignal>(CheckReplayLevelButtonInteractable);
        SignalBus.Unsubscribe<WrongÑlickSignal>(ShowPopup);
    }

}
