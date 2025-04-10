using Zenject;

public class SignalBusInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //Container.Bind<SignalBus>().AsSingle().NonLazy();
    }
}
