using System;
using NaughtyAttributes;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField, ReadOnly] private float currentHealth;

    private bool isInvulnerable;

    public bool IsDead { get; private set; }
    public float MaxHealth;
    
    public event Action<float, float> OnHealthInit;
    public event Action<float> OnDamage;
    public event Action OnDeath;
    
    private void Start()
    {
        currentHealth = MaxHealth;
        OnHealthInit?.Invoke(currentHealth, MaxHealth);
    }

    public void TakeDamage(AmmoDefinition ammoDefinition)
    {
        if (IsDead || isInvulnerable) return;

        var damage = Mathf.Min(ammoDefinition.baseDamage, currentHealth);
        
        currentHealth -= damage;
        
        OnDamage?.Invoke(damage);
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            Debug.Log($"{name} damaged by {ammoDefinition.ammoId} of {ammoDefinition.baseDamage}[{ammoDefinition.damageType}] | {currentHealth}/{MaxHealth}", this);
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (IsDead || isInvulnerable) return;

        currentHealth -= damage;
        
        OnDamage?.Invoke(damage);
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            Debug.Log($"{name} damaged of {damage} | {currentHealth}/{MaxHealth}", this);
        }
    }

    private void Die()
    {
        if (IsDead || isInvulnerable) return;

        IsDead = true;
        OnDeath?.Invoke();
        
        Debug.Log($"{name} died", this);
    }
}