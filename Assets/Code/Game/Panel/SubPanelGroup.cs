using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

public sealed class SubPanelGroup : ValidatedMonoBehaviour, IPanelObserver
{
    [Anywhere, SerializeField] private SubPanel startSubPanel;

    private readonly List<SubPanel> ownedSubPanels = new();
    private bool isOwnerPanelActive;

    private void Awake()
    {
        CollectOwnedSubPanels();
        ValidateStartSubPanel();
    }

    public void ActivateExclusive(SubPanel target)
    {
        if (!isOwnerPanelActive) return;

        ownedSubPanels.ActivateExclusive(target);
    }

    void IPanelObserver.OnPanelActivated()
    {
        isOwnerPanelActive = true;
        ownedSubPanels.ActivateExclusive(startSubPanel);
    }

    void IPanelObserver.OnPanelDeactivated()
    {
        isOwnerPanelActive = false;
        ownedSubPanels.DeactivateAll();
    }

    private void CollectOwnedSubPanels()
    {
        foreach (var subPanel in GetComponentsInChildren<SubPanel>(true))
        {
            if (subPanel.OwnerGroup == this)
            {
                ownedSubPanels.Add(subPanel);
            }
        }
    }

    private void ValidateStartSubPanel()
    {
        if (ownedSubPanels.Contains(startSubPanel)) return;

        Debug.LogError($"Start sub panel '{startSubPanel}' is not owned by '{name}'", this);
    }
}
