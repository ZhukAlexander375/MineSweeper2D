using UnityEngine;
using Zenject;

public class ClassicMinesweeperInstaller : MonoInstaller
{
    [SerializeField] private Board _board;
    [SerializeField] private SimpleGridManager _gridManager;
    [SerializeField] private CameraController _cameraController;

    public override void InstallBindings()
    {
        // Биндим объекты, которые уже есть на сцене
        Container.Bind<Board>().FromInstance(_board).AsSingle();
        Container.Bind<SimpleGridManager>().FromInstance(_gridManager).AsSingle();
        Container.Bind<CameraController>().FromInstance(_cameraController).AsSingle();
    }
}
