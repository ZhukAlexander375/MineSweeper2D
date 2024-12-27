using UnityEngine;
using UnityEngine.UI;

public class ThemeSelector : MonoBehaviour
{
    [SerializeField] private Button[] _themeButtons;
    [SerializeField] private Image[] _themeImages;
    [SerializeField] private GameObject _chooseFrame;

    private void Start()
    {
        for (int i = 0; i < _themeButtons.Length; i++)
        {
            int themeIndex = i;
            _themeButtons[i].onClick.AddListener(() => SelectTheme(themeIndex));
        }

        int currentThemeIndex = ThemeManager.Instance.CurrentThemeIndex;
        ShowOutline(new ThemeChangeSignal(ThemeManager.Instance.CurrentTheme, currentThemeIndex));
    }

    private void SelectTheme(int index)
    {
        ThemeManager.Instance.ApplyTheme(index);        
    }

    private void ShowOutline(ThemeChangeSignal signal)
    {
        var index = signal.ThemeIndex;

        _chooseFrame.transform.position = _themeImages[index].transform.position;
        _chooseFrame.transform.SetParent(_themeImages[index].transform);
        _chooseFrame.SetActive(true);
    }

    private void OnEnable()
    {
        SignalBus.Subscribe<ThemeChangeSignal>(ShowOutline);
    }

    private void OnDisable()
    {
        SignalBus.Unsubscribe<ThemeChangeSignal>(ShowOutline);
    }
}
