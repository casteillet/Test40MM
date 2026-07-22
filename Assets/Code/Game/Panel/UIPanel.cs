using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasGroup))]
public sealed class UIPanel : ValidatedMonoBehaviour, IActivatable
{
    [SerializeField] private PanelId id;
    [SerializeField] private bool activeOnStart;

    [Self, SerializeField] private Canvas canvas;
    [Self, SerializeField] private GraphicRaycaster raycaster;
    [Self, SerializeField] private CanvasGroup canvasGroup;
    [Child(Flag.IncludeInactive | Flag.Optional), SerializeField] private List<InterfaceRef<IPanelObserver>> observers = new();

    public PanelId Id => id;
    public bool IsActive { get; private set; }

    private void Start()
    {
        ApplyState(activeOnStart);
        NotifyObservers(IsActive);
    }

    public void Activate() => SetVisibility(true);
    public void Deactivate() => SetVisibility(false);

    private void SetVisibility(bool active)
    {
        if (IsActive == active) return;

        ApplyState(active);
        NotifyObservers(active);
    }

    private void ApplyState(bool active)
    {
        IsActive = active;
        
        canvas.enabled = active;
        raycaster.enabled = active;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }

    private void NotifyObservers(bool active)
    {
        foreach (var observer in observers)
        {
            if (active)
            {
                observer.Value.OnPanelActivated();
            }
            else
            {
                observer.Value.OnPanelDeactivated();
            }
        }
    }
    
#if UNITY_EDITOR
    [VInspector.Button] private void Switch() => PanelManager.Instance.ActivateExclusive(id);
#endif
}
