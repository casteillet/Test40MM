using Unity.Netcode;

public class LobbyManager : Singleton<LobbyManager>
{
    public void AssignSpawn(string playerId, SpawnPosition spawnPosition)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        SessionManager.Instance.AssignSpawn(playerId, spawnPosition);
    }

    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (!SessionManager.Instance.AllPlayersReady()) return;

        NetworkSceneManager.Instance.LoadGameAsServer();
    }
}