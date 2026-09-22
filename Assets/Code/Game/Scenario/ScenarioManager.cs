using System;
using System.Collections.Generic;
using BennyKok.RuntimeDebug.Actions;
using BennyKok.RuntimeDebug.Attributes;
using BennyKok.RuntimeDebug.Systems;
using Unity.Netcode;
using VInspector;

public class ScenarioManager : Singleton<ScenarioManager>, ISaveable
{
    private ScenarioData data;
    private Scenario currentScenario;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private BaseDebugAction[] actions;
#endif

    public event Action<ScenarioData> OnScenarioDataChanged;
    public event Action<Scenario> OnScenarioSelected;
    public event Action<Scenario> OnScenarioDeleted;
    public event Action OnScenarioUpdated;

    public event Action<bool> OnDirtyChanged;
    public bool IsDirty { get; private set; }

    public WeatherType CurrentWeather => currentScenario?.WeatherType ?? default;

    private void OnEnable()
    {
        var saveLoadSystem = SaveLoadSystem.Instance;
        if (!saveLoadSystem) return;

        saveLoadSystem.Register(this);
        saveLoadSystem.OnGameSaved += HandleGameSaved;
    }

    private void OnDisable()
    {
        if (!SaveLoadSystem.HasInstance) return;

        var saveLoadSystem = SaveLoadSystem.Instance;
        saveLoadSystem.OnGameSaved -= HandleGameSaved;
        saveLoadSystem.Unregister(this);
    }

    private void Start()
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer) return;

        var session = SessionManager.Instance;
        if (!session) return;

        foreach (var player in session.Players)
        {
            PushDefaultWeather(player);
        }

        session.OnPlayerRegistered += PushDefaultWeather;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        actions = RuntimeDebugSystem.RegisterActionsAuto(this);
#endif
    }

    private void OnDestroy()
    {
        if (SessionManager.Instance)
        {
            SessionManager.Instance.OnPlayerRegistered -= PushDefaultWeather;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        RuntimeDebugSystem.UnregisterActions(actions);
#endif
    }

    public void Save(GameData gameData)
    {
        gameData.ScenarioData = data;
    }

    public void Load(GameData gameData)
    {
        data = gameData.ScenarioData ??= new ScenarioData();
        data.Scenarios ??= new List<Scenario>();

        OnScenarioDataChanged?.Invoke(data);
        SetDirty(false);
    }

    public void AddScenario(string scenarioName)
    {
        data.Scenarios.Add(new Scenario(scenarioName));

        OnScenarioDataChanged?.Invoke(data);
        SetDirty(true);
    }

    public void DeleteScenario(Scenario scenario)
    {
        if (!data.Scenarios.Remove(scenario)) return;

        if (currentScenario == scenario) Deselect();

        OnScenarioDeleted?.Invoke(scenario);
        OnScenarioDataChanged?.Invoke(data);
        SetDirty(true);
    }

    public void Select(Scenario scenario)
    {
        if (!CanSelect()) return;

        Deselect();

        currentScenario = scenario;

        if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer)
        {
            foreach (var player in SessionManager.Instance.Players)
            {
                player.View.SetDefaultWeather(currentScenario.WeatherType);
            }
        }

        OnScenarioSelected?.Invoke(currentScenario);
    }

    public void SetGlobalWeather(WeatherType weather)
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer) return;
        if (currentScenario == null) return;

        currentScenario.WeatherType = weather;

        foreach (var player in SessionManager.Instance.Players)
        {
            player.View.SetDefaultWeather(weather);
        }

        OnScenarioUpdated?.Invoke();
    }

    public void SetNavigation(ulong clientId, NetworkObjectReference target)
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer) return;

        if (SessionManager.Instance.TryGetPlayer(clientId, out var player))
        {
            player.View.SetNavigation(target);
        }

        OnScenarioUpdated?.Invoke();
    }

    private bool CanSelect()
    {
        var missionManager = MissionManager.Instance;
        if (missionManager)
        {
            return missionManager.IsMissionRunning;
        }

        return true;
    }

    private void Deselect()
    {
        currentScenario = null;
    }

    private void PushDefaultWeather(Player player) => player.View.SetDefaultWeather(CurrentWeather);

    private void HandleGameSaved() => SetDirty(false);

    private void SetDirty(bool value)
    {
        if (IsDirty == value) return;

        IsDirty = value;
        OnDirtyChanged?.Invoke(value);
    }

#if UNITY_EDITOR
    [Button] private void TestAddScenario() => AddScenario($"{data.Scenarios.Count}");

    [Button]
    private void TestRemoveLastScenario()
    {
        if (data.Scenarios.Count == 0) return;
        DeleteScenario(data.Scenarios[^1]);
    }
#endif

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [DebugAction] public void TestServerSetGlobalWeather(int weatherType) => SetGlobalWeather((WeatherType)weatherType);
#endif
}
