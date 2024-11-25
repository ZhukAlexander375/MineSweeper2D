using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeUIController : MonoBehaviour
{
    [SerializeField] private Image _topFieldImage;
    [SerializeField] private Image[] _icons;
    [SerializeField] private Image _transparentBackground;
    [SerializeField] private Image _menuFrameBackground;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private TMP_Text _menuTitleText;        
    [SerializeField] private TMP_Text[] _textsOnButtons;    
    

    private ThemeConfig _currentAppliedTheme;

    private void Start()
    {
        SignalBus.Subscribe<ThemeChangeSignal>(OnThemeChanged);
        TryApplyTheme(ThemeManager.Instance.CurrentTheme);
    }

    private void OnEnable()
    {
        if (ThemeManager.Instance == null)
        {
            Debug.Log("ThemeManager.Instance не инициализирован!");
            return;
        }

        TryApplyTheme(ThemeManager.Instance.CurrentTheme);
    }

    private void OnThemeChanged(ThemeChangeSignal signal)
    {        
        TryApplyTheme(signal.Theme);
    }

    private void TryApplyTheme(ThemeConfig theme)
    {
        if (theme == null)
        {
            return;
        }

        if (_currentAppliedTheme == theme)
        {
            return;
        }

        ApplyTheme(theme);
    }

    private void ApplyTheme(ThemeConfig theme)
    {
        if (theme == null)
        {
            return;
        }

        _currentAppliedTheme = theme;

        ApplyTextsColors(_currentAppliedTheme);
        ApplyButtonsColor(_currentAppliedTheme);
        ApplyBackgrounsColor(_currentAppliedTheme);                
    }

    private void ApplyTextsColors(ThemeConfig theme)
    {
        if (_menuTitleText != null)
        {
            _menuTitleText.color = theme.MenuTitleTextColor;
        }

        if (_textsOnButtons.Length > 0)
        {
            foreach (var text in _textsOnButtons)
            {
                text.color = theme.TextsOnButtons;
            }
        }

        /*if (_lightTextsOnButtons.Length > 0)
        {
            foreach (var text in _lightTextsOnButtons)
            {
                text.color = theme.ButtonsTextColorLight;
            }
        }

        if (_darkTextsOnButtons.Length > 0)
        {
            foreach (var text in _darkTextsOnButtons)
            {
                text.color = theme.ButtonsTextColorDark;
            }
        }*/
    }

    private void ApplyButtonsColor(ThemeConfig theme)
    {
        if (_buttons.Length > 0)
        {
            foreach (var button in _buttons)
            {
                button.image.color = theme.ButtonsColor;
            }
        }
    }

    private void ApplyBackgrounsColor(ThemeConfig theme)
    {
        if (_topFieldImage != null)
        {
            _topFieldImage.color = theme.TopFieldColor;
        }

        if (_transparentBackground != null)
        {
            _transparentBackground.color = theme.TransparentBGColor;
        }

        if (_menuFrameBackground != null)
        {
            _menuFrameBackground.color = theme.MenuFrameBGColor;
        }

        if (_icons.Length > 0)
        {
            foreach(var icon in _icons)
            {
                icon.color = theme.IconsColor;
            }
        }

    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<ThemeChangeSignal>(OnThemeChanged);
    }
}
