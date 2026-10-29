using System;

namespace Eclipse.Combat
{
    public interface IHealth
    {
        float MaxHealth { get; }

        float CurrentHealth { get; }

        bool IsDead { get; }

        event Action<float, float> HealthChanged;

        event Action Died;

        void Initialize(float maxHealth);

        void TakeDamage(float amount);

        void Heal(float amount);
    }
}