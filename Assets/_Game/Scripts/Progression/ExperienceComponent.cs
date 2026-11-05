
using System;
using UnityEngine;

namespace Eclipse.Progression
{
    public sealed class ExperienceComponent : MonoBehaviour
    {
        [SerializeField]
        private ExperienceCurve curve = new ExperienceCurve();

        public int Level { get; private set; } = 1;
        public int CurrentXP { get; private set; }

        public int RequiredXP => ActiveCurve.GetRequiredXP(Level);

        // Parameters: level, current XP, required XP.
        public event Action<int, int, int> ExperienceChanged;

        private ExperienceCurve ActiveCurve =>
            curve ?? (curve = new ExperienceCurve());

        public void AddExperience(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Experience cannot be negative.");
            }

            if (amount == 0)
                return;

            long remainingXP = (long)CurrentXP + amount;
            int nextLevel = Level;

            ExperienceCurve activeCurve = ActiveCurve;

            // A single gain can cross multiple level thresholds.
            while (true)
            {
                int required = activeCurve.GetRequiredXP(nextLevel);

                if (remainingXP < required)
                    break;

                if (nextLevel == int.MaxValue)
                {
                    throw new OverflowException(
                        "Experience level exceeds the supported range.");
                }

                remainingXP -= required;
                nextLevel++;
            }

            // Commit state only after calculations succeed.
            Level = nextLevel;
            CurrentXP = (int)remainingXP;

            ExperienceChanged?.Invoke(
                Level,
                CurrentXP,
                RequiredXP);
        }
    }
}
