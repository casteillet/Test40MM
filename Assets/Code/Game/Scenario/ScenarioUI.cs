using TMPro;
using UnityEngine;
using UnityUtils;

public class ScenarioUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;
    
    [SerializeField] private TextMeshProUGUI scenarioText;
    [SerializeField] private TextMeshProUGUI weatherText;

    private ScenarioData data;
    private Scenario scenario;
    
    private void OnEnable()
    {
        ScenarioManager.Instance.OnScenarioDataChanged += OnScenarioDataChanged;
        
        ScenarioManager.Instance.OnScenarioLoaded += OnScenarioLoaded;
        ScenarioManager.Instance.OnScenarioUpdated += OnScenarioUpdated;
    }
    
    private void OnDisable()
    {
        if (ScenarioManager.Instance)
        {
            ScenarioManager.Instance.OnScenarioDataChanged -= OnScenarioDataChanged;
            
            ScenarioManager.Instance.OnScenarioLoaded -= OnScenarioLoaded;
            ScenarioManager.Instance.OnScenarioUpdated -= OnScenarioUpdated;
        }
    }

    // private void Start()
    // {
    //     if (ScenarioManager.Instance.Data == null) return;
    //     
    //     OnScenarioDataChanged(ScenarioManager.Instance.Data);
    // }

    private void OnScenarioDataChanged(ScenarioData data)
    {
        this.data = data;
        SpawnButtonList();
    }
    
    private void OnScenarioLoaded(Scenario scenario)
    {
        this.scenario = scenario;
        UpdateScenario();
    }

    private void OnScenarioUpdated() => UpdateScenario();

    private void UpdateScenario()
    {
        SetScenarioText();
        SetWeatherText();
    }
    
    private void SpawnButtonList()
    {
        buttonParent.DestroyChildren();
        data.Scenarios.ForEach(SpawnButton);
    }
    
    private void SpawnButton(Scenario scenario)
    {
        var instance = Instantiate(buttonPrefab, buttonParent);
        var button = instance.GetComponent<ScenarioButton>();
        button.Initialize(scenario);
    }

    private void SetScenarioText() => scenarioText.text = scenario.Name;
    private void SetWeatherText() => weatherText.text = scenario.WeatherType.ToString();
}
