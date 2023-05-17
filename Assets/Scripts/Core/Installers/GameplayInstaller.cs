using Zenject;

public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
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

        Container.Bind<CombatEvaluator>()
            .FromNew()
            .AsSingle();
    }
}
