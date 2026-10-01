using UnityEngine;

public sealed class Panel : PanelBase
{
    [SerializeField] private PanelId id;
    [SerializeField] private bool activeOnStart;

    public PanelId Id => id;

    private void Start()
    {
        InitializeActiveState(activeOnStart);
    }

#if UNITY_EDITOR
    [VInspector.Button] private void Switch() => PanelManager.Instance.ActivateExclusive(id);
#endif
}
