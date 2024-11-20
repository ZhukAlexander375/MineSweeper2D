using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "Create Theme")]

public class ThemeConfig : ScriptableObject
{
    [Header("Colors")]
    public Color MainBackgroundColor;
    public Color MainTitleColor;
    public Color IconsColor;
    public Color TitleTextColor;
    public Color SettingWindowsColor;
    public Color ButtonsColor;
    public Color ButtonsTextColorDark;
    public Color ButtonsTextColorLight;
}