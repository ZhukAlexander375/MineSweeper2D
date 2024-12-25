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
    [SerializeField] private TMP_Text _scoresText;
    [SerializeField] private TMP_Text _sectorsText;
    [SerializeField] private TMP_Text _tilesText;
    [SerializeField] private TMP_Text _flagsText;
    [SerializeField] private TMP_Text _minesText;

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
        _scoresText.text = PlayerProgress.Instance.TotalReward.ToString();
        _sectorsText.text = GameManager.Instance.CurrentStatisticController.CompletedSectors.ToString();
        _tilesText.text = GameManager.Instance.CurrentStatisticController.OpenedCells.ToString();
        _flagsText.text = GameManager.Instance.CurrentStatisticController.PlacedFlags.ToString();
        _minesText.text = GameManager.Instance.CurrentStatisticController.ExplodedMines.ToString();
    }

    private void SetTitleText()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case (GameMode.Hardcore):                
                _titleText.text = "Remember: one mistake.\nTry again!";
                
                break;

            case (GameMode.TimeTrial):
                _titleText.text = "Time is up.\nTry again!";
                break;
        }
    }

    private void OnEnable()
    {
        UpdateStatisticTexts();
    }
}
