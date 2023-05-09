using Unity.Netcode;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<NetworkManager>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();

        Container.Bind<IServerManager>()
            .To<ServerManager>()
            .FromComponentInNewPrefabResource("ServerManager")
            .AsSingle()
            .NonLazy();

        Container.Bind<IDatabase>()
            .To<Database>()
            .FromComponentInNewPrefabResource("Database")
            .AsSingle()
            .NonLazy();
    }
}
