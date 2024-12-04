using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "Create Theme")]

public class ThemeConfig : ScriptableObject
{
    [Header("Frames Colors")]
    public Color MenuFrameBGColor;
    public Color MenuFrameShadowColor;

    [Header("Buttons Colors")]
    public Color ButtonsMainColor;
    public Color ButtonsMainShadowColor;    
    public Color ButtonsMinorColor;
    public Color ButtonsMinorShadowColor;    

    [Header("Backgrounds Colors")]
    public Color CameraBackground;
    public Color MainBackgroundColor;
    public Color TransparentBackgroundColor;
    public Color TopFieldColor;

    [Header("Icons Colors")]
    public Color IconsSettingsBackColor;

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