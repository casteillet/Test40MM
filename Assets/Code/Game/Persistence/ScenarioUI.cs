using UnityEngine;
using UnityUtils;

public class ScenarioUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;

    private ScenarioData data;
    
    private void OnEnable()
    {
        ScenarioManager.Instance.OnScenarioDataChanged += OnScenarioDataChanged;
    }

    private void OnDisable()
    {
        if (ScenarioManager.Instance)
        {
            ScenarioManager.Instance.OnScenarioDataChanged -= OnScenarioDataChanged;
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
}
