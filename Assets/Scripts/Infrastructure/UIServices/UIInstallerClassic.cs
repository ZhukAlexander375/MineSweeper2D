using UnityEngine;
using Zenject;

public class UIInstallerClassic : MonoInstaller
{
    [SerializeField] private GameUIClassic _gameUIClassic;
    [SerializeField] private PopupUIClassic _popupUIClassic;
    [SerializeField] private OverUIClassic _overUIClassic;

    public override void InstallBindings()
    {
        Container
            .BindInterfacesAndSelfTo<UIServiceClassic>()
            .AsSingle()
            .WithArguments(_gameUIClassic, _popupUIClassic, _overUIClassic);
    }
}
