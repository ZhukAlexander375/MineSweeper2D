using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "Create Theme")]

public class ThemeConfig : ScriptableObject
{
    [Header("Colors")]
    public Color MenuFrameBGColor;
    public Color MenuFrameShadowColor;
    public Color ButtonsMainColor;
    public Color ButtonsMainShadowColor;
    public Color ButtonsMainTextColor;
    public Color ButtonsMinorColor;
    public Color ButtonsMinorShadowColor;
    public Color ButtonsMinorTextColor;

    [Header("Images")]
    public Sprite NavigationPanel;
    public Sprite MenuTitleImage;


    public Color TopFieldColor;
    //public Color IconsColor;
    //public Color TransparentBGColor;    
    //public Color MenuTitleTextColor; 
    
    //public Color TextsOnButtons;
    //public Color ButtonsTextColorLight;
    public Color CameraBackground;
}