using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoseScreenUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _replayHardcoreLevelButton;
    [SerializeField] private Button _goHomeButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _fieldOverviewButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _resultText1Text;
    [SerializeField] private TMP_Text _resultText2Text;
    [SerializeField] private TMP_Text _resultText3Text;
    [SerializeField] private TMP_Text _resultText4Text;
    [SerializeField] private TMP_Text _resultValueText1Text;
    [SerializeField] private TMP_Text _resultValueText2Text;
    [SerializeField] private TMP_Text _resultValueText3Text;
    [SerializeField] private TMP_Text _resultValueText4Text;

    [Header("Objects")]
    [SerializeField] private GameObject _loseMenuObject;

    private InfiniteGridManager _infiniteGridManager;

    private void Awake()
    {
        _infiniteGridManager = FindObjectOfType<InfiniteGridManager>();
    }


    private void Start()
    {
        _replayHardcoreLevelButton.onClick.AddListener(ReplayGame);
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

        GameManager.Instance.ClearCurrentGame(GameManager.Instance.CurrentGameMode);
        GameManager.Instance.SetCurrentGameMode(GameManager.Instance.CurrentGameMode);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
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
        SetTitleText();
        SetResultTexts();
        SetResultValueTexts();
    }

    private void SetTitleText()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case (GameMode.Hardcore):                
                _titleText.text = "Hardcore";                
                break;

            case (GameMode.TimeTrial):
                _titleText.text = "Time";
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

    private void OnEnable()
    {
        UpdateStatisticTexts();
    }
}
