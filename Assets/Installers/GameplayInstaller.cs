using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("Installing!");
        Container.Bind<IGameUIManager>()
            .To<GameUIManager>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
    }
}