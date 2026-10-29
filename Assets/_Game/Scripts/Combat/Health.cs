using System;
using UnityEngine;

namespace Eclipse.Combat
{
    public sealed class Health : MonoBehaviour, IHealth
    {
        [SerializeField, Min(1f)]
        private float maxHealth = 100f;

        private float invulnerabilityDuration;
        private float invulnerableUntilTime;

        public float MaxHealth { get; private set; }

        public float CurrentHealth { get; private set; }

        public bool IsDead { get; private set; }

        public bool IsInvulnerable =>
            !IsDead &&
            invulnerabilityDuration > 0f &&
            Time.time < invulnerableUntilTime;

        public float InvulnerabilityDuration =>
            invulnerabilityDuration;

        public event Action<float, float> HealthChanged;

        public event Action Died;

        /// <summary>
        /// Hook for future hit flash / invulnerability visuals.
        /// Parameter is the active invulnerability duration.
        /// </summary>
        public event Action<float> InvulnerabilityStarted;

        private void Awake()
        {
            Initialize(maxHealth);
        }

        public void Initialize(float initialMaxHealth)
        {
            MaxHealth = Mathf.Max(1f, initialMaxHealth);
            CurrentHealth = MaxHealth;
            IsDead = false;
            invulnerableUntilTime = 0f;
        }

        public void ConfigureInvulnerability(float duration)
        {
            invulnerabilityDuration = Mathf.Max(0f, duration);
            invulnerableUntilTime = 0f;
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f ||
                IsDead ||
                IsInvulnerable)
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
                HealthChanged?.Invoke(
                    CurrentHealth,
                    MaxHealth);
            }

            if (CurrentHealth <= 0f)
            {
                Die();
                return;
            }

            StartInvulnerability();
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
                HealthChanged?.Invoke(
                    CurrentHealth,
                    MaxHealth);
            }
        }

        private void StartInvulnerability()
        {
            if (invulnerabilityDuration <= 0f)
            {
                return;
            }

            invulnerableUntilTime =
                Time.time + invulnerabilityDuration;

            InvulnerabilityStarted?.Invoke(
                invulnerabilityDuration);
        }

        private void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            invulnerableUntilTime = 0f;

            Died?.Invoke();
        }
    }
}