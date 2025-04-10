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
    //[SerializeField] private Button _viewCompleteSectorButton;
    //[SerializeField] private Button _hideCompleteSectorButton;


    private GraphicRaycaster _raycaster;
    private Sector _sector;
    private ThemeManager _themeManager;             //          REPLACE IN SECTOR!!!!!!!!!!!!
    private PlayerProgress _playerProgress;        //          REPLACE IN SECTOR!!!!!!!!!!!!
    private GameManager _gameManager;             //          REPLACE IN SECTOR!!!!!!!!!!!!
    private SceneLoader _sceneLoader;             //          REPLACE IN SECTOR!!!!!!!!!!!!


    private void Awake()
    {
        _raycaster = GetComponent<GraphicRaycaster>();
        SignalBus.Subscribe<OnGameRewardSignal>(CheckRewardButtonInteractable);        
    }

    private void Start()
    {
        _viewLostSectorButton.onClick.AddListener(ShowLostSector);
        _hideLostSectorButton.onClick.AddListener(HideLostSector);
        _openSectorForAwardButton.onClick.AddListener(OpenSectorForAward);
        _openSectorForAdButton.onClick.AddListener(OpenSectorForAd);
        _replayLevelButton.onClick.AddListener(ReplayLevel);

        //_viewCompleteSectorButton.onClick.AddListener(ShowCompleteSector);
        //_hideCompleteSectorButton.onClick.AddListener(CompletedSector);
        //gameObject.SetActive(false);
        UpdatePrizeCountText();        
    }

    public void SectorInit(Sector sector, ThemeManager themeManager, PlayerProgress playerProgress, GameManager gameManager, SceneLoader sceneLoader)
    {
        _sector = sector;
        _themeManager = themeManager;
        _playerProgress = playerProgress;
        _gameManager = gameManager;
        _sceneLoader = sceneLoader;
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

        if (_raycaster.enabled == false)
        {
            _raycaster.enabled = true;
        }        

        UpdatePrizeCountText();

        _loseSectorBackground.gameObject.SetActive(true);
        _hideLostSectorButton.gameObject.SetActive(false);
        _containerForHideObjects.gameObject.SetActive(true);
    }

    public void CompletedSector()
    {
        Color currentColor = _completeSectorBackground.color;
        currentColor.a = 0.15f;
        _completeSectorBackground.color = currentColor;
        _raycaster.enabled = false;

        _completeSectorBackground.gameObject.SetActive(true);
        _loseSectorBackground.gameObject.SetActive(false);
        //_hideCompleteSectorButton.gameObject.SetActive(false);
    }

    /*private void ShowCompleteSector()
    {
        _completeSectorBackground.gameObject.SetActive(false);
        _hideCompleteSectorButton.gameObject.SetActive(true);        
    }*/

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
        _gameManager.CurrentStatisticController.ResetStatistic();
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
        }
    }

    private void UpdatePrizeCountText()
    {
        _prizeCountText.text = _sector.CurrentBuyoutCost.ToString();
    }

    private void SetReplayButton()
    {
        switch (_gameManager.CurrentGameMode)
        {
            case (GameMode.Hardcore):
            case (GameMode.TimeTrial):
                _replayLevelText.text = $"Replay level <sprite=0> {_gameManager.HardcoreTimeModeCost}";
                break;

            default:
                _replayLevelText.text = $"Replay level";
                break;
        }
    }

    private void CheckRewardButtonInteractable(OnGameRewardSignal signal)
    {
        _openSectorForAwardButton.interactable = _playerProgress.CheckAwardCount(_sector.CurrentBuyoutCost);

        SignalBus.Fire(new ThemeChangeSignal(_themeManager.CurrentTheme, _themeManager.CurrentThemeIndex));
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
        SetReplayButton();

        CheckRewardButtonInteractable(new OnGameRewardSignal());
        CheckReplayLevelButtonInteractable(new OnGameRewardSignal());
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(CheckRewardButtonInteractable);        
    }
}
