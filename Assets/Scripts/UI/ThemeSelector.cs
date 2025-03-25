using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ThemeSelector : MonoBehaviour
{
    [SerializeField] private Button[] _themeButtons;
    [SerializeField] private Image[] _themeImages;
    [SerializeField] private GameObject _chooseFrame;

    private ThemeManager _themeManager;

    [Inject]
    private void Construct(ThemeManager themeManager)
    {
        _themeManager = themeManager;
    }

    private void Start()
    {
        for (int i = 0; i < _themeButtons.Length; i++)
        {
            int themeIndex = i;
            _themeButtons[i].onClick.AddListener(() => SelectTheme(themeIndex));
        }

        int currentThemeIndex = _themeManager.CurrentThemeIndex;
        ShowOutline(new ThemeChangeSignal(_themeManager.CurrentTheme, currentThemeIndex));
    }

    private void SelectTheme(int index)
    {
        _themeManager.ApplyTheme(index);        
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
