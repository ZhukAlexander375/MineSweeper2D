using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SectorUI : MonoBehaviour
{
    [Header("Lose Sector UI")]
    [SerializeField] private Image _loseSectorBackground;
    [SerializeField] private GameObject _containerForHideObjects;    
    [SerializeField] private Button _viewLostSectorButton;
    [SerializeField] private Button _hideLostSectorButton;
    [SerializeField] private Button _openSectorForAwardButton;
    [SerializeField] private Button _openSectorForAdButton;
    [SerializeField] private Button _replayLevelButton;
    [SerializeField] private TMP_Text _prizeCountText;
    [SerializeField] private TMP_Text _replayLevelText;

    [Header("Win Sector UI")]
    [SerializeField] private Image _completeSectorBackground;
    [SerializeField] private Button _viewCompleteSectorButton;
    [SerializeField] private Button _hideCompleteSectorButton;

    private Sector _sector;

    private void Awake()
    {
        SignalBus.Subscribe<OnGameRewardSignal>(CheckRewardButtonInteractable);        
    }

    private void Start()
    {
        _viewLostSectorButton.onClick.AddListener(ShowLostSector);
        _hideLostSectorButton.onClick.AddListener(HideLostSector);
        _openSectorForAwardButton.onClick.AddListener(OpenSectorForAward);
        _openSectorForAdButton.onClick.AddListener(OpenSectorForAd);
        _replayLevelButton.onClick.AddListener(ReplayLevel);

        _viewCompleteSectorButton.onClick.AddListener(ShowCompleteSector);
        _hideCompleteSectorButton.onClick.AddListener(CompletedSector);
        //gameObject.SetActive(false);
        UpdatePrizeCountText();        
    }

    public void SetSector(Sector sector)
    {
        _sector = sector;
    }

    private void ShowLostSector()
    {
        Color currentColor = _loseSectorBackground.color;
        currentColor.a = 0.5f;
        _loseSectorBackground.color = currentColor;
        //UpdatePrizeCountText();

        _containerForHideObjects.SetActive(false);
        _hideLostSectorButton.gameObject.SetActive(true);
    }

    public void HideLostSector()
    {
        Color currentColor = _loseSectorBackground.color;
        currentColor.a = 0.99f;
        _loseSectorBackground.color = currentColor;
        UpdatePrizeCountText();

        _loseSectorBackground.gameObject.SetActive(true);
        _hideLostSectorButton.gameObject.SetActive(false);
        _containerForHideObjects.gameObject.SetActive(true);
    }

    public void CompletedSector()
    {
        Color currentColor = Color.white;
        currentColor.a = 0.20f;
        _loseSectorBackground.color = currentColor;

        _completeSectorBackground.gameObject.SetActive(true);
        _loseSectorBackground.gameObject.SetActive(false);
        _hideCompleteSectorButton.gameObject.SetActive(false);
    }

    private void ShowCompleteSector()
    {
        _completeSectorBackground.gameObject.SetActive(false);
        _hideCompleteSectorButton.gameObject.SetActive(true);        
    }

    private void OpenSectorForAward()
    {
        _sector.OpenSector(_sector.CurrentBuyoutCost);
    }

    private void OpenSectorForAd()
    {
        _sector.OpenSector(0);
    }

    private void ReplayLevel()
    {
        GameManager.Instance.CurrentStatisticController.ResetStatistic();
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
        }
    }

    private void UpdatePrizeCountText()
    {
        _prizeCountText.text = _sector.CurrentBuyoutCost.ToString();
    }

    private void SetReplayButton()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case (GameMode.Hardcore):
            case (GameMode.TimeTrial):
                _replayLevelText.text = $"Replay level <sprite=0> {GameManager.Instance.HardcoreTimeModeCost}";
                break;

            default:
                _replayLevelText.text = $"Replay level";
                break;
        }
    }

    private void CheckRewardButtonInteractable(OnGameRewardSignal signal)
    {
        _openSectorForAwardButton.interactable = PlayerProgress.Instance.CheckAwardCount(_sector.CurrentBuyoutCost);

        SignalBus.Fire(new ThemeChangeSignal(ThemeManager.Instance.CurrentTheme, ThemeManager.Instance.CurrentThemeIndex));
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
        SetReplayButton();

        CheckRewardButtonInteractable(new OnGameRewardSignal());
        CheckReplayLevelButtonInteractable(new OnGameRewardSignal());
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(CheckRewardButtonInteractable);        
    }
}
