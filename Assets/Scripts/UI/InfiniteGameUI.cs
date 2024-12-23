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

    [Header("Screens")]
    [SerializeField] private Canvas _pauseMenuScreen;
    [SerializeField] private Canvas _settingsScreen;

    [Header("Texts")]
    [SerializeField] private TMP_Text _awardText;
    [SerializeField] private TMP_Text _flagsTexts;
    [SerializeField] private TMP_Text _gameModeText;
        
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
        _replayLevelButton.onClick.AddListener(ReplayGame);
        _goHomeButton.onClick.AddListener(ReturnToMainMenu);        

        UpdateTexts();
    }

    private void ReturnToMainMenu()
    {
        _infiniteGridManager.SaveCurrentGame();
        PlayerProgress.Instance.SavePlayerProgress();

        /*if (!_infiniteGridManager.IsFirstClick)
        {
            GameManager.Instance.IsDownloadedInfiniteGame = false;
            GameManager.Instance.IsNewInfiniteGame = true;
        }

        else
        {
            GameManager.Instance.IsDownloadedInfiniteGame = true;
            GameManager.Instance.IsNewInfiniteGame = false;
        }
        
        GameManager.Instance.SaveGameModes();*/
        //Debug.Log($"Return:    IsDownloadedInfiniteGame: {GameModesManager.Instance.IsDownloadedInfiniteGame}, IsNewInfiniteGame: {GameModesManager.Instance.IsNewInfiniteGame}");

        SceneLoader.Instance.LoadMainMenuScene();
    }

    private void ReplayGame()
    {
        /*PlayerProgress.Instance.ResetSessionStatistic();        
        GameManager.Instance.IsDownloadedInfiniteGame = false;
        GameManager.Instance.IsNewInfiniteGame = true;
        GameManager.Instance.SaveGameModes();*/

        //Debug.Log($"Replay:    IsDownloadedInfiniteGame: {GameModesManager.Instance.IsDownloadedInfiniteGame}, IsNewInfiniteGame: {GameModesManager.Instance.IsNewInfiniteGame}");
        GameManager.Instance.ClearCurrentGame(GameManager.Instance.CurrentGameMode);
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
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
    }

    private void OnDisable()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(UpdateAwardUI);
        SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagUI);
        SignalBus.Unsubscribe<LoadCompletedSignal>(UpdateTexts);
    }
}
