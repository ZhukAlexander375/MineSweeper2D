using UnityEngine;
using Zenject;


public class RewardManagerInstaller : MonoInstaller
{
    [SerializeField] private RewardConfig _rewardConfig;
    [SerializeField] private RewardManager _rewardManagerPrefab;

    public override void InstallBindings()
    {
        if (_rewardManagerPrefab == null || _rewardConfig == null)
        {
            return;
        }

        BindRewardConfig();
        BindRewardManager();        
    }

    private void BindRewardConfig()
    {
        Container.Bind<RewardConfig>().FromInstance(_rewardConfig).AsSingle();
    }

    private void BindRewardManager()
    {
        var rewardManager = Container.InstantiatePrefabForComponent<RewardManager>(_rewardManagerPrefab);
        DontDestroyOnLoad(rewardManager.gameObject);

        Container.Bind<RewardManager>()
                    .FromInstance(rewardManager)
                    .AsSingle()
                    .NonLazy();
    }
}
