using System;
using Eclipse.Combat;
using NUnit.Framework;

namespace Eclipse.Tests.EditMode
{
    public sealed class ProjectileHitTrackerTests
    {
        private sealed class DummyDamageable :
            IDamageable
        {
            public void TakeDamage(
                DamageContext context)
            {
            }
        }

        [Test]
        public void PierceZero_AllowsOneTarget()
        {
            var tracker =
                new ProjectileHitTracker();

            var first =
                new DummyDamageable();

            var second =
                new DummyDamageable();

            tracker.Reset(0);

            Assert.AreEqual(
                1,
                tracker.RemainingHits);

            Assert.IsTrue(
                tracker.TryRegister(first));

            Assert.AreEqual(
                0,
                tracker.RemainingHits);

            Assert.IsFalse(
                tracker.TryRegister(second));
        }

        [Test]
        public void PierceOne_AllowsTwoTargets()
        {
            var tracker =
                new ProjectileHitTracker();

            var first =
                new DummyDamageable();

            var second =
                new DummyDamageable();

            var third =
                new DummyDamageable();

            tracker.Reset(1);

            Assert.AreEqual(
                2,
                tracker.RemainingHits);

            Assert.IsTrue(
                tracker.TryRegister(first));

            Assert.IsTrue(
                tracker.TryRegister(second));

            Assert.AreEqual(
                0,
                tracker.RemainingHits);

            Assert.IsFalse(
                tracker.TryRegister(third));
        }

        [Test]
        public void SameTarget_DoesNotConsumeAnotherHit()
        {
            var tracker =
                new ProjectileHitTracker();

            var first =
                new DummyDamageable();

            var second =
                new DummyDamageable();

            tracker.Reset(2);

            Assert.IsTrue(
                tracker.TryRegister(first));

            Assert.AreEqual(
                2,
                tracker.RemainingHits);

            Assert.IsFalse(
                tracker.TryRegister(first));

            Assert.AreEqual(
                2,
                tracker.RemainingHits);

            Assert.IsTrue(
                tracker.TryRegister(second));

            Assert.AreEqual(
                1,
                tracker.RemainingHits);
        }

        [Test]
        public void PierceThree_AllowsFourUniqueTargets()
        {
            var tracker =
                new ProjectileHitTracker();

            tracker.Reset(3);

            Assert.IsTrue(
                tracker.TryRegister(
                    new DummyDamageable()));

            Assert.IsTrue(
                tracker.TryRegister(
                    new DummyDamageable()));

            Assert.IsTrue(
                tracker.TryRegister(
                    new DummyDamageable()));

            Assert.IsTrue(
                tracker.TryRegister(
                    new DummyDamageable()));

            Assert.AreEqual(
                0,
                tracker.RemainingHits);

            Assert.IsFalse(
                tracker.TryRegister(
                    new DummyDamageable()));
        }

        [Test]
        public void NegativePierce_Throws()
        {
            var tracker =
                new ProjectileHitTracker();

            Assert.Throws<
                ArgumentOutOfRangeException>(
                () => tracker.Reset(-1));
        }
    }
}