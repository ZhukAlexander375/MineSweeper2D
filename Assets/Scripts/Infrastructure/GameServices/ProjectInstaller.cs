using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallServices();
    }

    private void InstallServices()
    {

        //RewardManagerInstaller.Install(Container); // Добавляем RewardManager
        // В будущем сюда добавятся другие сервисы (SaveManager, GameManager и т. д.)
        Debug.Log("отработало?");
    }
}
