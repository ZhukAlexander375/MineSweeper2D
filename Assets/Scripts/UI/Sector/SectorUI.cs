using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SectorUI : MonoBehaviour
{
    [SerializeField] private Canvas _sectorCanvas;
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _containerForHideObjects;    
    [SerializeField] private Button _viewSectorButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _openSectorForAwardButton;
    [SerializeField] private Button _openSectorForAdButton;
    [SerializeField] private Button _replayLevelButton;
    [SerializeField] private TMP_Text _prizeCountText;
    [SerializeField] private TMP_Text _replayLevelText;

    private Sector _sector;

    private void Awake()
    {
        SignalBus.Subscribe<OnGameRewardSignal>(CheckRewardButtonInteractable);        
    }

    private void Start()
    {
        _viewSectorButton.onClick.AddListener(ShowSector);
        _closeButton.onClick.AddListener(HideSector);
        _openSectorForAwardButton.onClick.AddListener(OpenSectorForAward);
        _openSectorForAdButton.onClick.AddListener(OpenSectorForAd);
        _replayLevelButton.onClick.AddListener(ReplayLevel);
        //gameObject.SetActive(false);
        UpdatePrizeCountText();        
    }

    public void SetSector(Sector sector)
    {
        _sector = sector;
    }

    private void ShowSector()
    {
        Color currentColor = _background.color;
        currentColor.a = 0.5f;
        _background.color = currentColor;
        //UpdatePrizeCountText();

        _containerForHideObjects.SetActive(false);
        _closeButton.gameObject.SetActive(true);
    }

    public void HideSector()
    {
        Color currentColor = _background.color;
        currentColor.a = 0.99f;
        _background.color = currentColor;
        UpdatePrizeCountText();

        _closeButton.gameObject.SetActive(false);
        _containerForHideObjects.gameObject.SetActive(true);
    }

    public void CompletedSector()
    {
        Color currentColor = Color.white;
        currentColor.a = 0.25f;
        _background.color = currentColor;

        _closeButton.gameObject.SetActive(false);
        _containerForHideObjects.gameObject.SetActive(false);
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
