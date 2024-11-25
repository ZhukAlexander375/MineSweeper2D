
public struct ThemeChangeSignal 
{
    public ThemeConfig Theme;
    public int ThemeIndex;

    public ThemeChangeSignal(ThemeConfig theme, int index)
    {
        Theme = theme;
        ThemeIndex = index;
    }
}

