using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameUIClassic : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _pauseButton;
    //[SerializeField] private Button _settingsButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingsOnPauseButton;
    [SerializeField] private Button _replayLevelButton;
    [SerializeField] private Button _goHomeButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text _gameModeText;
    [SerializeField] private TMP_Text _minesCountText;
    [SerializeField] private TMP_Text _flagsText;


    private UIServiceClassic _uiServiceClassic;

    [Inject]
    public void Construct(UIServiceClassic uiService)
    {
        _uiServiceClassic = uiService;
    }

    private void Start()
    {
        //Debug.Log("Game UI inastall");
        _pauseButton.onClick.AddListener(OpenPauseMenu);
        //_settingsButton.onClick.AddListener(OpenSettings);
        _continueButton.onClick.AddListener(ClosePauseMenu);
        _settingsOnPauseButton.onClick.AddListener(OpenSettings);
        _replayLevelButton.onClick.AddListener(ReplayGame);
        _goHomeButton.onClick.AddListener(ReturnToMainMenu);
    }


    public void SetModeName(string modeName)
    {
        _gameModeText.text = modeName;
    }

    public void PauseButtonInteractable(bool isActive)
    {
        _pauseButton.gameObject.SetActive(isActive);
    }

    public void UpdateFlagText()
    {
        if (_uiServiceClassic.GameManager.CurrentStatisticController != null)
        {
            _flagsText.text = _uiServiceClassic.GameManager.CurrentStatisticController.PlacedFlags.ToString();
        }
        else
        {
            _flagsText.text = "0";
        }
    }

    private void OpenPauseMenu()
    {
        _uiServiceClassic.OpenPauseMenu();
        _uiServiceClassic.StopGameTimer();
    }

    private void OpenSettings()
    {
        _uiServiceClassic.OpenSettngs();
    }

    private void ClosePauseMenu()
    {
        _uiServiceClassic.StartGameTimer();
        gameObject.SetActive(false);
    }

    private void ReplayGame()
    {
        _uiServiceClassic.CloseLoseScreen();
        _uiServiceClassic.ReplayGame();
        _uiServiceClassic.LoadScene(SceneType.ClassicModeScene);

    }

    private void ReturnToMainMenu()
    {
        _uiServiceClassic.StopGameTimer();
        _uiServiceClassic.SavePlayerProgress();
        _uiServiceClassic.CheckFirstClick();
        _uiServiceClassic.LoadScene(SceneType.MainMenu);
    }

    private void UpdateBombText()
    {
        switch (_uiServiceClassic.GameManager.CurrentGameMode)
        {
            case GameMode.ClassicEasy:
                SetMinesCount(0);
                break;

            case GameMode.ClassicMedium:
                SetMinesCount(1);
                break;

            case GameMode.ClassicHard:
                SetMinesCount(2);
                break;

            case GameMode.Custom:
                SetMinesCount(_uiServiceClassic.GameManager.CustomLevel);
                break;
        }
    }

    private void SetMinesCount(int levelIndex)
    {
        var levels = _uiServiceClassic.GameManager.PredefinedLevels;

        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            _minesCountText.text = levels[levelIndex].MineCount.ToString();
            //Debug.Log($"{levels[levelIndex].MineCount}");
        }
        else
        {
            Debug.LogError("Level index out of range");
        }
    }

    private void SetMinesCount(LevelConfig customLevel)
    {
        _minesCountText.text = customLevel.MineCount.ToString();
        //Debug.Log($"{customLevel.MineCount}");
    }

    private void OnEnable()
    {
        UpdateBombText();
    }
}
