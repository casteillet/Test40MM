using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

public sealed class PanelManager : Singleton<PanelManager>
{
    [Scene, SerializeField] private Panel[] panels;

    private Dictionary<PanelId, Panel> panelsById;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    protected override void Awake()
    {
        base.Awake();
        InitRegistry();
    }

    public void Activate(PanelId id)
    {
        if (!TryGetPanel(id, out var panel)) return;

        panel.Activate();
    }

    public void Deactivate(PanelId id)
    {
        if (!TryGetPanel(id, out var panel)) return;

        panel.Deactivate();
    }

    public void ActivateExclusive(PanelId id)
    {
        if (!TryGetPanel(id, out var target)) return;

        panelsById.Values.ActivateExclusive(target);
    }

    private void InitRegistry()
    {
        panelsById = new Dictionary<PanelId, Panel>(panels.Length);

        foreach (var panel in panels)
        {
            if (!panel) continue;

            if (!panelsById.TryAdd(panel.Id, panel))
            {
                Debug.LogError($"Duplicated PanelId found: '{panel.Id}' on '{panel.name}'", panel);
            }
        }
    }

    private bool TryGetPanel(PanelId id, out Panel panel)
    {
        if (panelsById.TryGetValue(id, out panel)) return true;

        Debug.LogError($"No panel found with id: '{id}'", this);
        return false;
    }
}
