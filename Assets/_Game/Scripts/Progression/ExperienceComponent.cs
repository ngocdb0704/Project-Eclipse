
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

        // Raised once for each level gained.
        public event Action<int> LevelUp;

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

            int previousLevel = Level;
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


            /*
             * Commit the final state before notifying subscribers.
             * This prevents listeners from observing partially
             * calculated experience values.
             */
            Level = nextLevel;
            CurrentXP = (int)remainingXP;

            // Preserve Task 054/055 behavior:
            // one experience event per successful addition.
            ExperienceChanged?.Invoke(
                Level,
                CurrentXP,
                RequiredXP);

            // Notify each level gained, in ascending order.
            int levelsGained = nextLevel - previousLevel;

            for (int i = 0; i < levelsGained; i++)
            {
                int reachedLevel = previousLevel + i + 1;
                LevelUp?.Invoke(reachedLevel);
            }

        }
    }
}
