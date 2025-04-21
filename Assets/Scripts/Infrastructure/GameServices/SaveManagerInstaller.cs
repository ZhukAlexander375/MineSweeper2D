using UnityEngine;
using Zenject;

public class SaveManagerInstaller : MonoInstaller
{
    [SerializeField] private SaveManager _saveManagerPrefab;

    public override void InstallBindings()
    {
        if (_saveManagerPrefab == null)
        {
            return;
        }

        BindSaveManager();        
    }

    private void BindSaveManager()
    {
        var saveManager = Container.InstantiatePrefabForComponent<SaveManager>(_saveManagerPrefab);
        DontDestroyOnLoad(saveManager.gameObject);

        Container.Bind<SaveManager>()
                    .FromInstance(saveManager)
                    .AsSingle()
                    .NonLazy();
    }
}
