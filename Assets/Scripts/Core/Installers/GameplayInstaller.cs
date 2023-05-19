using Zenject;

public class GameplayInstaller : MonoInstaller
{
    public CombatConfiguration Configuration;

    public override void InstallBindings()
    {
        Container.BindInstance(Configuration);

        Container.Bind<IGameUIManager>()
            .To<GameUIManager>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();

        Container.Bind<ICameraFocusObject>()
            .To<CameraFocusObject>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();

        Container.Bind<ITurnHistory>()
            .To<TurnHistory>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container.Bind<IPlayerDataCollection>()
            .To<PlayerDataCollection>()
            .FromNew()
            .AsSingle();

        Container.Bind<CombatCommandExecutor>()
            .FromNew()
            .AsSingle();

        Container.Bind<TurnFactory>()
            .FromNew()
            .AsSingle();
    }
}
