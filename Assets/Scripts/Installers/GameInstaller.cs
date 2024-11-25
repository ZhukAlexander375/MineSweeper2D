using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private ThemeManager _themeManagerPrefab;
    public override void InstallBindings()
    {
        BindThemeController();
    }

    private void BindThemeController()
    {
        if (_themeManagerPrefab != null)
        {
            ThemeManager themeManager = Container.InstantiatePrefabForComponent<ThemeManager>(_themeManagerPrefab);
            Container.BindInstance(themeManager).AsSingle().NonLazy();
        }
    }
}
