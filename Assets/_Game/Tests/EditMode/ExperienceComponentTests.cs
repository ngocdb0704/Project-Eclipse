
using System;
using Eclipse.Progression;
using NUnit.Framework;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class ExperienceComponentTests
    {
        private GameObject testObject;
        private ExperienceComponent experience;

        [SetUp]
        public void SetUp()
        {
            testObject = new GameObject("ExperienceTests");

            experience =
                testObject.AddComponent<ExperienceComponent>();
        }

        [TearDown]
        public void TearDown()
        {
            if (testObject != null)
            {
                UnityEngine.Object.DestroyImmediate(testObject);
            }
        }

        [Test]
        public void InitialState_IsValid()
        {
            Assert.AreEqual(1, experience.Level);
            Assert.AreEqual(0, experience.CurrentXP);
            Assert.AreEqual(10, experience.RequiredXP);
        }

        [Test]
        public void BelowThreshold_DoesNotLevelUp()
        {
            experience.AddExperience(5);

            Assert.AreEqual(1, experience.Level);
            Assert.AreEqual(5, experience.CurrentXP);
            Assert.AreEqual(10, experience.RequiredXP);
        }

        [Test]
        public void ExactThreshold_LevelsUp()
        {
            experience.AddExperience(10);

            Assert.AreEqual(2, experience.Level);
            Assert.AreEqual(0, experience.CurrentXP);

            // Level 2 requires ceil(10 * 2^1.3) = 25 XP.
            Assert.AreEqual(25, experience.RequiredXP);
        }

        [Test]
        public void Overflow_PreservesRemainingXP()
        {
            experience.AddExperience(17);

            Assert.AreEqual(2, experience.Level);
            Assert.AreEqual(7, experience.CurrentXP);
            Assert.AreEqual(25, experience.RequiredXP);
        }

        [Test]
        public void LargeGain_SupportsMultipleLevelUps()
        {
            // Level 1 -> 2: 10 XP
            // Level 2 -> 3: 25 XP
            // Total: 35 XP
            experience.AddExperience(35);

            Assert.AreEqual(3, experience.Level);
            Assert.AreEqual(0, experience.CurrentXP);
            Assert.AreEqual(42, experience.RequiredXP);
        }

        [Test]
        public void MultipleLevels_UseIncreasingRequirements()
        {
            // 53 - 10 - 25 = 18 XP remaining.
            experience.AddExperience(53);

            Assert.AreEqual(3, experience.Level);
            Assert.AreEqual(18, experience.CurrentXP);
            Assert.AreEqual(42, experience.RequiredXP);
        }

        [Test]
        public void SequentialGains_UseUpdatedThreshold()
        {
            experience.AddExperience(10);

            Assert.AreEqual(2, experience.Level);
            Assert.AreEqual(0, experience.CurrentXP);

            experience.AddExperience(20);

            Assert.AreEqual(2, experience.Level);
            Assert.AreEqual(20, experience.CurrentXP);

            // Complete the Level 2 threshold.
            experience.AddExperience(5);

            Assert.AreEqual(3, experience.Level);
            Assert.AreEqual(0, experience.CurrentXP);
            Assert.AreEqual(42, experience.RequiredXP);
        }

        [Test]
        public void SequentialGains_PreserveOverflow()
        {
            experience.AddExperience(8);
            experience.AddExperience(7);

            // 8 + 7 = 15
            // Level 1 costs 10 XP.
            Assert.AreEqual(2, experience.Level);
            Assert.AreEqual(5, experience.CurrentXP);

            experience.AddExperience(23);

            // 5 + 23 = 28
            // Level 2 costs 25 XP.
            Assert.AreEqual(3, experience.Level);
            Assert.AreEqual(3, experience.CurrentXP);
            Assert.AreEqual(42, experience.RequiredXP);
        }

        [Test]
        public void ExperienceChanged_FiresOnceWithFinalState()
        {
            int eventCount = 0;
            int reportedLevel = 0;
            int reportedXP = 0;
            int reportedRequirement = 0;

            experience.ExperienceChanged +=
                (level, xp, required) =>
                {
                    eventCount++;
                    reportedLevel = level;
                    reportedXP = xp;
                    reportedRequirement = required;
                };

            experience.AddExperience(23);

            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(2, reportedLevel);
            Assert.AreEqual(13, reportedXP);
            Assert.AreEqual(25, reportedRequirement);
        }

        [Test]
        public void MultipleLevelUps_RaiseExperienceChangedOnce()
        {
            int eventCount = 0;
            int reportedLevel = 0;
            int reportedXP = 0;
            int reportedRequirement = 0;

            experience.ExperienceChanged +=
                (level, xp, required) =>
                {
                    eventCount++;
                    reportedLevel = level;
                    reportedXP = xp;
                    reportedRequirement = required;
                };

            experience.AddExperience(53);

            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(3, reportedLevel);
            Assert.AreEqual(18, reportedXP);
            Assert.AreEqual(42, reportedRequirement);
        }

        [Test]
        public void ZeroExperience_DoesNotRaiseEvent()
        {
            int eventCount = 0;

            experience.ExperienceChanged +=
                (_, _, _) => eventCount++;

            experience.AddExperience(0);

            Assert.AreEqual(1, experience.Level);
            Assert.AreEqual(0, experience.CurrentXP);
            Assert.AreEqual(0, eventCount);
        }

        [Test]
        public void NegativeExperience_IsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => experience.AddExperience(-5));

            Assert.AreEqual(1, experience.Level);
            Assert.AreEqual(0, experience.CurrentXP);
            Assert.AreEqual(10, experience.RequiredXP);
        }

        [Test]
        public void NegativeExperience_DoesNotModifyExistingState()
        {
            experience.AddExperience(17);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => experience.AddExperience(-1));

            Assert.AreEqual(2, experience.Level);
            Assert.AreEqual(7, experience.CurrentXP);
            Assert.AreEqual(25, experience.RequiredXP);
        }

        [Test]
        public void LargeExperience_DoesNotOverflowIntAddition()
        {
            experience.AddExperience(9);
            experience.AddExperience(int.MaxValue);

            Assert.Greater(experience.Level, 1);
            Assert.GreaterOrEqual(experience.CurrentXP, 0);
            Assert.Less(
                experience.CurrentXP,
                experience.RequiredXP);
        }

        [Test]
        public void LargeExperience_RaisesValidEvent()
        {
            int eventCount = 0;
            int reportedLevel = 0;
            int reportedXP = 0;
            int reportedRequirement = 0;

            experience.ExperienceChanged +=
                (level, xp, required) =>
                {
                    eventCount++;
                    reportedLevel = level;
                    reportedXP = xp;
                    reportedRequirement = required;
                };

            experience.AddExperience(int.MaxValue);

            Assert.AreEqual(1, eventCount);

            Assert.AreEqual(
                experience.Level,
                reportedLevel);

            Assert.AreEqual(
                experience.CurrentXP,
                reportedXP);

            Assert.AreEqual(
                experience.RequiredXP,
                reportedRequirement);

            Assert.Greater(reportedLevel, 1);
            Assert.GreaterOrEqual(reportedXP, 0);
            Assert.Less(reportedXP, reportedRequirement);
        }


        [Test]
        public void ExactThreshold_RaisesLevelUpOnce()
        {
            int eventCount = 0;
            int reachedLevel = 0;

            experience.LevelUp += level =>
            {
                eventCount++;
                reachedLevel = level;
            };

            experience.AddExperience(10);

            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(2, reachedLevel);
            Assert.AreEqual(2, experience.Level);
        }

        [Test]
        public void MultipleLevelUps_RaiseOrderedEvents()
        {
            var reachedLevels = new System.Collections.Generic.List<int>();

            experience.LevelUp += level =>
                reachedLevels.Add(level);

            experience.AddExperience(53);

            CollectionAssert.AreEqual(
                new[] { 2, 3 },
                reachedLevels);

            Assert.AreEqual(3, experience.Level);
            Assert.AreEqual(18, experience.CurrentXP);
        }

        [Test]
        public void BelowThreshold_DoesNotRaiseLevelUp()
        {
            int eventCount = 0;

            experience.LevelUp += _ => eventCount++;

            experience.AddExperience(5);
            experience.AddExperience(0);

            Assert.AreEqual(0, eventCount);
            Assert.AreEqual(1, experience.Level);
            Assert.AreEqual(5, experience.CurrentXP);
        }

        [Test]
        public void LevelUp_DoesNotDuplicateAcrossGains()
        {
            var reachedLevels = new System.Collections.Generic.List<int>();
            int experienceChangedCount = 0;

            experience.LevelUp += level =>
                reachedLevels.Add(level);

            experience.ExperienceChanged +=
                (_, _, _) => experienceChangedCount++;

            experience.AddExperience(10);
            experience.AddExperience(5);
            experience.AddExperience(20);

            CollectionAssert.AreEqual(
                new[] { 2, 3 },
                reachedLevels);

            Assert.AreEqual(3, experienceChangedCount);
            Assert.AreEqual(3, experience.Level);
            Assert.AreEqual(0, experience.CurrentXP);
        }

    }
}
