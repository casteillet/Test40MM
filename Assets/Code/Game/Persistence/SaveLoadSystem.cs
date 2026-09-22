using System;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;

[Serializable]
public class GameData
{
    public string Name;
    public ScenarioData ScenarioData;
}

public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem>
{
    [SerializeField] public GameData gameData;

    private const string GameDataName = "Game";

    private readonly HashSet<ISaveable> saveables = new();
    private IDataService dataService;

    public event Action OnGameSaved;
    public event Action OnGameLoaded;
    public event Action OnGameDeleted;

    protected override void Awake()
    {
        base.Awake();
        dataService = new FileDataService(new JsonSerializer());

        if (!LoadGame())
        {
            NewGame();
        }
    }

    public void Register(ISaveable saveable)
    {
        if (!saveables.Add(saveable)) return;
        if (gameData != null) saveable.Load(gameData);
    }

    public void Unregister(ISaveable saveable) => saveables.Remove(saveable);

    public void NewGame()
    {
        gameData = new GameData
        {
            Name = GameDataName,
            ScenarioData = new ScenarioData()
        };

        PushToSaveables();
    }

    public void SaveGame()
    {
        gameData.Name = GameDataName;

        foreach (var saveable in saveables)
        {
            saveable.Save(gameData);
        }

        dataService.Save(gameData);
        OnGameSaved?.Invoke();
    }

    public bool LoadGame()
    {
        var loaded = dataService.Load(GameDataName);
        if (loaded == null) return false;

        gameData = loaded;

        PushToSaveables();
        OnGameLoaded?.Invoke();
        return true;
    }

    public void ReloadGame() => LoadGame();

    public void DeleteGame()
    {
        dataService.Delete(GameDataName);
        OnGameDeleted?.Invoke();
    }

    private void PushToSaveables()
    {
        foreach (var saveable in saveables)
        {
            saveable.Load(gameData);
        }
    }
}
