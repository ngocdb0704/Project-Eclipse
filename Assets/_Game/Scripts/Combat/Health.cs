using System;
using UnityEngine;

namespace Eclipse.Combat
{
    public sealed class Health : MonoBehaviour, IHealth
    {
        [SerializeField, Min(1f)]
        private float maxHealth = 100f;

        public float MaxHealth { get; private set; }

        public float CurrentHealth { get; private set; }

        public bool IsDead { get; private set; }

        public event Action<float, float> HealthChanged;

        public event Action Died;

        private void Awake()
        {
            Initialize(maxHealth);
        }

        public void Initialize(float initialMaxHealth)
        {
            MaxHealth = Mathf.Max(1f, initialMaxHealth);
            CurrentHealth = MaxHealth;
            IsDead = false;
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || IsDead)
            {
                return;
            }

            float previousHealth = CurrentHealth;

            CurrentHealth = Mathf.Clamp(
                CurrentHealth - amount,
                0f,
                MaxHealth);

            if (CurrentHealth != previousHealth)
            {
                HealthChanged?.Invoke(CurrentHealth, MaxHealth);
            }

            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (amount <= 0f || IsDead)
            {
                return;
            }

            float previousHealth = CurrentHealth;

            CurrentHealth = Mathf.Clamp(
                CurrentHealth + amount,
                0f,
                MaxHealth);

            if (CurrentHealth != previousHealth)
            {
                HealthChanged?.Invoke(CurrentHealth, MaxHealth);
            }
        }

        private void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            Died?.Invoke();
        }
    }
}