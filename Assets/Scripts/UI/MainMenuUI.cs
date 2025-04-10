using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Vector3 = UnityEngine.Vector3;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons for infinity modes")]
    [SerializeField] private Button _chooseInfinitymodeButton;
    [SerializeField] private Button _infinityNewGameButton;
    [SerializeField] private Button _infinityContinuedGameButton;
    [SerializeField] private Button _hardcoreNewGameMenuButton;
    [SerializeField] private Button _hardcoreContinuedGameMenuButton;
    [SerializeField] private Button _timeTrialNewGameButton;
    [SerializeField] private Button _timeTrialContinuedGameButton;

    [Header("Buttons for classic modes")]
    [SerializeField] private Button _newClassicGameButton;
    [SerializeField] private Button _easyLvlButton;
    [SerializeField] private Button _mediumLvlButton;
    [SerializeField] private Button _hardLvlButton;
    [SerializeField] private Button _customLvlButton;
    [SerializeField] private Button _continueClassicGameButton;
    //[SerializeField] private Button _classicGameMenuButton;
    //[SerializeField] private Button _episodeGameMenuButton;

    [Header("Buttons")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _bonusButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text[] _awardTexts;
    [SerializeField] private TMP_Text _hardcoreNewGameText;
    [SerializeField] private TMP_Text _timeTrialNewGameText;

    [Header("Screens")]
    [SerializeField] private Canvas[] _mainMenuScreens;
    [SerializeField] private Canvas _settingsScreen;
    [SerializeField] private Canvas _infinityModeMenuScreen;
    [SerializeField] private Canvas _classicModeMenuScreen;
    [SerializeField] private Canvas _customGameSettingsScreen;
    [SerializeField] private Canvas _bonusScreen;

    [Header("Home Mode Screen Texts")]
    [SerializeField] private TMP_Text _lastModeText;
    [SerializeField] private TMP_Text _timeSpentText;
    [SerializeField] private TMP_Text _cellsOpenText;
    [SerializeField] private TMP_Text _flagsPlacedText;

    [Header("Ñlassic Modes Menu Screen Texts")]
    //[SerializeField] private TMP_Text _lastClassicSessionText;
    [SerializeField] private TMP_Text _difficultyLevelText;   
    [SerializeField] private TMP_Text _classicSessionCellsOpenText;
    [SerializeField] private TMP_Text _classicSessionFlagsPlacedText;
    [SerializeField] private TMP_Text _classicSessionTimeSpentText;

    [Header("Objects")]
    [SerializeField] private GameObject _containerLastSession;
    [SerializeField] private GameObject _startTutorialObject;

    private Tween _bonusButtonTween;
    private Tween _bonusButtonRotationTween;

    private RewardManager _rewardManager;
    private ThemeManager _themeManager;
    private SaveManager _saveManager;
    private PlayerProgress _playerProgress;
    private GameManager _gameManager;
    private SceneLoader _sceneLoader;

    [Inject]
    private void Construct(
        RewardManager rewardManager, 
        ThemeManager themeManager, 
        SaveManager saveManager, 
        PlayerProgress playerProgress, 
        GameManager gameManager,
        SceneLoader sceneLoader)
    {
        _rewardManager = rewardManager;
        _themeManager = themeManager;
        _saveManager = saveManager;
        _playerProgress = playerProgress;
        _gameManager = gameManager;
        _sceneLoader = sceneLoader;
    }

    private void Awake()
    {
        //SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastSessionStatistic);
        //SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastClassicSessionStatistic);
        //SignalBus.Subscribe<PlayerProgressLoadCompletedSignal>(PlayButtonInteractable);

        //SignalBus.Subscribe<OnGameRewardSignal>(UpdateAwardText);
        //SignalBus.Subscribe<OnGameRewardSignal>(CheckNewGameButtonsInteractable);    
        //_sceneLoader = SceneLoader.Instance;

        SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastSessionStatistic);
        SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastClassicSessionStatistic);
    }

    private void Start()
    {
        _infinityNewGameButton.onClick.AddListener(NewInfinityGame);
        _infinityContinuedGameButton.onClick.AddListener(ContinuedInfinityGame);
        _hardcoreNewGameMenuButton.onClick.AddListener(NewHardcoreGame);
        _hardcoreContinuedGameMenuButton.onClick.AddListener(ContinuedHardcoreGame);
        _timeTrialNewGameButton.onClick.AddListener(NewTimeTrialGame);
        _timeTrialContinuedGameButton.onClick.AddListener(ContinuedTimeTrialGame);

        _playButton.onClick.AddListener(OpenChooseModeScreen);
        _easyLvlButton.onClick.AddListener(NewClassicEasyGame);
        _mediumLvlButton.onClick.AddListener(NewClassicMediumGame);
        _hardLvlButton.onClick.AddListener(NewClassicHardGame);
        _customLvlButton.onClick.AddListener(OpenCustomGameSettingsScreen);
        //_episodeGameMenuButton.onClick.AddListener(OpenEpisodeGame);
        _settingsButton.onClick.AddListener(OpenSettingsScreen);
        _continueGameButton.onClick.AddListener(ContinueLastSession);
        _continueClassicGameButton.onClick.AddListener(ContinueLastClassicSession);
        _bonusButton.onClick.AddListener(OpenBonusScreen);

        _chooseInfinitymodeButton.onClick.AddListener(OpenInfinityModeMenuScreen);
        _newClassicGameButton.onClick.AddListener(OpenClassicModeMenuScreen);

        UpdateNewGameButtonsText();
        ContinueClassicButtonInteractable();        
        CheckNewGameButtonsInteractable(new OnGameRewardSignal());
        UpdateLastSessionStatistic(new GameManagerLoadCompletedSignal());
        UpdateLastClassicSessionStatistic(new GameManagerLoadCompletedSignal());
        //UpdateLastSessionStatistic(new LoadCompletedSignal());        
    }       

    public void SelectMenu(int index)
    {
        SetActiveScreen(index);
    }

    private void SetActiveScreen(int index)
    {
        for (int i = 0; i < _mainMenuScreens.Length; i++)
        {
            _mainMenuScreens[i].gameObject.SetActive(i == index);            
        }

        _infinityModeMenuScreen.gameObject.SetActive(false);
        _classicModeMenuScreen.gameObject.SetActive(false);
    }

    private void ContinueLastSession()
    {
        if (_gameManager != null && _gameManager.CurrentStatisticController != null)
        {
            if (_gameManager.LastSessionGameMode == GameMode.SimpleInfinite ||
                _gameManager.LastSessionGameMode == GameMode.Hardcore ||
                _gameManager.LastSessionGameMode == GameMode.TimeTrial)
            {
                _gameManager.SetCurrentGameMode(_gameManager.LastSessionGameMode);
                _sceneLoader.LoadScene(SceneType.InfiniteModeScene);
            }
            else if (_gameManager.LastSessionGameMode == GameMode.ClassicEasy ||
                _gameManager.LastSessionGameMode == GameMode.ClassicMedium ||
                _gameManager.LastSessionGameMode == GameMode.ClassicHard ||
                _gameManager.LastSessionGameMode == GameMode.Custom)
            {
                _gameManager.SetCurrentGameMode(_gameManager.LastSessionGameMode);
                _sceneLoader.LoadScene(SceneType.ClassicModeScene);
            }
        }
        else
        {
            _gameManager.SetCurrentGameMode(GameMode.SimpleInfinite);
            _sceneLoader.LoadScene(SceneType.InfiniteModeScene);
        }
    }

    private void ContinueLastClassicSession()
    {
        if (_gameManager != null && _gameManager.CurrentStatisticController != null)
        {
            _gameManager.SetCurrentGameMode(_gameManager.LastClassicSessionMode);
            _sceneLoader.LoadScene(SceneType.ClassicModeScene);
        }        
    }

    private void NewInfinityGame()
    {
        _gameManager.SetCurrentGameMode(GameMode.SimpleInfinite);
        _gameManager.ClearCurrentGame(GameMode.SimpleInfinite);
        _sceneLoader.LoadScene(SceneType.InfiniteModeScene);        
        _playerProgress.SetFirstTimePlayed();
    }

    private void ContinuedInfinityGame()
    {
        _gameManager.SetCurrentGameMode(GameMode.SimpleInfinite);
        _sceneLoader.LoadScene(SceneType.InfiniteModeScene);
    }

    private void NewHardcoreGame()
    {
        if (_playerProgress.CheckAwardCount(_gameManager.HardcoreTimeModeCost))
        {            
            SignalBus.Fire(new OnGameRewardSignal(0, -_gameManager.HardcoreTimeModeCost));

            _gameManager.SetCurrentGameMode(GameMode.Hardcore);
            _gameManager.ClearCurrentGame(GameMode.Hardcore);
            _sceneLoader.LoadScene(SceneType.InfiniteModeScene);            
            _playerProgress.SetFirstTimePlayed();
        }       
    }

    private void ContinuedHardcoreGame()
    {
        _gameManager.SetCurrentGameMode(GameMode.Hardcore);
        _sceneLoader.LoadScene(SceneType.InfiniteModeScene);
    }

    private void NewTimeTrialGame()
    {
        if (_playerProgress.CheckAwardCount(_gameManager.HardcoreTimeModeCost))
        {
            SignalBus.Fire(new OnGameRewardSignal(0, -_gameManager.HardcoreTimeModeCost));

            _gameManager.SetCurrentGameMode(GameMode.TimeTrial);
            _gameManager.ClearCurrentGame(GameMode.TimeTrial);
            _sceneLoader.LoadScene(SceneType.InfiniteModeScene);
            _playerProgress.SetFirstTimePlayed();
        }        
    }

    private void ContinuedTimeTrialGame()
    {
        _gameManager.SetCurrentGameMode(GameMode.TimeTrial);
        _sceneLoader.LoadScene(SceneType.InfiniteModeScene);
    }

    private void NewClassicEasyGame()
    {
        _gameManager.SetCurrentGameMode(GameMode.ClassicEasy);
        _gameManager.ClearCurrentGame(GameMode.ClassicEasy);
        _sceneLoader.LoadScene(SceneType.ClassicModeScene);
        _playerProgress.SetFirstTimePlayed();
    }

    private void NewClassicMediumGame()
    {
        _gameManager.SetCurrentGameMode(GameMode.ClassicMedium);
        _gameManager.ClearCurrentGame(GameMode.ClassicMedium);
        _sceneLoader.LoadScene(SceneType.ClassicModeScene);
        _playerProgress.SetFirstTimePlayed();
    }

    private void NewClassicHardGame()
    {
        _gameManager.SetCurrentGameMode(GameMode.ClassicHard);
        _gameManager.ClearCurrentGame(GameMode.ClassicHard);
        _sceneLoader.LoadScene(SceneType.ClassicModeScene);
        _playerProgress.SetFirstTimePlayed();
    }

    private void OpenCustomGameSettingsScreen()
    {
        _customGameSettingsScreen.gameObject.SetActive(true);
    }

    private void OpenChooseModeScreen()
    {
        SetActiveScreen(1);
    }

    private void OpenInfinityModeMenuScreen() 
    {        
        _infinityModeMenuScreen.gameObject.SetActive(true);
    }

    private void OpenClassicModeMenuScreen()
    {
        _classicModeMenuScreen.gameObject.SetActive(true);
        ContinueClassicButtonInteractable();
    }

    private void OpenBonusScreen()
    {
        _bonusScreen.gameObject.SetActive(true);
    }

    private void OpenEpisodeGame()
    {

    }

    private void OpenSettingsScreen()
    {
        _settingsScreen.gameObject.SetActive(true);
    }


    private void UpdateLastSessionStatistic(GameManagerLoadCompletedSignal signal)
    {
        StartCoroutine(UpdateLastSession(signal));
    }

    private IEnumerator UpdateLastSession(GameManagerLoadCompletedSignal signal)
    {
        yield return null;
        
        if (_gameManager != null && _gameManager.CurrentStatisticController != null)
        {
            _lastModeText.text = GetGameModeName(_gameManager.LastSessionGameMode);
            _timeSpentText.text = "Time spent: " + FormatTime(_gameManager.CurrentStatisticController.TotalPlayTime);
            _flagsPlacedText.text = "Flags placed: " + _gameManager.CurrentStatisticController.PlacedFlags.ToString();
            _cellsOpenText.text = "Cells open: " + _gameManager.CurrentStatisticController.OpenedCells.ToString();
        }
        else
        {
            _lastModeText.text = "Choose mode";
            _timeSpentText.text = "Time spent: 0";
            _flagsPlacedText.text = "Flags placed: 0";
            _cellsOpenText.text = "Cells open: 0";
        }
    }

    private void UpdateLastClassicSessionStatistic(GameManagerLoadCompletedSignal signal)
    {
        if (_gameManager != null && _gameManager.ClassicStats != null)
        {
            //_lastClassicSessionText.text = "Last session " + GetGameModeName(GameManager.Instance.LastClassicSessionMode);
            _difficultyLevelText.text = "Difficulty level: " + GetGameModeDifficulty(_gameManager.LastClassicSessionMode);            
            _classicSessionCellsOpenText.text = "Cells open: " + _gameManager.ClassicStats.OpenedCells.ToString();
            _classicSessionFlagsPlacedText.text = "Flags placed: " + _gameManager.ClassicStats.PlacedFlags.ToString();
            _classicSessionTimeSpentText.text = "Time spent: " + FormatTime(_gameManager.ClassicStats.TotalPlayTime);            
        }
    }

    private string GetGameModeName(GameMode lastMode)
    {
        switch (lastMode)
        {
            case GameMode.SimpleInfinite:
                return "Infinity";

            case GameMode.Hardcore:
                return "Hardcore";

            case GameMode.TimeTrial:
                return "Time";

            case GameMode.ClassicEasy:
                return "Classic easy";

            case GameMode.ClassicMedium:
                return "Classic medium";

            case GameMode.ClassicHard:
                return "Classic hard";

            case GameMode.Custom:
                return "Custom level";

            default:
                return "Choose mode";
        }        
    }

    private string GetGameModeDifficulty(GameMode lastMode)
    {
        switch (lastMode)
        {           
            case GameMode.ClassicEasy:
                return "Easy";

            case GameMode.ClassicMedium:
                return "Medium";

            case GameMode.ClassicHard:
                return "Hard";

            case GameMode.Custom:
                return "Custom";

            default:
                return "";
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

    private void ContinueClassicButtonInteractable()
    {
        _continueClassicGameButton.interactable = _saveManager.HasClassicGameSave();
        SignalBus.Fire(new ThemeChangeSignal(_themeManager.CurrentTheme, _themeManager.CurrentThemeIndex));
    }

    private void PlayButtonInteractable(PlayerProgressLoadCompletedSignal signal)
    {
        if (_playerProgress != null)
        {
            _startTutorialObject.gameObject.SetActive(!_playerProgress.IsFirstTimePlayed);
            _containerLastSession.SetActive(_playerProgress.IsFirstTimePlayed);


            UpdateAwardText(new OnGameRewardSignal());              ///KOSTYLISCHE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
    }

    private void UpdateAwardText(OnGameRewardSignal signal)
    {
        StartCoroutine(UpdateAwardTextDelayed(signal));        
    }

    private IEnumerator UpdateAwardTextDelayed(OnGameRewardSignal signal)
    {
        yield return null;
        foreach (var text in _awardTexts)
        {
            text.text = _playerProgress.TotalReward.ToString();
        }
    }

    private void UpdateNewGameButtonsText()
    {
        _hardcoreNewGameText.text = $"New game \n<sprite=0> {_gameManager.HardcoreTimeModeCost}";
        _timeTrialNewGameText.text = $"New game \n<sprite=0> {_gameManager.HardcoreTimeModeCost}";
    }

    private void CheckNewGameButtonsInteractable(OnGameRewardSignal signal)
    {
        _hardcoreNewGameMenuButton.interactable = _playerProgress.CheckAwardCount(_gameManager.HardcoreTimeModeCost); 
        _timeTrialNewGameButton.interactable = _playerProgress.CheckAwardCount(_gameManager.HardcoreTimeModeCost);
    }

    private void OnBonusAvailable(OnBonusGrantSignal signal)
    {
        if (signal.RewardAmount > 0)
        {
            StartBonusButtonAnimation();
        }
        else
        {
            StopBonusButtonAnimation();
        }
    }

    private void OnBonusCollected(OnGameRewardSignal signal)
    {
        StopBonusButtonAnimation();
    }

    private void StartBonusButtonAnimation()
    {
        StopBonusButtonAnimation();

        _bonusButtonTween = _bonusButton.transform.DOScale(1.33f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);

        _bonusButtonRotationTween = _bonusButton.transform.DORotate(new Vector3(0, 0, 10), 0.75f)
            .From(new Vector3(0, 0, -10))
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);


        /*if (_bonusButton.TryGetComponent(out Image buttonImage))
        {
            Color targetColor;
            if (ColorUtility.TryParseHtmlString("#D31C64", out targetColor))
            {
                buttonImage.DOColor(targetColor, 0.75f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutQuad);
            }
        }*/
        //.DOShakePosition(2f, strength: new UnityEngine.Vector3(10f, 10f, 0), vibrato: 5, randomness: 90)
        //.SetLoops(-1)
        //.SetEase(Ease.Linear);
    }

    private void StopBonusButtonAnimation()
    {
        if (_bonusButtonTween != null)
        {
            _bonusButtonTween.Kill();
            _bonusButtonRotationTween.Kill();
            _bonusButton.transform.localScale = Vector3.one;
            _bonusButton.transform.rotation = Quaternion.identity;
            _bonusButtonTween = null;
            _bonusButtonRotationTween = null;
        }
    }


    private void OnEnable()
    {
        //SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastSessionStatistic);        
        //UpdateLastSessionStatistic(new GameManagerLoadCompletedSignal());

        //SignalBus.Subscribe<GameManagerLoadCompletedSignal>(UpdateLastClassicSessionStatistic);
        //UpdateLastClassicSessionStatistic(new GameManagerLoadCompletedSignal());

        SignalBus.Subscribe<PlayerProgressLoadCompletedSignal>(PlayButtonInteractable);
        PlayButtonInteractable(new PlayerProgressLoadCompletedSignal());

        SignalBus.Subscribe<OnGameRewardSignal>(UpdateAwardText);
        SignalBus.Subscribe<OnGameRewardSignal>(CheckNewGameButtonsInteractable);

        SignalBus.Subscribe<OnBonusGrantSignal>(OnBonusAvailable);
        SignalBus.Subscribe<OnGameRewardSignal>(OnBonusCollected);

        OnBonusAvailable(new OnBonusGrantSignal(_rewardManager.CurrentReward));
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<GameManagerLoadCompletedSignal>(UpdateLastSessionStatistic);
        SignalBus.Unsubscribe<PlayerProgressLoadCompletedSignal>(PlayButtonInteractable);
        SignalBus.Unsubscribe<OnGameRewardSignal>(UpdateAwardText);
        SignalBus.Unsubscribe<OnGameRewardSignal>(CheckNewGameButtonsInteractable);

        SignalBus.Unsubscribe<OnBonusGrantSignal>(OnBonusAvailable);
        SignalBus.Unsubscribe<OnGameRewardSignal>(OnBonusCollected);

        StopBonusButtonAnimation();
    }
}
