using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeUIController : MonoBehaviour
{
    [SerializeField] private Image[] _menuFrameBackgrounds;
    [SerializeField] private Shadow[] _menuFrameShadows;
    [SerializeField] private Button[] _buttonsMain;
    [SerializeField] private Shadow[] _buttonsShadowMain;
    [SerializeField] private Button[] _buttonsMinor;    
    [SerializeField] private Shadow[] _buttonsShadowMinor;

    /// <summary>
    /// ZATYCHKA FOR NAVIGATION PANEL AND MENU TITLE IMAGE 
    /// </summary>

    [SerializeField] private Image _menuTitleImage;
    [SerializeField] private Image _navigationPanelImage;
    ///
    ///

    [SerializeField] private Image[] _topFieldImage;
    [SerializeField] private Image[] _icons;
    [SerializeField] private Image _transparentBackground;
    
    
    [SerializeField] private TMP_Text _menuTitleText;        
    [SerializeField] private TMP_Text[] _textsOnButtons;
    [SerializeField] private InputFieldHandler[] _inputFields;
    

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

        //ApplyTextsColors(_currentAppliedTheme);
        ApplyButtonsColor(_currentAppliedTheme);
        ApplyBackgrounsColor(_currentAppliedTheme);
        ApplyInputFieldsColor(_currentAppliedTheme);

        ApplyMenuTitleImage(_currentAppliedTheme);
        ApplyNavigationPanel(_currentAppliedTheme);
    }

    /*private void ApplyTextsColors(ThemeConfig theme)
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

        if (_inputFields != null && _inputFields.Length > 0)
        {
            foreach (var field in _inputFields)
            {
                if (field?.InputField != null && field.InputField.placeholder != null)
                {                    
                    field.InputField.placeholder.color = theme.TextsOnButtons;
                }
                else
                {
                    Debug.LogWarning($"InputField или его placeholder отсутствует на объекте {field?.name ?? "неизвестный"}.");
                }
            }
        }

        *//*if (_lightTextsOnButtons.Length > 0)
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
        }*//*
    }*/

    private void ApplyButtonsColor(ThemeConfig theme)
    {
        if (_buttonsMain.Length > 0)
        {
            foreach (var button in _buttonsMain)
            {
                button.image.color = theme.ButtonsMainColor;
            }
        }

        if (_buttonsShadowMain.Length > 0)
        {
            foreach (var shadow in _buttonsShadowMain)
            {
                shadow.effectColor = theme.ButtonsMainShadowColor;
            }
        }

        if (_buttonsMinor.Length > 0)
        {
            foreach (var button in _buttonsMinor)
            {
                button.image.color = theme.ButtonsMinorColor;
            }
        }

        if (_buttonsShadowMinor.Length > 0)
        {
            foreach (var shadow in _buttonsShadowMinor)
            {
                shadow.effectColor = theme.ButtonsMinorShadowColor;
            }
        }
    }

    private void ApplyBackgrounsColor(ThemeConfig theme)
    {
        if (_topFieldImage.Length > 0)
        {
            foreach (var image in _topFieldImage)
            {
                image.color = theme.TopFieldColor;
            }
        }

        /*if (_transparentBackground != null)
        {
            _transparentBackground.color = theme.TransparentBGColor;
        }*/

        if (_menuFrameBackgrounds.Length > 0)
        {
            foreach (var image in _menuFrameBackgrounds)
            {
                image.color = theme.MenuFrameBGColor;
            }
        }
        
        if (_menuFrameShadows.Length > 0)
        {
            foreach (var shadow in _menuFrameShadows)
            {
                shadow.effectColor = theme.MenuFrameShadowColor;
            }
        }

        /*if (_icons.Length > 0)
        {
            foreach (var icon in _icons)
            {
                icon.color = theme.IconsColor;
            }
        }*/
    }

    private void ApplyInputFieldsColor(ThemeConfig theme)
    {
        if (_inputFields == null || _inputFields.Length == 0)
        {
            return;
        }

        /*foreach (var field in _inputFields)
        {
            if (field?.InputField != null && field.InputField.image != null)
            {
                field.InputField.image.color = theme.ButtonsColor;
            }
            else
            {
                Debug.LogWarning($"InputField или его image отсутствует на объекте {field?.name ?? "неизвестный"}.");
            }
        }*/
    }

    private void ApplyNavigationPanel(ThemeConfig theme)
    {
        if (_navigationPanelImage != null)
        {
            _navigationPanelImage.sprite = theme.NavigationPanel;
        }
    }

    private void ApplyMenuTitleImage(ThemeConfig theme)
    {
        if (_menuTitleImage != null)
        {
            _menuTitleImage.sprite = theme.MenuTitleImage;
        }
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<ThemeChangeSignal>(OnThemeChanged);
    }
}
