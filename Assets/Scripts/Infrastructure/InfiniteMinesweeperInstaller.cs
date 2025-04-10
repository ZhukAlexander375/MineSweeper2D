using UnityEngine;
using Zenject;

public class InfiniteMinesweeperInstaller : MonoInstaller
{
    [SerializeField] private InfiniteGridManager _gridManager;
    [SerializeField] private CameraController _cameraController;

    public override void InstallBindings()
    {
        // Биндим объекты, которые уже есть на сцене        
        Container.Bind<InfiniteGridManager>().FromInstance(_gridManager).AsSingle();
        Container.Bind<CameraController>().FromInstance(_cameraController).AsSingle();
    }
}
