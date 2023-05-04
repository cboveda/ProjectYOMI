using UnityEngine;
using Zenject;

public class MainMenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IClientManager>()
            .To<ClientManager>()
            .FromComponentInNewPrefabResource("ClientManager")
            .AsSingle()
            .NonLazy();
    }
}