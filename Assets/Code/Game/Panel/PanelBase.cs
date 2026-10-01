using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasGroup))]
public abstract class PanelBase : ValidatedMonoBehaviour, IActivatable
{
    [Self, SerializeField] private Canvas canvas;
    [Self, SerializeField] private GraphicRaycaster raycaster;
    [Self, SerializeField] private CanvasGroup canvasGroup;

    private readonly List<IPanelObserver> ownedObservers = new();
    private bool hasAppliedActiveState;

    public event Action<bool> ActiveStateChanged;

    public bool IsActive { get; private set; }

    protected virtual void Awake()
    {
        InitOwnedObservers();
    }

    public void Activate() => SetActiveState(true);
    public void Deactivate() => SetActiveState(false);

    protected void InitializeActiveState(bool active)
    {
        if (hasAppliedActiveState) return;

        SetActiveState(active);
    }

    private void SetActiveState(bool active)
    {
        if (hasAppliedActiveState && IsActive == active) return;

        hasAppliedActiveState = true;
        IsActive = active;

        ApplyVisibility(active);
        NotifyObservers(active);
        ActiveStateChanged?.Invoke(active);
    }

    private void ApplyVisibility(bool visible)
    {
        canvas.enabled = visible;
        raycaster.enabled = visible;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    private void InitOwnedObservers()
    {
        foreach (var observer in GetComponentsInChildren<IPanelObserver>(true))
        {
            if (IsOwnedByThisPanel(observer))
            {
                ownedObservers.Add(observer);
            }
        }
    }

    private bool IsOwnedByThisPanel(IPanelObserver observer)
    {
        return observer is Component component && component.GetComponentInParent<PanelBase>(true) == this;
    }

    private void NotifyObservers(bool active)
    {
        foreach (var observer in ownedObservers)
        {
            if (active)
            {
                observer.OnPanelActivated();
            }
            else
            {
                observer.OnPanelDeactivated();
            }
        }
    }
}
