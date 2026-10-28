using Eclipse.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class HealthTests
    {
        private GameObject gameObject;
        private Health health;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject("HealthTests");
            health = gameObject.AddComponent<Health>();
            health.Initialize(100f);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void TakeDamage_ReducesCurrentHealth()
        {
            health.TakeDamage(25f);

            Assert.AreEqual(75f, health.CurrentHealth);
            Assert.IsFalse(health.IsDead);
        }

        [Test]
        public void Heal_DoesNotExceedMaxHealth()
        {
            health.TakeDamage(25f);
            health.Heal(50f);

            Assert.AreEqual(100f, health.CurrentHealth);
        }

        [Test]
        public void LethalDamage_ClampsHealthToZero()
        {
            health.TakeDamage(150f);

            Assert.AreEqual(0f, health.CurrentHealth);
            Assert.IsTrue(health.IsDead);
        }

        [Test]
        public void Death_IsRaisedOnlyOnce()
        {
            int deathCount = 0;

            health.Died += () => deathCount++;

            health.TakeDamage(100f);
            health.TakeDamage(10f);
            health.TakeDamage(10f);

            Assert.AreEqual(1, deathCount);
        }
    }
}