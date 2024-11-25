using UnityEngine;
using UnityEngine.UI;

public class ThemeSelector : MonoBehaviour
{
    [SerializeField] private Button[] _themeButtons;

    private void Start()
    {
        for (int i = 0; i < _themeButtons.Length; i++)
        {
            int themeIndex = i;
            _themeButtons[i].onClick.AddListener(() => SelectTheme(themeIndex));
        }
    }

    private void SelectTheme(int index)
    {
        ThemeManager.Instance.ApplyTheme(index);
    }
}
