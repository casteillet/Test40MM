using UnityEngine;
using UnityUtils;

public class ScenarioListView : MonoBehaviour, IPanelObserver
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;

    public void OnPanelActivated()
    {
        ScenarioManager.Instance.OnScenarioDataChanged += HandleDataChanged;
        Rebuild(ScenarioManager.Instance.CurrentData);
    }

    public void OnPanelDeactivated()
    {
        if (!ScenarioManager.HasInstance) return;

        ScenarioManager.Instance.OnScenarioDataChanged -= HandleDataChanged;
    }

    private void HandleDataChanged(ScenarioData data) => Rebuild(data);

    private void Rebuild(ScenarioData data)
    {
        buttonParent.DestroyChildren();
        if (data == null) return;

        data.Scenarios.ForEach(SpawnButton);
    }

    private void SpawnButton(Scenario scenario)
    {
        var instance = Instantiate(buttonPrefab, buttonParent);
        instance.GetComponent<ScenarioButton>().Initialize(scenario);
    }
}
