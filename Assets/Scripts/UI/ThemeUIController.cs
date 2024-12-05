using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeUIController : MonoBehaviour
{
    [Header("Backgrounds Colors")]
    [SerializeField] private Image _mainBackground;
    [SerializeField] private Image[] _topFieldImage;
    [SerializeField] private Image _transparentBackground;
    [SerializeField] private Image[] _enabledSettingsImage;
    [SerializeField] private Image[] _disabledSettingsImage;

    [Header("Frames Colors")]
    [SerializeField] private Image[] _menuFrameBackgrounds;
    [SerializeField] private Shadow[] _menuFrameShadows;

    [Header("Buttons Colors")]
    [SerializeField] private Button[] _buttonsMain;
    [SerializeField] private Shadow[] _buttonsShadowMain;
    [SerializeField] private Button[] _buttonsMinor;    
    [SerializeField] private Shadow[] _buttonsShadowMinor;

    [Header("Dropdown Colors")]
    [SerializeField] private Image _dropdown;
    [SerializeField] private Shadow _dropdownShadow;

    [Header("Icons Colors")]
    [SerializeField] private Image[] _iconsSettingsBack;
    [SerializeField] private Image[] _iconsOnActiveButtons;
    [SerializeField] private Image[] _iconsOnInactiveButtons;

    [Header("Sliders Colors")]
    [SerializeField] private Image[] _slidersFillImage;
    [SerializeField] private Image[] _slidersHandleImage;

    [Header("Texts Colors")]
    [SerializeField] private TMP_Text _menuTitleText;    
    [SerializeField] private TMP_Text[] _mainMenuTexts;
    //[SerializeField] private TMP_Text[] _textsOnButtons;


    /// <summary>
    /// ZATYCHKA FOR NAVIGATION PANEL AND MENU TITLE IMAGE 
    /// </summary>
    [Header("Images")]
    [SerializeField] private Image _navigationPanelImage;
    [SerializeField] private Image _menuTitleImage; 



    //[SerializeField] private InputFieldHandler[] _inputFields;
    

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
        ApplyDropdownColor(_currentAppliedTheme);
        ApplyIconsColor(_currentAppliedTheme);
        ApplySlidersColor(_currentAppliedTheme);
        ApplyBackgrounsColor(_currentAppliedTheme);
        ApplyInputFieldsColor(_currentAppliedTheme);

        ApplyMenuTitleImage(_currentAppliedTheme);
        ApplyNavigationPanel(_currentAppliedTheme);
    }

    private void ApplyTextsColors(ThemeConfig theme)
    {
        if (_menuTitleText != null)
        {
            _menuTitleText.color = theme.MenuTitleTextColor;

            Material textMaterial = _menuTitleText.fontMaterial;
            textMaterial.SetColor(ShaderUtilities.ID_FaceColor, theme.MenuTitleTextColor);
            textMaterial.SetColor(ShaderUtilities.ID_GlowColor, theme.MenuTitleTextColor);
        }
        
        if (_mainMenuTexts.Length > 0)
        {
            foreach (var text in _mainMenuTexts)
            {
                text.color = theme.MainMenuTextsColor;
            }
        }

        /*if (_textsOnButtons.Length > 0)
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

        if (_lightTextsOnButtons.Length > 0)
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
                button.image.color = theme.ButtonsInactiveColor;
            }
        }

        if (_buttonsShadowMinor.Length > 0)
        {
            foreach (var shadow in _buttonsShadowMinor)
            {
                shadow.effectColor = theme.ButtonsInactiveShadowColor;
            }
        }
    }

    private void ApplyDropdownColor(ThemeConfig theme)
    {
        if (_dropdown != null)
        {
            _dropdown.color = theme.DropdownColor;
        }

        if (_dropdownShadow != null)
        {
            _dropdownShadow.effectColor = theme.DropdownShadowColor;
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

        if (_mainBackground != null)
        {
            _mainBackground.color = theme.MainBackgroundColor;
        }

        if (_transparentBackground != null)
        {
            _transparentBackground.color = theme.TransparentBackgroundColor;
        }

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

        if (_enabledSettingsImage.Length > 0)
        {
            foreach (var image in _enabledSettingsImage)
            {
                image.color = theme.EnabledSettingColor;
            }
        }

        if (_disabledSettingsImage.Length > 0)
        {
            foreach (var image in _disabledSettingsImage)
            {
                image.color = theme.DisabledSettingColor;
            }
        }
    }

    private void ApplyIconsColor(ThemeConfig theme)
    {
        if (_iconsSettingsBack.Length > 0)
        {
            foreach (var icon in _iconsSettingsBack)
            {
                icon.color = theme.IconsSettingsBackColor;
            }
        }

        if (_iconsOnActiveButtons.Length > 0)
        {
            foreach (var icon in _iconsOnActiveButtons)
            {
                icon.color = theme.IconsOnActiveButtonColor;
            }
        }

        if (_iconsOnInactiveButtons.Length > 0)
        {
            foreach (var icon in _iconsOnInactiveButtons)
            {
                icon.color = theme.IconsOnInactiveButtonColor;
            }
        }
    }      
    
    private void ApplySlidersColor(ThemeConfig theme)
    {
        if (_slidersFillImage.Length > 0)
        {
            foreach (var fill in _slidersFillImage)
            {
                fill.color = theme.SliderFillColor;
            }
        }

        if (_slidersHandleImage.Length > 0)
        {
            foreach (var handle in _slidersHandleImage)
            {
                handle.color = theme.SliderHandleColor;
            }
        }
    }

    private void ApplyInputFieldsColor(ThemeConfig theme)
    {
        /*if (_inputFields == null || _inputFields.Length == 0)
        {
            return;
        }*/

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
