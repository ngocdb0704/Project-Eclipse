
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
    public sealed class ProjectileCountUpgradeTests
    {
        private const string WeaponPath =
            "Assets/_Game/Data/Weapons/MagicBoltData.asset";

        private const string UpgradePath =
            "Assets/_Game/Data/Upgrades/ProjectileCountBoost.asset";

        private GameObject weaponObject;
        private TestWeapon weapon;
        private WeaponData weaponData;
        private UpgradeData upgradeData;
        private PlayerStats stats;

        [SetUp]
        public void SetUp()
        {
            weaponData =
                AssetDatabase.LoadAssetAtPath<WeaponData>(
                    WeaponPath);

            upgradeData =
                AssetDatabase.LoadAssetAtPath<UpgradeData>(
                    UpgradePath);

            Assert.IsNotNull(weaponData);
            Assert.IsNotNull(upgradeData);

            stats = new PlayerStats();

            weaponObject =
                new GameObject("ProjectileCountUpgradeTests");

            weapon = weaponObject.AddComponent<TestWeapon>();
            weapon.Initialize(weaponData, stats);
        }

        [TearDown]
        public void TearDown()
        {
            if (weaponObject != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    weaponObject);
            }
        }

        [Test]
        public void InitialProjectileCount_IsOne()
        {
            Assert.AreEqual(1, stats.ProjectileCount);
            Assert.AreEqual(1, weapon.EffectiveProjectileCount);
        }

        [Test]
        public void UpgradeLevels_IncreaseProjectileCount()
        {
            for (int level = 1; level <= 3; level++)
            {
                stats.AddProjectileCount(
                    upgradeData.ProjectilesPerLevel);

                Assert.AreEqual(
                    level + 1,
                    weapon.EffectiveProjectileCount);
            }
        }

        [Test]
        public void WeaponLevelBonus_IsAddedExactlyOnce()
        {
            stats.AddProjectileCount(3);

            // Level 1 -> Level 4
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(weapon.TryUpgrade());
            }

            Assert.AreEqual(4, weapon.Level);
            Assert.AreEqual(
                1,
                weaponData.GetLevelStats(4).ExtraProjectiles);

            // Base 1 + Player bonus 3 + Weapon bonus 1
            Assert.AreEqual(
                5,
                weapon.EffectiveProjectileCount);
        }

        [Test]
        public void InvalidIncreases_AreRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => stats.AddProjectileCount(0));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => stats.AddProjectileCount(-1));

            Assert.AreEqual(1, stats.ProjectileCount);
        }

        [Test]
        public void Overflow_IsRejected()
        {
            stats.AddProjectileCount(int.MaxValue - 1);

            Assert.AreEqual(
                int.MaxValue,
                stats.ProjectileCount);

            Assert.Throws<OverflowException>(
                () => stats.AddProjectileCount(1));
        }

        [Test]
        public void Upgrade_DoesNotMutateWeaponData()
        {
            int originalCount = weaponData.ProjectileCount;
            int originalLevelBonus =
                weaponData.GetLevelStats(4).ExtraProjectiles;

            stats.AddProjectileCount(3);

            for (int i = 0; i < 3; i++)
                weapon.TryUpgrade();

            Assert.AreEqual(
                originalCount,
                weaponData.ProjectileCount);

            Assert.AreEqual(
                originalLevelBonus,
                weaponData.GetLevelStats(4).ExtraProjectiles);
        }

        [Test]
        public void MaxedUpgrade_IsExcludedFromPool()
        {
            var owned = new Dictionary<string, int>
            {
                { upgradeData.Id, upgradeData.MaxLevel }
            };

            var pool = new UpgradePool(
                new[] { upgradeData },
                new System.Random(42));

            var options = pool.GetOptions(1, owned);

            Assert.IsEmpty(options);
        }
    }
}
