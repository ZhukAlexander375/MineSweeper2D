using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CustomGameSettingsScreen : MonoBehaviour
{
    [Header("InputFields")]
    [SerializeField] private InputFieldHandler _inputWidth;
    [SerializeField] private InputFieldHandler _inputHeight;
    [SerializeField] private InputFieldHandler _inputMines;

    [Header("Buttons")]
    [SerializeField] private Button _startCustomModeButton;
    [SerializeField] private Button _closeCustomGameSettingsScreen;

    [Header("Texts")]
    [SerializeField] private TMP_Text _minesText;


    private ThemeManager _themeManager;
    private PlayerProgress _playerProgress;
    private GameManager _gameManager;
    private SceneLoader _sceneLoader;

    [Inject]
    private void Construct(ThemeManager themeManager, PlayerProgress playerProgress, GameManager gameManager, SceneLoader sceneLoader)
    {
        _themeManager = themeManager;
        _playerProgress = playerProgress;
        _gameManager = gameManager;
        _sceneLoader = sceneLoader;
    }


    void Start()
    {
        _inputWidth.InputField.onEndEdit.AddListener(value => ValidateInput(_inputWidth, ClassicGameMinSize.MinWidth, ClassicGameMinSize.MaxWidth));
        _inputHeight.InputField.onEndEdit.AddListener(value => ValidateInput(_inputHeight, ClassicGameMinSize.MinHeight, ClassicGameMinSize.MaxHeight));
        _inputMines.InputField.onEndEdit.AddListener(value =>
        {
            int width = GetClampedValue(_inputWidth.InputField.text, ClassicGameMinSize.MinWidth, ClassicGameMinSize.MaxWidth);
            int height = GetClampedValue(_inputHeight.InputField.text, ClassicGameMinSize.MinHeight, ClassicGameMinSize.MaxHeight);
            ValidateInput(_inputMines, ClassicGameMinSize.MinMines(width, height), ClassicGameMinSize.MaxMines(width, height));
        });

        _inputWidth.InputField.onValueChanged.AddListener(value => OnFieldSizeChanged());
        _inputHeight.InputField.onValueChanged.AddListener(value => OnFieldSizeChanged());

        _closeCustomGameSettingsScreen.onClick.AddListener(CloseScreen);
        _startCustomModeButton.onClick.AddListener(StartCustomGame);

        OnFieldSizeChanged();        
        ValidateStartButton();
    }

    private void OnFieldSizeChanged()
    {
        bool areSizeFieldsValid = !string.IsNullOrEmpty(_inputWidth.InputField.text)
                                  && !string.IsNullOrEmpty(_inputHeight.InputField.text);

        _inputMines.InputField.interactable = areSizeFieldsValid;

        if (areSizeFieldsValid)
        {
            UpdateMinesRange();
        }
        else
        {
            _minesText.text = "Mines";
        }
    }

    private void UpdateMinesRange()
    {
        int width = GetClampedValue(_inputWidth.InputField.text, ClassicGameMinSize.MinWidth, ClassicGameMinSize.MaxWidth);
        int height = GetClampedValue(_inputHeight.InputField.text, ClassicGameMinSize.MinHeight, ClassicGameMinSize.MaxHeight);

        int minMines = ClassicGameMinSize.MinMines(width, height);
        int maxMines = ClassicGameMinSize.MaxMines(width, height);

        _minesText.text = $"Mines ({minMines} - {maxMines})";
    }

    private void ValidateInput(InputFieldHandler inputFieldHandler, int min, int max)
    {
        if (int.TryParse(inputFieldHandler.InputField.text, out int value))
        {
            value = ClassicGameMinSize.ClampValue(value, min, max);
            inputFieldHandler.InputField.text = value.ToString();
        }
        else
        {
            inputFieldHandler.InputField.text = min.ToString();
        }

        ValidateStartButton();
    }

    private int GetClampedValue(string input, int min, int max)
    {
        if (int.TryParse(input, out int value))
        {
            return ClassicGameMinSize.ClampValue(value, min, max);
        }
        return min;
    }

    private void ValidateStartButton()
    {
        _startCustomModeButton.interactable = !string.IsNullOrEmpty(_inputWidth.InputField.text)
                                    && !string.IsNullOrEmpty(_inputHeight.InputField.text)
                                    && !string.IsNullOrEmpty(_inputMines.InputField.text);

        SignalBus.Fire(new ThemeChangeSignal(_themeManager.CurrentTheme, _themeManager.CurrentThemeIndex));
    }

    private void CloseScreen()
    {
        //gameObject.SetActive(false);
        ClearInputFields();
        ValidateStartButton();
    }

    public void StartCustomGame()
    {       
        int width = GetClampedValue(_inputWidth.InputField.text, ClassicGameMinSize.MinWidth, ClassicGameMinSize.MaxWidth);
        int height = GetClampedValue(_inputHeight.InputField.text, ClassicGameMinSize.MinHeight, ClassicGameMinSize.MaxHeight);
        int maxMines = ClassicGameMinSize.MaxMines(width, height);
        int mines = GetClampedValue(_inputMines.InputField.text, ClassicGameMinSize.MinMines(width, height), maxMines);

        //Debug.Log($"width: {width}, height: {height}, mines: {mines}");

        LevelConfig customLevel = ScriptableObject.CreateInstance<LevelConfig>();
        customLevel.Width = width;
        customLevel.Height = height;
        customLevel.MineCount = mines;
        _gameManager.SetCustomLevelSettings(customLevel);

        _gameManager.SetCurrentGameMode(GameMode.Custom);
        _gameManager.ClearCurrentGame(GameMode.Custom);
        //_gameManager.ResetCurrentModeStatistic();

        _sceneLoader.LoadScene(SceneType.ClassicModeScene);
        _playerProgress.SetFirstTimePlayed();

        ClearInputFields();
    }

    private void ClearInputFields()
    {
        _inputWidth.InputField.text = string.Empty;
        _inputHeight.InputField.text = string.Empty;
        _inputMines.InputField.text = string.Empty;
        OnFieldSizeChanged();
    }
}
