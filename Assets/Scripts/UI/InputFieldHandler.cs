using TMPro;
using UnityEngine;

public class InputFieldHandler : MonoBehaviour
{
    [SerializeField] protected TMP_InputField _inputField;

    public TMP_InputField InputField => _inputField;

    private void Start()
    {
        _inputField.characterLimit = 50;
        _inputField.onValueChanged.AddListener(ValidateInput);
    }

    public virtual void ValidateInput(string input)
    {
        if (!float.TryParse(input, out _))
        {
            _inputField.text = input.Length > 0 ? input.Substring(0, input.Length - 1) : "";
        }
    }

    public void ClearInputField()
    {
        _inputField.text = string.Empty;
    }

    public long GetInputNumber()
    {
        long result;

        if (long.TryParse(_inputField.text, out result))
        {
            return result;
        }
        return 0;
    }
}
