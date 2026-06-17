using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    private IDataService dataService;
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    protected override void Awake()
    {
        base.Awake();
        dataService = new FileDataService(new JsonSerializer());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu") return;

        if (!LoadGame(GameDataName))
        {
            NewGame();
        }
        
        BindDatas();
    }
    
    private void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
    {
        var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
        if (!entity) return;
        
        data ??= new TData { Id = entity.Id };
        entity.Bind(data);
    }

    private void Bind<T, TData>(List<TData> datas) where T: MonoBehaviour, IBind<TData> where TData : ISaveable, new()
    {
        var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

        foreach(var entity in entities)
        {
            var data = datas.FirstOrDefault(d=> d.Id == entity.Id);
            
            if (data == null)
            {
                data = new TData { Id = entity.Id };
                datas.Add(data); 
            }
            
            entity.Bind(data);
        }
    }

    public void NewGame()
    {
        gameData = new GameData()
        {
            Name = GameDataName,
            ScenarioData = new ScenarioData()
        };
    }
    
    public void SaveGame()
    {
        gameData.Name = GameDataName;
        dataService.Save(gameData);
        
        Debug.Log($"Save Game");
    }

    public bool LoadGame(string gameName)
    {
        gameData = dataService.Load(gameName);
        Debug.Log($"Load Game: {gameData} with {gameName}");
        return gameData != null;
    }

    public void BindDatas()
    {
        Bind<ScenarioManager, ScenarioData>(gameData.ScenarioData);
    }

    public void ReloadGame() => LoadGame(GameDataName);
    public void DeleteGame(string gameName) => dataService.Delete(gameName);

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}