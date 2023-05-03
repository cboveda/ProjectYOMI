using Zenject;

public class GlobalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IServerManager>()
            .To<ServerManager>()
            .FromComponentInNewPrefabResource("ServerManager")
            .AsSingle()
            .NonLazy();
    }
}
