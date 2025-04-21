using UnityEngine;
using Zenject;

public class GameSettingsInstaller : MonoInstaller
{
    [SerializeField] private GameSettingsConfig _defaultSettingsConfig;

    public override void InstallBindings()
    {

        if (_defaultSettingsConfig == null)
        {
            return;
        }

        BindDefaultSettings();
        BindGameSettingsManager();
    }

    private void BindDefaultSettings()
    {
        //Container.BindInstance(_defaultSettingsConfig).AsSingle();

        Container.Bind<GameSettingsConfig>().FromInstance(_defaultSettingsConfig).AsSingle();
    }

    private void BindGameSettingsManager()
    {
        Container.BindInterfacesAndSelfTo<GameSettingsManager>().AsSingle();

        /*Container.Bind<SceneLoader>()
                    .FromComponentInNewPrefab(_sceneLoaderPrefab)
                    .AsSingle()
                    .NonLazy();*/
    }
}
