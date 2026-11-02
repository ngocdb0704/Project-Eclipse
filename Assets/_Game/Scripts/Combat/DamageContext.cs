using System;
using UnityEngine;

namespace Eclipse.Combat
{
    public enum DamageType
    {
        Generic = 0,
        Physical = 1,
        Magical = 2
    }

    public readonly struct DamageContext
    {
        public UnityEngine.Object Source { get; }

        public float BaseDamage { get; }

        public bool IsCritical { get; }

        public float CriticalMultiplier { get; }

        public Vector2 Knockback { get; }

        public DamageType DamageType { get; }

        public float FinalDamage =>
            IsCritical
                ? BaseDamage * CriticalMultiplier
                : BaseDamage;

        public DamageContext(
            UnityEngine.Object source,
            float baseDamage,
            bool isCritical = false,
            float criticalMultiplier = 1f,
            Vector2 knockback = default,
            DamageType damageType = DamageType.Generic)
        {
            if (!IsFinite(baseDamage) ||
                baseDamage < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(baseDamage),
                    "Base damage must be finite and non-negative.");
            }

            if (!IsFinite(criticalMultiplier) ||
                criticalMultiplier < 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(criticalMultiplier),
                    "Critical multiplier must be finite and at least 1.");
            }

            if (!IsFinite(knockback.x) ||
                !IsFinite(knockback.y))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(knockback),
                    "Knockback components must be finite.");
            }

            Source = source;
            BaseDamage = baseDamage;
            IsCritical = isCritical;
            CriticalMultiplier = criticalMultiplier;
            Knockback = knockback;
            DamageType = damageType;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) &&
                   !float.IsInfinity(value);
        }
    }
}