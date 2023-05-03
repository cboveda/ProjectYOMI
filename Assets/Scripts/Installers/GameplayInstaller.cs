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
    }
}
