using System;
using UnityEngine;

namespace Eclipse.Combat
{
    public static class DamageCalculator
    {
        public static DamageContext Calculate(
            UnityEngine.Object source,
            float baseDamage,
            float criticalChance,
            float criticalMultiplier,
            float randomRoll,
            Vector2 knockback = default,
            DamageType damageType = DamageType.Generic)
        {
            bool isCritical =
                EvaluateCritical(
                    criticalChance,
                    randomRoll);

            return new DamageContext(
                source,
                baseDamage,
                isCritical,
                criticalMultiplier,
                knockback,
                damageType);
        }

        public static bool EvaluateCritical(
            float criticalChance,
            float randomRoll)
        {
            if (!IsFinite(criticalChance))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(criticalChance),
                    "Critical chance must be finite.");
            }

            if (!IsFinite(randomRoll) ||
                randomRoll < 0f ||
                randomRoll > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(randomRoll),
                    "Random roll must be finite and between 0 and 1.");
            }

            float chance =
                Mathf.Clamp01(criticalChance);

            // Explicit boundaries guarantee:
            // 0% can never crit, even with roll 0.
            // 100% always crits, even with roll 1.
            if (chance <= 0f)
            {
                return false;
            }

            if (chance >= 1f)
            {
                return true;
            }

            return randomRoll < chance;
        }

        private static bool IsFinite(
            float value)
        {
            return
                !float.IsNaN(value) &&
                !float.IsInfinity(value);
        }
    }
}