using NaughtyAttributes;
using UnityEngine;

public class AgentHealth : Health
{
#if UNITY_EDITOR
    [Button] private void Heal10() => Heal(10);
    [Button] private void Damage10() => TakeDamage(10);
#endif
}
