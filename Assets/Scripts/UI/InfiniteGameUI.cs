using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
    [SerializeField] private TMP_Text _awardPopupText;
    [SerializeField] private TMP_Text _flagsTexts;
    [SerializeField] private TMP_Text _gameModeText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _replayLevelText;

    [Header("Objects")]
    [SerializeField] private GameObject _timerObject;

    //[Header("Toggle")]
    //[SerializeField] private Toggle _testToggle;

    private InfiniteGridManager _infiniteGridManager;
    private bool _isShowing;
    private Tween _awardTween;

    private ThemeManager _themeManager;
    private PlayerProgress _playerProgress;
    private GameManager _gameManager;
    private TimeModeTimerManager _timeModeTimerManager;
    private SceneLoader _sceneLoader;

    [Inject]
    private void Construct(ThemeManager themeManager,
        PlayerProgress playerProgress,
        GameManager gameManager,
        TimeModeTimerManager timeModeTimerManager,
        SceneLoader sceneLoader)
    {
        _themeManager = themeManager;
        _playerProgress = playerProgress;
        _gameManager = gameManager;
        _timeModeTimerManager = timeModeTimerManager;
        _sceneLoader = sceneLoader;
    }

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

        _awardPopupText.gameObject.SetActive(false);
        //_testToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void FixedUpdate()
    {
        /*if (_gameManager.CurrentGameMode == GameMode.TimeTrial && _timeModeTimerManager != null)
        {
            
            float remainingTime = _timeModeTimerManager.GetRemainingTime();
            if (remainingTime >= 0)
            {
                Debug.Log($"{FormatTime(remainingTime)}");
                _timerText.text = FormatTime(remainingTime);
            }
            ///
            ///
        }*/
    }

    private void ReturnToMainMenu()
    {
        _gameManager.CurrentStatisticController.StopTimer();
        _playerProgress.SavePlayerProgress();

        if (_infiniteGridManager.IsFirstClick)
        {
            _infiniteGridManager.SaveCurrentGame();
        }

        _sceneLoader.LoadScene(SceneType.MainMenu);
    }

    private void ReplayGame()
    {
        _loseScreen.gameObject.SetActive(false);
        _gameManager.CurrentStatisticController.ResetStatistic();

        if (_gameManager.CurrentStatisticController is TimeTrialStatisticController)
        {
            if (_timeModeTimerManager.IsTimerOver && !_timeModeTimerManager.IsTimerRunning)
            {
                _timeModeTimerManager.ResetModeTimer();
            }
        }

        _gameManager.ResetCurrentModeStatistic();
        _gameManager.ClearCurrentGame(_gameManager.CurrentGameMode);
        _gameManager.SetCurrentGameMode(_gameManager.CurrentGameMode);

        switch (_gameManager.CurrentGameMode)
        {
            case GameMode.Hardcore:
            case GameMode.TimeTrial:
                if (_playerProgress.CheckAwardCount(_gameManager.HardcoreTimeModeCost))
                {
                    SignalBus.Fire(new OnGameRewardSignal(0, -_gameManager.HardcoreTimeModeCost));
                    _sceneLoader.LoadScene(SceneType.InfiniteModeScene);                    
                }
                break;

            case GameMode.SimpleInfinite:
                _sceneLoader.LoadScene(SceneType.InfiniteModeScene);                
                break;
        }
    }

    private void OpenPauseMenu()
    {
        _pauseMenuScreen.gameObject.SetActive(true);
        _gameManager.CurrentStatisticController.StopTimer();
    }

    private void ClosePauseMenu()
    {
        _pauseMenuScreen.gameObject.SetActive(false);
        _gameManager.CurrentStatisticController.StartTimer();
    }

    private void OpenSettings()
    {
        _settingsScreen.gameObject.SetActive(true);
        _gameManager.CurrentStatisticController.StopTimer();
    }

    private void OpenLoseScreen(GameOverSignal signal)
    {
        if (signal.CurrentGameMode == _gameManager.CurrentGameMode)
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
        _awardTween?.Kill();

        string sign = signal.Count >= 0 ? "+" : "-";
        _awardPopupText.text = $"{sign}{Mathf.Abs(signal.Count)}";

        _awardPopupText.color = new Color(_awardPopupText.color.r, _awardPopupText.color.g, _awardPopupText.color.b, 1);
        _awardPopupText.gameObject.SetActive(true);

        Vector3 startPosition = _awardPopupText.transform.position; // �������� �������
        Vector3 targetPosition = _awardText.transform.position; // �������� �������

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(1.5f) // ���� 1.5 �������
            .Append(_awardPopupText.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.InOutQuad)) // �����������
            .Join(_awardPopupText.DOFade(0, 0.5f)) // ������������ ��������� ������������
            .OnComplete(() =>
            {
                _awardText.text = _playerProgress.TotalReward.ToString(); // ��������� ����� �������
                _awardPopupText.transform.position = startPosition; // ���������� �� �����
                _awardPopupText.color = new Color(_awardPopupText.color.r, _awardPopupText.color.g, _awardPopupText.color.b, 0); // ���������� �����
                _awardPopupText.gameObject.SetActive(false); // ������ ������
            });

        _awardTween = sequence; // ��������� ������� �������� � ����������
    }

    private void UpdateFlagText(FlagPlacingSignal signal)
    {
        if (_gameManager.CurrentStatisticController != null)
        {
            _flagsTexts.text = _gameManager.CurrentStatisticController.PlacedFlags.ToString();
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
        switch (_gameManager.CurrentGameMode)
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
        _awardText.text = _playerProgress.TotalReward.ToString();
        _flagsTexts.text = _gameManager.CurrentStatisticController.PlacedFlags.ToString();

        switch (_gameManager.CurrentGameMode)
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
        switch (_gameManager.CurrentGameMode)
        {
            case (GameMode.Hardcore):
            case (GameMode.TimeTrial):
                _replayLevelText.text = $"Replay level \n<sprite=0> {_gameManager.HardcoreTimeModeCost}";
                break;

            default:
                _replayLevelText.text = $"Replay level";
                break;
        }
    }

    private void CheckReplayLevelButtonInteractable(OnGameRewardSignal signal)
    {
        switch (_gameManager.CurrentGameMode)
        {
            case GameMode.SimpleInfinite:
                _replayLevelButton.interactable = true;
                break;

            case GameMode.Hardcore:
            case GameMode.TimeTrial:

                _replayLevelButton.interactable = _playerProgress.CheckAwardCount(_gameManager.HardcoreTimeModeCost);
                break;
        }

        SignalBus.Fire(new ThemeChangeSignal(_themeManager.CurrentTheme, _themeManager.CurrentThemeIndex));
    }

    /*private void OnToggleChanged(bool isOn)
    {
        //Debug.Log(isOn ? "ON" : "OFF");
        SignalBus.Fire(new OnVisibleMinesSignal(isOn));
    }*/

    public void ShowPopup(WrongClickSignal signal)
    {
        if (_isShowing) return; // ���� popup ��� �������, ���������� �����

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

    private void UpdateTimerUI()
    {
        if (_timeModeTimerManager == null) return;

        float remainingTime = _timeModeTimerManager.GetRemainingTime();
        if (remainingTime >= 0)
        {
            _timerText.text = FormatTime(remainingTime);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:D2}:{seconds:D2}";
    }


    private void OnEnable()
    {
        SignalBus.Subscribe<OnGameRewardSignal>(UpdateAwardUI);
        SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagText);
        SignalBus.Subscribe<LoadCompletedSignal>(UpdateTexts);
        SignalBus.Subscribe<GameOverSignal>(OpenLoseScreen);
        SignalBus.Subscribe<WrongClickSignal>(ShowPopup);

        SetReplayButton();
        if (_gameManager.CurrentGameMode == GameMode.TimeTrial)
        {
            InvokeRepeating(nameof(UpdateTimerUI), 0f, 1f);
        }
    }
    private void OnDisable()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(UpdateAwardUI);
        SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagText);
        SignalBus.Unsubscribe<LoadCompletedSignal>(UpdateTexts);
        SignalBus.Unsubscribe<GameOverSignal>(OpenLoseScreen);
        SignalBus.Unsubscribe<OnGameRewardSignal>(CheckReplayLevelButtonInteractable);
        SignalBus.Unsubscribe<WrongClickSignal>(ShowPopup);
        CancelInvoke(nameof(UpdateTimerUI));

        _awardTween?.Kill();
    }

    private void OnApplicationQuit()
    {
        _awardTween?.Kill();
    }
}
