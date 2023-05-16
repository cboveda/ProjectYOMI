using Unity.Netcode;
using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField] private ScriptableObject[] _objects;

    public override void InstallBindings()
    {
        foreach(var obj in _objects) 
        { 
            Container.QueueForInject(obj);
        }

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
