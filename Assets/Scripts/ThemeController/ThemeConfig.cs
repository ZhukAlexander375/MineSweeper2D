using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "Create Theme")]

public class ThemeConfig : ScriptableObject
{
    [Header("Frames Colors")]
    public Color MenuFrameBGColor;
    public Color MenuFrameShadowColor;
    public Color MenuFrameSecondBGColor;
    public Color MenuFrameSecondShadow2Color;
    public Color EnabledSettingColor;
    public Color DisabledSettingColor;

    [Header("Buttons Colors")]
    public Color ButtonsMainColor;
    public Color ButtonsMainShadowColor;    
    public Color ButtonsInactiveColor;
    public Color ButtonsInactiveShadowColor;

    [Header("Dropdowns Colors")]
    public Color DropdownColor;
    public Color DropdownShadowColor;

    [Header("Backgrounds Colors")]
    public Color CameraBackground;
    public Color MainBackgroundColor;
    public Color TransparentBackgroundColor;
    public Color TopFieldColor;
    public Color CloseSectorBackgroundColor;

    [Header("Icons Colors")]
    public Color IconsSettingsBackColor;
    public Color IconsOnActiveButtonColor;
    public Color IconsOnInactiveButtonColor;

    [Header("Sliders Colors")]
    public Color SliderFillColor;
    public Color SliderHandleColor;

    [Header("Texts Colors")]
    public Color MenuTitleTextColor;
    public Color MainMenuTextsColor;
    public Color ButtonsMainTextColor;
    public Color ButtonsMinorTextColor;

    [Header("Images")]
    public Sprite NavigationPanel;
    public Sprite MenuTitleImage;


    
    //       
    //public Color MenuTitleTextColor; 
    
    //public Color TextsOnButtons;
    //public Color ButtonsTextColorLight;
}