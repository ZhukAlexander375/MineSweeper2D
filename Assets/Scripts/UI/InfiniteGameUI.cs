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
        SignalBus.Subscribe<OnGameRewardSignal>(UpdateAwardUI);
        SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagUI);

        UpdateTexts();
    }

    private void ReturnToMainMenu()
    {
        _infiniteGridManager.SaveCurrentGame();
        PlayerProgress.Instance.SavePlayerProgress();

        if (!_infiniteGridManager.IsFirstClick)
        {
            GameModesManager.Instance.IsDownloadedInfiniteGame = false;
            GameModesManager.Instance.IsNewInfiniteGame = true;
        }

        else
        {
            GameModesManager.Instance.IsDownloadedInfiniteGame = true;
            GameModesManager.Instance.IsNewInfiniteGame = false;
        }
        
        GameModesManager.Instance.SaveGameModes();
        //Debug.Log($"Return:    IsDownloadedInfiniteGame: {GameModesManager.Instance.IsDownloadedInfiniteGame}, IsNewInfiniteGame: {GameModesManager.Instance.IsNewInfiniteGame}");

        SceneLoader.Instance.LoadMainMenuScene();
    }

    private void ReplayGame()
    {        
        PlayerProgress.Instance.ResetSessionStatistic();        
        GameModesManager.Instance.IsDownloadedInfiniteGame = false;
        GameModesManager.Instance.IsNewInfiniteGame = true;
        GameModesManager.Instance.SaveGameModes();

        //Debug.Log($"Replay:    IsDownloadedInfiniteGame: {GameModesManager.Instance.IsDownloadedInfiniteGame}, IsNewInfiniteGame: {GameModesManager.Instance.IsNewInfiniteGame}");

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
        _awardText.text = PlayerProgress.Instance.AwardCount.ToString();
    }

    private void UpdateFlagUI(FlagPlacingSignal signal)
    {
        _flagsTexts.text = PlayerProgress.Instance.PlacedFlags.ToString();
    }

    private void UpdateTexts()
    {
        _awardText.text = PlayerProgress.Instance.AwardCount.ToString();
        _flagsTexts.text = PlayerProgress.Instance.PlacedFlags.ToString();
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(UpdateAwardUI);
        SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagUI);
    }
}
