
using System;
using System.Collections.Generic;
using Eclipse.Player;
using Eclipse.Progression;
using Eclipse.Weapons;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class DamageUpgradeTests
    {
        private const float Tolerance = 0.0001f;

        private const string WeaponPath =
            "Assets/_Game/Data/Weapons/MagicBoltData.asset";

        private const string UpgradePath =
            "Assets/_Game/Data/Upgrades/DamageBoost.asset";

        private GameObject weaponObject;
        private TestWeapon weapon;
        private WeaponData weaponData;
        private UpgradeData damageBoost;
        private PlayerStats stats;

        [SetUp]
        public void SetUp()
        {
            weaponData =
                AssetDatabase.LoadAssetAtPath<WeaponData>(
                    WeaponPath);

            damageBoost =
                AssetDatabase.LoadAssetAtPath<UpgradeData>(
                    UpgradePath);

            Assert.IsNotNull(weaponData);
            Assert.IsNotNull(damageBoost);

            stats = new PlayerStats();

            weaponObject = new GameObject("DamageUpgradeTests");
            weapon = weaponObject.AddComponent<TestWeapon>();

            weapon.Initialize(weaponData, stats);
        }

        [TearDown]
        public void TearDown()
        {
            if (weaponObject != null)
                UnityEngine.Object.DestroyImmediate(weaponObject);
        }

        [Test]
        public void InitialDamage_IsTen()
        {
            Assert.AreEqual(
                10f, weapon.EffectiveDamage, Tolerance);
        }

        [Test]
        public void DamageBoost_StacksAdditively()
        {
            ApplyBoost(1);

            Assert.AreEqual(
                11f, weapon.EffectiveDamage, Tolerance);

            ApplyBoost(1);

            Assert.AreEqual(
                12f, weapon.EffectiveDamage, Tolerance);
        }

        [Test]
        public void FiveBoosts_IncreaseDamageByFiftyPercent()
        {
            ApplyBoost(5);

            Assert.AreEqual(
                1.5f, stats.DamageMultiplier, Tolerance);

            Assert.AreEqual(
                15f, weapon.EffectiveDamage, Tolerance);
        }

        [Test]
        public void WeaponLevelAndDamageBoost_CombineCorrectly()
        {
            Assert.IsTrue(weapon.TryUpgrade());

            // Magic Bolt level 2 multiplier = 1.1
            Assert.AreEqual(
                11f, weapon.EffectiveDamage, Tolerance);

            ApplyBoost(2);

            // 10 * 1.1 * 1.2 = 13.2
            Assert.AreEqual(
                13.2f, weapon.EffectiveDamage, Tolerance);
        }

        [Test]
        public void DamageUpgrade_DoesNotMutateSourceAssets()
        {
            float originalDamage = weaponData.Damage;
            float originalValue = damageBoost.ValuePerLevel;
            float originalLevelMultiplier =
                weaponData.GetLevelStats(2).DamageMultiplier;

            ApplyBoost(3);
            weapon.TryUpgrade();

            Assert.AreEqual(originalDamage, weaponData.Damage);
            Assert.AreEqual(
                originalValue, damageBoost.ValuePerLevel);
            Assert.AreEqual(
                originalLevelMultiplier,
                weaponData.GetLevelStats(2).DamageMultiplier);
        }

        [Test]
        public void MaxedDamageBoost_IsExcludedFromPool()
        {
            var owned = new Dictionary<string, int>
            {
                { damageBoost.Id, damageBoost.MaxLevel }
            };

            var pool = new UpgradePool(
                new[] { damageBoost },
                new System.Random(42));

            var options = pool.GetOptions(1, owned);

            Assert.IsEmpty(options);
        }

        private void ApplyBoost(int count)
        {
            for (int i = 0; i < count; i++)
                stats.AddDamageMultiplier(
                    damageBoost.ValuePerLevel);
        }
    }
}
