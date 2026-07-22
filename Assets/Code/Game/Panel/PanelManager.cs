using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

public sealed class PanelManager : Singleton<PanelManager>
{
    [Scene, SerializeField] private UIPanel[] panels;

    private Dictionary<PanelId, UIPanel> panelsById;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    protected override void Awake()
    {
        base.Awake();
        InitRegistry();
    }

    public void Activate(PanelId id) => GetPanel(id).Activate();
    public void Deactivate(PanelId id) => GetPanel(id).Deactivate();

    public void ActivateExclusive(PanelId id)
    {
        var target = GetPanel(id);

        foreach (var panel in panelsById.Values)
        {
            if (panel != target)
            {
                panel.Deactivate();
            }
        }

        target.Activate();
    }

    private void InitRegistry()
    {
        panelsById = new Dictionary<PanelId, UIPanel>(panels.Length);

        foreach (var panel in panels)
        {
            if (!panel) continue;

            if (!panelsById.TryAdd(panel.Id, panel))
            {
                Debug.LogError($"Duplicated PanelId found: '{panel.Id}' on '{panel.name}'", panel);
            }
        }
    }

    private UIPanel GetPanel(PanelId id)
    {
        if (!panelsById.TryGetValue(id, out var panel))
        {
            Debug.LogError($"No panel found with id: '{id}'");
        }

        return panel;
    }
}
