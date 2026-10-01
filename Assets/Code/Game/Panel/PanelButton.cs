using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class PanelButton : ValidatedMonoBehaviour
{
    [SerializeField] private PanelId targetPanelId;

    [Self, SerializeField] private Button button;

    private void OnEnable() => button.onClick.AddListener(ActivateTargetPanel);
    private void OnDisable() => button.onClick.RemoveListener(ActivateTargetPanel);

    private void ActivateTargetPanel() => PanelManager.Instance.ActivateExclusive(targetPanelId);
}
