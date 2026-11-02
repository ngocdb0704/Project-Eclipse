using Eclipse.Combat;
using NUnit.Framework;

namespace Eclipse.Tests.EditMode
{
    public sealed class DamageCalculatorTests
    {
        [Test]
        public void ZeroChance_NeverCritical()
        {
            bool result =
                DamageCalculator.EvaluateCritical(
                    0f,
                    0f);

            Assert.IsFalse(result);
        }

        [Test]
        public void HundredPercentChance_AlwaysCritical()
        {
            bool result =
                DamageCalculator.EvaluateCritical(
                    1f,
                    1f);

            Assert.IsTrue(result);
        }

        [Test]
        public void Chance_IsClampedToZeroAndOne()
        {
            Assert.IsFalse(
                DamageCalculator.EvaluateCritical(
                    -10f,
                    0f));

            Assert.IsTrue(
                DamageCalculator.EvaluateCritical(
                    10f,
                    1f));
        }

        [Test]
        public void IntermediateChance_UsesDeterministicRoll()
        {
            Assert.IsTrue(
                DamageCalculator.EvaluateCritical(
                    0.5f,
                    0.49f));

            Assert.IsFalse(
                DamageCalculator.EvaluateCritical(
                    0.5f,
                    0.5f));

            Assert.IsFalse(
                DamageCalculator.EvaluateCritical(
                    0.5f,
                    0.75f));
        }

        [Test]
        public void CriticalMultiplier_IsAppliedExactlyOnce()
        {
            DamageContext context =
                DamageCalculator.Calculate(
                    null,
                    10f,
                    1f,
                    2.5f,
                    0.5f);

            Assert.IsTrue(
                context.IsCritical);

            Assert.AreEqual(
                10f,
                context.BaseDamage);

            Assert.AreEqual(
                2.5f,
                context.CriticalMultiplier);

            Assert.AreEqual(
                25f,
                context.FinalDamage);
        }

        [Test]
        public void NonCritical_FinalDamageEqualsBaseDamage()
        {
            DamageContext context =
                DamageCalculator.Calculate(
                    null,
                    10f,
                    0f,
                    2.5f,
                    0f);

            Assert.IsFalse(
                context.IsCritical);

            Assert.AreEqual(
                10f,
                context.FinalDamage);
        }
    }
}