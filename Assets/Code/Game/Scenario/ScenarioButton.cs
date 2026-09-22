using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioButton : ValidatedMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [Self, SerializeField] private Button button;

    public void Initialize(Scenario scenario)
    {
        text.text = scenario.Name;
        button.onClick.AddListener(() => ScenarioManager.Instance.Select(scenario));
    }
}
