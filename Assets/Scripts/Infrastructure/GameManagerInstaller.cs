using UnityEngine;
using Zenject;

public class GameManagerInstaller : MonoInstaller
{
    [SerializeField] private GameManager _gameManagerPrefab;

    [SerializeField] private SimpleInfiniteStatisticController _simpleInfiniteStatisticController;
    [SerializeField] private HardcoreStatisticController _hardcoreStatisticController;
    [SerializeField] private TimeTrialStatisticController _timeTrialStatisticController;
    [SerializeField] private ClassicModeStatisticController _classicStatisticController;

    public override void InstallBindings()
    {
        if (_gameManagerPrefab == null)
        {
            return;
        }

        BindControllers();
        BindGameManager();

        // Создаём GameManager как Singleton
        //Container.Bind<GameManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }

    private void BindControllers()
    {
        // Биндим контроллеры статистики как Singleton'ы
        Container.Bind<SimpleInfiniteStatisticController>().FromInstance(_simpleInfiniteStatisticController).AsSingle();
        Container.Bind<HardcoreStatisticController>().FromInstance(_hardcoreStatisticController).AsSingle();
        Container.Bind<TimeTrialStatisticController>().FromInstance(_timeTrialStatisticController).AsSingle();
        Container.Bind<ClassicModeStatisticController>().FromInstance(_classicStatisticController).AsSingle();
    }

    private void BindGameManager()
    {
        var gameManager = Container.InstantiatePrefabForComponent <GameManager>(_gameManagerPrefab);
        DontDestroyOnLoad(gameManager.gameObject);

        Container.Bind<GameManager>()
                    .FromInstance(gameManager)
                    .AsSingle()
                    .NonLazy();
    }
}
