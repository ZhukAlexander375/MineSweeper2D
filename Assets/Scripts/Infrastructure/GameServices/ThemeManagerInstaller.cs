using UnityEngine;
using Zenject;

public class ThemeManagerInstaller : MonoInstaller
{
    [SerializeField] private ThemeManager _themeManagerPrefab;

    public override void InstallBindings()
    {
        if (_themeManagerPrefab == null)
        {
            return;
        }

        BindThemeManager();        
    }

    private void BindThemeManager()
    {
        var themeManager = Container.InstantiatePrefabForComponent<ThemeManager>(_themeManagerPrefab);
        DontDestroyOnLoad(themeManager.gameObject);

        Container.Bind<ThemeManager>()
                    .FromInstance(themeManager)
                    .AsSingle()
                    .NonLazy();
    }
}
