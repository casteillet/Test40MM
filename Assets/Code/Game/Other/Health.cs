using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField, ReadOnly] private float currentHealth;

    private bool isInvulnerable;

    public bool IsDead { get; private set; }
    public float MaxHealth;
    public Action<float> OnHealthChanged;
    public Action<float> OnMaxHealthChanged;
    public Action OnDeath;
    
    private void Start()
    {
        currentHealth = MaxHealth;
        OnMaxHealthChanged?.Invoke(MaxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || isInvulnerable) return;

        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth > MaxHealth)
        {
            currentHealth = MaxHealth;
        }
    }


    protected virtual void Die()
    {
        if (IsDead) return;

        IsDead = true;
        OnDeath?.Invoke();
    }
}