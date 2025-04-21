using System;
using Zenject;

public class UIServiceClassic : IInitializable, IDisposable
{
    public GameUIClassic GameUIClassic { get; private set; }
    public PopupUIClassic PopupUIClassic { get; private set; }
    public OverUIClassic OverUIClassic { get; private set; }
    public GameManager GameManager => _gameManager;

    private SimpleGridManager _simpleGridManager;
    private PlayerProgress _playerProgress;
    private GameManager _gameManager;
    private SceneLoader _sceneLoader;

    [Inject]
    public UIServiceClassic(GameUIClassic gameUIClassic,
                            PopupUIClassic popupUIClassic,
                            OverUIClassic overUIClassic,
                            SimpleGridManager simpleGridManager,
                            PlayerProgress playerProgress,
                            GameManager gameManager,
                            SceneLoader sceneLoader)
    {
        GameUIClassic = gameUIClassic;
        PopupUIClassic = popupUIClassic;
        OverUIClassic = overUIClassic;
        _simpleGridManager = simpleGridManager;
        _playerProgress = playerProgress;
        _gameManager = gameManager;
        _sceneLoader = sceneLoader;
    }

    public void Initialize()
    {
        SignalBus.Subscribe<GameOverSignal>(OpenLoseScreen);
        SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagText);

        SetModeName();
    }

    public void Dispose()
    {
        SignalBus.Unsubscribe<GameOverSignal>(OpenLoseScreen);
        SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagText);
    }

    public void StopGameTimer()
    {
        _gameManager.CurrentStatisticController.StopTimer();
    }

    public void StartGameTimer()
    {
        _gameManager.CurrentStatisticController.StartTimer();
    }

    public void ReplayGame()
    {
        _gameManager.ResetCurrentModeStatistic();
        _gameManager.ClearCurrentGame(_gameManager.CurrentGameMode);
        _gameManager.SetCurrentGameMode(_gameManager.CurrentGameMode);
    }

    public void CheckFirstClick()
    {
        if (_simpleGridManager.IsFirstClick)
        {
            _simpleGridManager.SaveCurrentGame();
        }
    }

    public void LoadScene(SceneType sceneType)
    {
        _sceneLoader.LoadScene(sceneType);
    }

    public void OpenPauseMenu()
    {
        PopupUIClassic.OpenPauseMenu();
    }

    public void OpenSettngs()
    {
        _gameManager.CurrentStatisticController.StopTimer();
        OverUIClassic.OpenSettingsScreen();
    }

    public void CloseLoseScreen()
    {
        PopupUIClassic.CloseWinLoseScreen();
    }

    private void SetModeName()
    {
        switch (_gameManager.CurrentGameMode)
        {
            case GameMode.ClassicEasy:
                GameUIClassic.SetModeName("Easy");
                //_gameModeName.text = "Easy";
                break;

            case GameMode.ClassicMedium:
                GameUIClassic.SetModeName("Medium");
                //_gameModeName.text = "Medium";
                break;

            case GameMode.ClassicHard:
                GameUIClassic.SetModeName("Hard");
                //_gameModeName.text = "Hard";
                break;

            case GameMode.Custom:
                GameUIClassic.SetModeName("Custom");
                //_gameModeName.text = "Custom";
                break;
        }
    }

    public void SavePlayerProgress()
    {
        _playerProgress.SavePlayerProgress();
    }

    private void OpenLoseScreen(GameOverSignal signal)
    {
        if (signal.CurrentGameMode == _gameManager.CurrentGameMode)
        {
            if (signal.IsGameOver == true || signal.IsGameWin == true)
            {
                PopupUIClassic.OpenWinLoseScreen(true);
                GameUIClassic.PauseButtonInteractable(false);
                //_loseScreen.gameObject.SetActive(true);
                //_pauseButton.gameObject.SetActive(false);
                //_pauseMenuScreen.gameObject.SetActive(false);
            }
            else
            {
                PopupUIClassic.OpenWinLoseScreen(false);
                GameUIClassic.PauseButtonInteractable(true);
                //_loseScreen.gameObject.SetActive(false);
                //_pauseButton.gameObject.SetActive(true);
            }
        }
    }

    private void UpdateFlagText(FlagPlacingSignal signal)
    {
        GameUIClassic.UpdateFlagText();
    }

}
