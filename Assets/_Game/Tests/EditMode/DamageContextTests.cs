using System;
using Eclipse.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class DamageContextTests
    {
        private GameObject source;

        [SetUp]
        public void SetUp()
        {
            source =
                new GameObject("DamageContextTests");
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(source);
        }

        [Test]
        public void Constructor_StoresProvidedValues()
        {
            Vector2 knockback =
                new Vector2(2f, 3f);

            var context =
                new DamageContext(
                    source,
                    10f,
                    true,
                    2f,
                    knockback,
                    DamageType.Physical);

            Assert.AreSame(
                source,
                context.Source);

            Assert.AreEqual(
                10f,
                context.BaseDamage);

            Assert.IsTrue(
                context.IsCritical);

            Assert.AreEqual(
                2f,
                context.CriticalMultiplier);

            Assert.AreEqual(
                knockback,
                context.Knockback);

            Assert.AreEqual(
                DamageType.Physical,
                context.DamageType);

            Assert.AreEqual(
                20f,
                context.FinalDamage);
        }

        [Test]
        public void FinalDamage_NonCritical_EqualsBaseDamage()
        {
            var context =
                new DamageContext(
                    source,
                    15f,
                    false,
                    3f);

            Assert.AreEqual(
                15f,
                context.FinalDamage);
        }

        [Test]
        public void Constructor_AllowsNullSource()
        {
            var context =
                new DamageContext(
                    null,
                    5f);

            Assert.IsNull(
                context.Source);

            Assert.AreEqual(
                5f,
                context.FinalDamage);
        }

        [Test]
        public void Constructor_InvalidValues_Throw()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new DamageContext(
                    source,
                    -1f));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new DamageContext(
                    source,
                    float.NaN));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new DamageContext(
                    source,
                    10f,
                    true,
                    0.5f));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new DamageContext(
                    source,
                    10f,
                    false,
                    1f,
                    new Vector2(
                        float.PositiveInfinity,
                        0f)));
        }
    }
}