using UnityEngine;
using Zenject;

public class PlayerProgressInstaller : MonoInstaller
{
    [SerializeField] private PlayerProgress _playerProgressPrefab;

    public override void InstallBindings()
    {
        if (_playerProgressPrefab == null)
        {
            return;
        }

        BindPlayerProgress();
    }

    private void BindPlayerProgress()
    {
        var playerProgress = Container.InstantiatePrefabForComponent<PlayerProgress>(_playerProgressPrefab);
        DontDestroyOnLoad(playerProgress.gameObject);

        Container.Bind<PlayerProgress>()
                    .FromInstance(playerProgress)
                    .AsSingle()
                    .NonLazy();
    }
}
