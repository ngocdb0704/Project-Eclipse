
using System;
using UnityEngine;

namespace Eclipse.Progression
{
    public sealed class ExperienceComponent : MonoBehaviour
    {
        // Temporary flat threshold for Task 054.
        // Task 055 will introduce a configurable EXP curve.
        [SerializeField, Min(1)]
        private int xpPerLevel = 10;

        public int Level { get; private set; } = 1;
        public int CurrentXP { get; private set; }

        public int RequiredXP => Mathf.Max(1, xpPerLevel);

        // Parameters: level, current XP, required XP.
        public event Action<int, int, int> ExperienceChanged;

        public void AddExperience(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Experience cannot be negative.");
            }

            if (amount == 0)
            {
                return;
            }

            // Use long to avoid overflow during addition.
            long totalXP = (long)CurrentXP + amount;
            int requirement = RequiredXP;

            // A constant requirement allows calculating
            // multiple level-ups without a long loop.
            long levelsGained = totalXP / requirement;

            if (levelsGained > int.MaxValue - (long)Level)
            {
                throw new OverflowException(
                    "Experience level exceeds the supported range.");
            }

            Level += (int)levelsGained;
            CurrentXP = (int)(totalXP % requirement);

            ExperienceChanged?.Invoke(
                Level,
                CurrentXP,
                requirement);
        }
    }
}
