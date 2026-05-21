using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EntityDamageable : ValidatedMonoBehaviour
{
    [Self, SerializeField] private Health health;
    
    [Child(Flag.ExcludeSelf), SerializeField] private List<Health> damageableParts = new();

    private void OnEnable()
    {
        health.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= OnDeath;
    }

    private void Start()
    {
        float totalHealth = 0;
        
        foreach (var damageable in damageableParts)
        {
            totalHealth += damageable.MaxHealth;
        }
        
        health.MaxHealth = totalHealth;
    }

    private void OnDeath()
    {
        // TODO: Add vfx, sound effect
        Debug.Log($"{name} died");
    }

    public void TakeDamage(float damage)
    {
        health.TakeDamage(damage);
    }
}