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

        //RewardManagerInstaller.Install(Container); // ��������� RewardManager
        // � ������� ���� ��������� ������ ������� (SaveManager, GameManager � �. �.)
        Debug.Log("����������?");
    }
}
