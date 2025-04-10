using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;

public class SceneLoaderInstaller : MonoInstaller
{
    [SerializeField] private ScenesConfig _scenesConfig;
    [SerializeField] private SceneLoader _sceneLoaderPrefab;

    public override void InstallBindings()
    {
        if (_sceneLoaderPrefab == null || _scenesConfig == null)
        {
            return;
        }

        BindScenesConfig();
        BindSceneLoader();
    }

    private void BindScenesConfig()
    {
        Container.Bind<ScenesConfig>().FromInstance(_scenesConfig).AsSingle();
    }

    private void BindSceneLoader()
    {
        var sceneLoader = Container.InstantiatePrefabForComponent<SceneLoader>(_sceneLoaderPrefab);
        DontDestroyOnLoad(sceneLoader.gameObject);

        Container.Bind<SceneLoader>()
                    .FromInstance(sceneLoader)
                    .AsSingle()
                    .NonLazy();
    }
}
