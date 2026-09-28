using TMPro;
using UnityEngine;

public class ScenarioEditorView : MonoBehaviour, IPanelObserver
{
    [SerializeField] private TextMeshProUGUI scenarioText;
    [SerializeField] private TextMeshProUGUI weatherText;

    public void OnPanelActivated()
    {
        ScenarioManager.Instance.OnScenarioUpdated += Refresh;
        Refresh();
    }

    public void OnPanelDeactivated()
    {
        if (!ScenarioManager.HasInstance) return;

        ScenarioManager.Instance.OnScenarioUpdated -= Refresh;
    }

    private void Refresh()
    {
        var scenario = ScenarioManager.Instance.CurrentScenario;
        if (scenario == null) return;

        scenarioText.text = scenario.Name;
        weatherText.text = scenario.WeatherType.ToString();
    }
}
