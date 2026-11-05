
using System;
using Eclipse.Progression;
using NUnit.Framework;

namespace Eclipse.Tests.EditMode
{
    public sealed class ExperienceCurveTests
    {
        [TestCase(1, 10)]
        [TestCase(2, 25)]
        [TestCase(3, 42)]
        [TestCase(10, 200)]
        [TestCase(50, 1617)]
        public void DefaultCurve_ReturnsExpectedXP(
            int level, int expected)
        {
            var curve = new ExperienceCurve();

            Assert.AreEqual(
                expected,
                curve.GetRequiredXP(level));
        }

        [Test]
        public void CustomConfiguration_IsApplied()
        {
            var curve = new ExperienceCurve(20, 1f);

            Assert.AreEqual(20, curve.GetRequiredXP(1));
            Assert.AreEqual(40, curve.GetRequiredXP(2));
            Assert.AreEqual(1000, curve.GetRequiredXP(50));
        }

        [Test]
        public void DefaultCurve_IncreasesWithLevel()
        {
            var curve = new ExperienceCurve();

            int previous = curve.GetRequiredXP(1);

            for (int level = 2; level <= 100; level++)
            {
                int current = curve.GetRequiredXP(level);

                Assert.Greater(current, previous);
                previous = current;
            }
        }

        [Test]
        public void InvalidLevel_IsRejected()
        {
            var curve = new ExperienceCurve();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => curve.GetRequiredXP(0));
        }

        [Test]
        public void ExtremeLevel_DoesNotOverflow()
        {
            var curve = new ExperienceCurve();

            Assert.AreEqual(
                int.MaxValue,
                curve.GetRequiredXP(100000000));
        }
    }
}
