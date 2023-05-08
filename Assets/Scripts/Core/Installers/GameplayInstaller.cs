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

        Container.Bind<ITurnHistory>()
            .To<TurnHistory>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container.Bind<PlayerDataCollection>()
            .FromNew()
            .AsSingle();

        Container.Bind<CombatEvaluator>()
            .FromNew()
            .AsSingle();
    }
}
