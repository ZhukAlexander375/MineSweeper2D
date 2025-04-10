using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WinLoseScreenUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _replayLevelButton;
    [SerializeField] private Button _goHomeButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _fieldOverviewButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _modeNameText;
    [SerializeField] private TMP_Text _resultText1Text;
    [SerializeField] private TMP_Text _resultText2Text;
    [SerializeField] private TMP_Text _resultText3Text;
    [SerializeField] private TMP_Text _resultText4Text;
    [SerializeField] private TMP_Text _resultValueText1Text;
    [SerializeField] private TMP_Text _resultValueText2Text;
    [SerializeField] private TMP_Text _resultValueText3Text;
    [SerializeField] private TMP_Text _resultValueText4Text;
    [SerializeField] private TMP_Text _replayLevelText;

    [Header("Objects")]
    [SerializeField] private GameObject _loseMenuObject;

    [Header("Images, Sprites")]
    [SerializeField] private Image _titleImage;
    [SerializeField] private Sprite _resultSprite;
    [SerializeField] private Sprite _loseSprite;
    [SerializeField] private Sprite _winSprite;

    private InfiniteGridManager _infiniteGridManager;
    private SimpleGridManager _simpleGridManager;

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
        _simpleGridManager = FindObjectOfType<SimpleGridManager>();
        SignalBus.Subscribe<OnGameRewardSignal>(CheckReplayLevelButtonInteractable);
    }


    private void Start()
    {
        _replayLevelButton.onClick.AddListener(ReplayGame);
        _goHomeButton.onClick.AddListener(ReturnToMainMenu);
        _fieldOverviewButton.onClick.AddListener(HideLoseMenu);
        _backButton.onClick.AddListener(ShowLoseMenu);        
    }

    private void ReplayGame()
    {
        gameObject.SetActive(false);
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
            case GameMode.SimpleInfinite:
                _sceneLoader.LoadScene(SceneType.InfiniteModeScene);
                break;

            case GameMode.Hardcore:
            case GameMode.TimeTrial:
                if (_playerProgress.CheckAwardCount(_gameManager.HardcoreTimeModeCost))
                {
                    SignalBus.Fire(new OnGameRewardSignal(0, -_gameManager.HardcoreTimeModeCost));
                    _sceneLoader.LoadScene(SceneType.InfiniteModeScene);
                }
                break;

            case GameMode.ClassicEasy:
            case GameMode.ClassicMedium:
            case GameMode.ClassicHard:
            case GameMode.Custom:
                _sceneLoader.LoadScene(SceneType.ClassicModeScene);
                break;
        }        
    }

    private void ReturnToMainMenu()
    {
        _gameManager.CurrentStatisticController.StopTimer();
        _playerProgress.SavePlayerProgress();

        switch (_gameManager.CurrentGameMode)
        {
            case GameMode.Hardcore:
            case GameMode.TimeTrial:
                if (_infiniteGridManager != null && _infiniteGridManager.IsFirstClick)
                {
                    _infiniteGridManager.SaveCurrentGame();
                }
                break;

            case GameMode.ClassicEasy:
            case GameMode.ClassicMedium:
            case GameMode.ClassicHard:
            case GameMode.Custom:
                if (_simpleGridManager != null && _simpleGridManager.IsFirstClick)
                {
                    _simpleGridManager.SaveCurrentGame();
                }
                break;
        }

        _sceneLoader.LoadScene(SceneType.MainMenu);
    }

    private void HideLoseMenu()
    {
        _loseMenuObject.SetActive(false);
        _backButton.gameObject.SetActive(true);
    }

    private void ShowLoseMenu()
    {
        _loseMenuObject.SetActive(true);
    }

    private void UpdateStatisticTexts()
    {
        SetTitleIconAndText();
        SetModeNameText();
        SetResultTexts();
        SetResultValueTexts();        
    }

    private void SetTitleIconAndText()
    {
        switch (_gameManager.CurrentGameMode)
        {
            case GameMode.Hardcore:
            case GameMode.TimeTrial:
                _titleImage.sprite = _resultSprite;
                _titleText.text = "Results";
                break;

            case GameMode.ClassicEasy:
            case GameMode.ClassicMedium:
            case GameMode.ClassicHard:
            case GameMode.Custom:
                if (_gameManager.CurrentStatisticController.IsGameOver)
                {
                    _titleImage.sprite = _loseSprite;
                    _titleText.text = "Try again";
                }
                else if (_gameManager.CurrentStatisticController.IsGameWin)
                {
                    _titleImage.sprite = _winSprite;
                    _titleText.text = "You win";
                }
                break;
        }
    }


    private void SetModeNameText()
    {
        switch (_gameManager.CurrentGameMode)
        {
            case (GameMode.Hardcore):
                _modeNameText.text = "Hardcore";
                break;

            case (GameMode.TimeTrial):
                _modeNameText.text = "Time";
                break;

            case (GameMode.ClassicEasy):
                _modeNameText.text = "Easy";
                break;

            case (GameMode.ClassicMedium):
                _modeNameText.text = "Medium";
                break;

            case (GameMode.ClassicHard):
                _modeNameText.text = "Hard";
                break;

            case (GameMode.Custom):
                _modeNameText.text = "Custom";
                break;
        }
    }

    private void SetResultTexts()
    {
        switch (_gameManager.CurrentGameMode)
        {
            case (GameMode.Hardcore):
                _resultText1Text.text = "Time in mode:";
                _resultText2Text.text = "Opened cells:";
                _resultText3Text.text = "Checkboxes placed:";
                _resultText4Text.text = "Completed sectors:";
                break;

            case (GameMode.TimeTrial):
                _resultText1Text.text = "Open cells: ";
                _resultText2Text.text = "Checkboxes placed:";
                _resultText3Text.text = "Completed sectors:";
                _resultText4Text.text = "Triggered mines:";
                break;

            case (GameMode.ClassicEasy):
            case (GameMode.ClassicMedium):
            case (GameMode.ClassicHard):
            case (GameMode.Custom):
                _resultText1Text.text = "Time in mode:";
                _resultText2Text.text = "Opened cells:";
                _resultText3Text.text = "Checkboxes placed:";
                _resultText4Text.text = "Triggered mines:";
                break;
        }    
    }

    private void SetResultValueTexts()
    {
        switch (_gameManager.CurrentGameMode)
        {
            case (GameMode.Hardcore):
                _resultValueText1Text.text = FormatTime(_gameManager.CurrentStatisticController.TotalPlayTime);
                _resultValueText2Text.text = _gameManager.CurrentStatisticController.OpenedCells.ToString();
                _resultValueText3Text.text = _gameManager.CurrentStatisticController.PlacedFlags.ToString();
                _resultValueText4Text.text = _gameManager.CurrentStatisticController.CompletedSectors.ToString();
                break;

            case (GameMode.TimeTrial):
                _resultValueText1Text.text = _gameManager.CurrentStatisticController.OpenedCells.ToString();
                _resultValueText2Text.text = _gameManager.CurrentStatisticController.PlacedFlags.ToString();
                _resultValueText3Text.text = _gameManager.CurrentStatisticController.CompletedSectors.ToString();
                _resultValueText4Text.text = _gameManager.CurrentStatisticController.ExplodedMines.ToString();
                break;

            case (GameMode.ClassicEasy):
            case (GameMode.ClassicMedium):
            case (GameMode.ClassicHard):
            case (GameMode.Custom):
                _resultValueText1Text.text = FormatTime(_gameManager.CurrentStatisticController.TotalPlayTime);
                _resultValueText2Text.text = _gameManager.CurrentStatisticController.OpenedCells.ToString();
                _resultValueText3Text.text = _gameManager.CurrentStatisticController.PlacedFlags.ToString();
                _resultValueText4Text.text = _gameManager.CurrentStatisticController.ExplodedMines.ToString();
                break;
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
            case (GameMode.Hardcore):
            case (GameMode.TimeTrial):
                _replayLevelButton.interactable = _playerProgress.CheckAwardCount(_gameManager.HardcoreTimeModeCost);
                break;

            default:
                _replayLevelButton.interactable = true;
                break;
        }

        SignalBus.Fire(new ThemeChangeSignal(_themeManager.CurrentTheme, _themeManager.CurrentThemeIndex));
    }

    private void OnEnable()
    {
        UpdateStatisticTexts();
        SetReplayButton();

        CheckReplayLevelButtonInteractable(new OnGameRewardSignal());
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(CheckReplayLevelButtonInteractable);
    }
}
