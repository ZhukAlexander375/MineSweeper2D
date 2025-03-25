using UnityEngine;
using Zenject;

public class ThemeManager : MonoBehaviour
{
    [SerializeField] private ThemeConfig[] _themes;
    
    public ThemeConfig CurrentTheme { get; private set; }
    public int CurrentThemeIndex {  get; private set; }

    private SaveManager _saveManager;

    [Inject]
    private void Construct(SaveManager saveManager)
    {
        _saveManager = saveManager;
    }

    private void Start()
    {       
        LoadSavedTheme();
    }

    public void ApplyTheme(int themeIndex)
    {
        if (themeIndex < 0 || themeIndex >= _themes.Length)
        {
            return;
        }
        CurrentThemeIndex = themeIndex;
        CurrentTheme = _themes[themeIndex];

        _saveManager.SaveSelectedTheme(CurrentThemeIndex);

        SignalBus.Fire(new ThemeChangeSignal(CurrentTheme, CurrentThemeIndex));
    }

    private void LoadSavedTheme()
    {
        int savedThemeIndex = _saveManager.LoadSelectedTheme();

        if (savedThemeIndex == -1 || savedThemeIndex >= _themes.Length)
        {
            savedThemeIndex = 0;
        }

        CurrentThemeIndex = savedThemeIndex;
        CurrentTheme = _themes[savedThemeIndex];
    }
}

