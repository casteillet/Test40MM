using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class SubPanelButton : ValidatedMonoBehaviour
{
    [SerializeField] private SubPanel targetSubPanel;
    [SerializeField] private Graphic activeHighlight;

    [Self, SerializeField] private Button button;

    private void OnEnable()
    {
        RefreshHighlight(targetSubPanel.IsActive);
        targetSubPanel.ActiveStateChanged += RefreshHighlight;
        button.onClick.AddListener(ActivateTargetSubPanel);
    }

    private void OnDisable()
    {
        targetSubPanel.ActiveStateChanged -= RefreshHighlight;
        button.onClick.RemoveListener(ActivateTargetSubPanel);
    }

    private void ActivateTargetSubPanel() => targetSubPanel.OwnerGroup.ActivateExclusive(targetSubPanel);

    private void RefreshHighlight(bool isTargetActive)
    {
        if (!activeHighlight) return;

        activeHighlight.enabled = isTargetActive;
    }
}
