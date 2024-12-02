using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _replyLevel;
    [SerializeField] private Button _goHomeButton;

    [Header("Screens")]
    [SerializeField] private Canvas _pauseMenuScreen;
    [SerializeField] private Canvas _settingsScreen;

    [Header("Texts")]
    [SerializeField] private TMP_Text _awardText;

    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = SceneLoader.Instance;
    }

    private void Start()
    {
        _pauseButton.onClick.AddListener(OpenPauseMenu);
        _continueButton.onClick.AddListener(ClosePauseMenu);
        _settingsButton.onClick.AddListener(OpenSettings);
        //_replyLevel.onClick.AddListener(RESTART GAME);
        _goHomeButton.onClick.AddListener(ReturnToMainMenu);
        SignalBus.Subscribe<OnGameRewardSignal>(UpdateUI);
    }

    private void ReturnToMainMenu()
    {
        _sceneLoader.LoadMainMenuScene();
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

    private void UpdateUI(OnGameRewardSignal signal)
    {
        _awardText.text = PlayerProgress.Instance.StarAward.ToString();
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(UpdateUI);
    }
}
