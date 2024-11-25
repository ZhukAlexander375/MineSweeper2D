using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "Create Theme")]

public class ThemeConfig : ScriptableObject
{
    [Header("Colors")]
    public Color TopFieldColor;
    public Color IconsColor;
    public Color TransparentBGColor;
    public Color MenuFrameBGColor;    
    public Color MenuTitleTextColor;    
    public Color ButtonsColor;
    public Color TextsOnButtons;
    //public Color ButtonsTextColorLight;
    public Color CameraBackground;
}