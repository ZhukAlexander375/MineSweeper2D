using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            case GameMode.SimpleInfinite:
                SceneLoader.Instance.LoadInfiniteMinesweeperScene();
                break;

            case GameMode.Hardcore:
            case GameMode.TimeTrial:
                if (PlayerProgress.Instance.CheckAwardCount(GameManager.Instance.HardcoreTimeModeCost))
                {
                    SignalBus.Fire(new OnGameRewardSignal(0, -GameManager.Instance.HardcoreTimeModeCost));
                    SceneLoader.Instance.LoadInfiniteMinesweeperScene();
                }
                break;

            case GameMode.ClassicEasy:
            case GameMode.ClassicMedium:
            case GameMode.ClassicHard:
            case GameMode.Custom:
                SceneLoader.Instance.LoadClassicMinesweeperScene();
                break;
        }        
    }

    private void ReturnToMainMenu()
    {
        GameManager.Instance.CurrentStatisticController.StopTimer();
        PlayerProgress.Instance.SavePlayerProgress();

        switch (GameManager.Instance.CurrentGameMode)
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

        SceneLoader.Instance.LoadMainMenuScene();
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
        switch (GameManager.Instance.CurrentGameMode)
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
                if (GameManager.Instance.CurrentStatisticController.IsGameOver)
                {
                    _titleImage.sprite = _loseSprite;
                    _titleText.text = "Try again";
                }
                else if (GameManager.Instance.CurrentStatisticController.IsGameWin)
                {
                    _titleImage.sprite = _winSprite;
                    _titleText.text = "You win";
                }
                break;
        }
    }


    private void SetModeNameText()
    {
        switch (GameManager.Instance.CurrentGameMode)
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
        switch (GameManager.Instance.CurrentGameMode)
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
        switch (GameManager.Instance.CurrentGameMode)
        {
            case (GameMode.Hardcore):
                _resultValueText1Text.text = FormatTime(GameManager.Instance.CurrentStatisticController.TotalPlayTime);
                _resultValueText2Text.text = GameManager.Instance.CurrentStatisticController.OpenedCells.ToString();
                _resultValueText3Text.text = GameManager.Instance.CurrentStatisticController.PlacedFlags.ToString();
                _resultValueText4Text.text = GameManager.Instance.CurrentStatisticController.CompletedSectors.ToString();
                break;

            case (GameMode.TimeTrial):
                _resultValueText1Text.text = GameManager.Instance.CurrentStatisticController.OpenedCells.ToString();
                _resultValueText2Text.text = GameManager.Instance.CurrentStatisticController.PlacedFlags.ToString();
                _resultValueText3Text.text = GameManager.Instance.CurrentStatisticController.CompletedSectors.ToString();
                _resultValueText4Text.text = GameManager.Instance.CurrentStatisticController.ExplodedMines.ToString();
                break;

            case (GameMode.ClassicEasy):
            case (GameMode.ClassicMedium):
            case (GameMode.ClassicHard):
            case (GameMode.Custom):
                _resultValueText1Text.text = FormatTime(GameManager.Instance.CurrentStatisticController.TotalPlayTime);
                _resultValueText2Text.text = GameManager.Instance.CurrentStatisticController.OpenedCells.ToString();
                _resultValueText3Text.text = GameManager.Instance.CurrentStatisticController.PlacedFlags.ToString();
                _resultValueText4Text.text = GameManager.Instance.CurrentStatisticController.ExplodedMines.ToString();
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
            case (GameMode.Hardcore):
            case (GameMode.TimeTrial):
                _replayLevelButton.interactable = PlayerProgress.Instance.CheckAwardCount(GameManager.Instance.HardcoreTimeModeCost);
                break;

            default:
                _replayLevelButton.interactable = true;
                break;
        }

        SignalBus.Fire(new ThemeChangeSignal(ThemeManager.Instance.CurrentTheme, ThemeManager.Instance.CurrentThemeIndex));
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
