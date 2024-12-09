using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    [SerializeField] private ThemeConfig[] _themes;
    public static ThemeManager Instance { get; private set; }
    public ThemeConfig CurrentTheme { get; private set; }
    public int CurrentThemeIndex {  get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
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

        SaveManager.Instance.SaveSelectedTheme(CurrentThemeIndex);

        SignalBus.Fire(new ThemeChangeSignal(CurrentTheme, CurrentThemeIndex));
    }

    private void LoadSavedTheme()
    {
        int savedThemeIndex = SaveManager.Instance.LoadSelectedTheme();

        if (savedThemeIndex == -1 || savedThemeIndex >= _themes.Length)
        {
            savedThemeIndex = 0;
        }

        ApplyTheme(savedThemeIndex);
    }
}

