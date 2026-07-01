using System;
using Unity.Netcode;

public sealed class OverridableField<T> where T : unmanaged
{
    private readonly NetworkVariable<T> effective;
    private readonly NetworkVariable<bool> overridden;

    public event Action<T> OnEffectiveChanged;

    public T Effective => effective.Value;
    public bool IsOverridden => overridden.Value;

    public OverridableField(NetworkVariable<T> effective, NetworkVariable<bool> overridden)
    {
        this.effective = effective;
        this.overridden = overridden;
        this.effective.OnValueChanged += HandleEffectiveChanged;
    }

    public void SetBaseline(T value)
    {
        overridden.Value = false;
        effective.Value = value;
    }
    
    public void ApplyOverride(T value)
    {
        overridden.Value = true;
        effective.Value = value;
    }

    public void Dispose()
    {
        effective.OnValueChanged -= HandleEffectiveChanged;
        OnEffectiveChanged = null;
    }

    private void HandleEffectiveChanged(T previous, T current) => OnEffectiveChanged?.Invoke(current);
}
