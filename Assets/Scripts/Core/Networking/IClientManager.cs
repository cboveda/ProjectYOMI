using System;

public interface IClientManager
{
    void StartClient(string joinCode, Action<string> callback);
}