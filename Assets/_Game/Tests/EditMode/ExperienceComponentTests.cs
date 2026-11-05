
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
            UnityEngine.Object.DestroyImmediate(testObject);
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
        }

        [Test]
        public void ExactThreshold_LevelsUp()
        {
            experience.AddExperience(10);

            Assert.AreEqual(2, experience.Level);
            Assert.AreEqual(0, experience.CurrentXP);
        }

        [Test]
        public void Overflow_PreservesRemainingXP()
        {
            experience.AddExperience(17);

            Assert.AreEqual(2, experience.Level);
            Assert.AreEqual(7, experience.CurrentXP);
        }

        [Test]
        public void LargeGain_SupportsMultipleLevelUps()
        {
            experience.AddExperience(35);

            Assert.AreEqual(4, experience.Level);
            Assert.AreEqual(5, experience.CurrentXP);
        }

        [Test]
        public void ExperienceChanged_FiresOnceWithFinalState()
        {
            int eventCount = 0;
            int reportedLevel = 0;
            int reportedXP = 0;
            int reportedRequirement = 0;

            experience.ExperienceChanged += (level, xp, required) =>
            {
                eventCount++;
                reportedLevel = level;
                reportedXP = xp;
                reportedRequirement = required;
            };

            experience.AddExperience(23);

            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(3, reportedLevel);
            Assert.AreEqual(3, reportedXP);
            Assert.AreEqual(10, reportedRequirement);
        }

        [Test]
        public void ZeroExperience_DoesNotRaiseEvent()
        {
            int eventCount = 0;

            experience.ExperienceChanged += (_, _, _) =>
                eventCount++;

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
        }

        [Test]
        public void LargeExperience_DoesNotOverflowIntAddition()
        {
            experience.AddExperience(9);
            experience.AddExperience(int.MaxValue);

            Assert.AreEqual(214748366, experience.Level);
            Assert.AreEqual(6, experience.CurrentXP);
        }
    }
}
