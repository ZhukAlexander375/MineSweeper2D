using UnityEngine;
using Zenject;

public class TimeModeTimerManagerInstaller : MonoInstaller
{
    [SerializeField] private TimeTrialSettings _timeTrialSettings;
    [SerializeField] private TimeModeTimerManager _timeModeTimerManagerPrefab;

    public override void InstallBindings()
    {
        if (_timeModeTimerManagerPrefab == null || _timeTrialSettings == null)
        {
            return;
        }

        BindTimeTrialSettings();
        BindTimeModeManager();
    }

    private void BindTimeTrialSettings()
    {
        Container.Bind<TimeTrialSettings>().FromInstance(_timeTrialSettings).AsSingle();
    }

    private void BindTimeModeManager()
    {
        var timeModeTimerManager = Container.InstantiatePrefabForComponent<TimeModeTimerManager>(_timeModeTimerManagerPrefab);
        DontDestroyOnLoad(timeModeTimerManager.gameObject);

        Container.Bind<TimeModeTimerManager>()
                    .FromInstance(timeModeTimerManager)
                    .AsSingle()
                    .NonLazy();
    }
}
