using KBCore.Refs;
using UnityEngine;

public sealed class SubPanel : PanelBase
{
    [Parent(Flag.ExcludeSelf | Flag.IncludeInactive), SerializeField] private SubPanelGroup ownerGroup;

    public SubPanelGroup OwnerGroup => ownerGroup;

#if UNITY_EDITOR
    [VInspector.Button] private void Switch() => ownerGroup.ActivateExclusive(this);
#endif
}
