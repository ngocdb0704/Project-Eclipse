
using System;
using UnityEngine;

namespace Eclipse.Progression
{
    [Serializable]
    public sealed class ExperienceCurve
    {
        [SerializeField, Min(1)]
        private int baseXP = 10;

        [SerializeField, Min(1f)]
        private float exponent = 1.3f;

        public ExperienceCurve()
        {
        }

        public ExperienceCurve(int baseXP, float exponent)
        {
            if (baseXP < 1)
                throw new ArgumentOutOfRangeException(nameof(baseXP));

            if (float.IsNaN(exponent) ||
                float.IsInfinity(exponent) ||
                exponent < 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(exponent));
            }

            this.baseXP = baseXP;
            this.exponent = exponent;
        }

        public int GetRequiredXP(int level)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level));

            int safeBase = Mathf.Max(1, baseXP);
            float safeExponent =
                float.IsNaN(exponent) ||
                float.IsInfinity(exponent)
                    ? 1.3f
                    : Mathf.Max(1f, exponent);

            double result =
                safeBase * Math.Pow(level, safeExponent);

            // Prevent integer overflow at extreme levels.
            if (result >= int.MaxValue)
                return int.MaxValue;

            // Always round upward to a whole EXP point.
            return Math.Max(1, (int)Math.Ceiling(result));
        }
    }
}
