using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    }

    private void CloseScreen()
    {
        //gameObject.SetActive(false);
        ClearInputFields();
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
        GameManager.Instance.SetCustomLevelSettings(customLevel);

        GameManager.Instance.SetCurrentGameMode(GameMode.Custom);
        GameManager.Instance.ClearCurrentGame(GameMode.Custom);
        //GameManager.Instance.ResetCurrentModeStatistic();
        SceneLoader.Instance.LoadClassicMinesweeperScene();

        PlayerProgress.Instance.SetFirstTimePlayed();

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
