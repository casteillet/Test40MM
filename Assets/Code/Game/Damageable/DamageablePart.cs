using KBCore.Refs;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageablePart : ValidatedMonoBehaviour
{
    [Self, SerializeField] private Health health;
    
    [Parent, SerializeField] private EntityDamageable entityDamageable;

    private void OnEnable()
    {
        health.OnDamage += OnDamage;
        health.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        health.OnDamage -= OnDamage;
        health.OnDeath -= OnDeath;
    }

    private void OnDamage(float damage)
    {
        entityDamageable.TakeDamage(damage);
    }

    private void OnDeath()
    {
        // TODO: Add vfx, sound effect
    }
}