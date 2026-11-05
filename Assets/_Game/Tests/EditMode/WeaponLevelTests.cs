
using System;
using System.Collections.Generic;
using Eclipse.Player;
using Eclipse.Weapons;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class WeaponLevelTests
    {
        private GameObject weaponObject;
        private TestWeapon weapon;
        private WeaponData data;

        [SetUp]
        public void SetUp()
        {
            data = ScriptableObject.CreateInstance<WeaponData>();

            weaponObject = new GameObject("WeaponLevelTests");
            weapon = weaponObject.AddComponent<TestWeapon>();

            var serialized = new SerializedObject(data);
            serialized.FindProperty("maxLevel").intValue = 3;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(weaponObject);
            UnityEngine.Object.DestroyImmediate(data);
        }

        [Test]
        public void UninitializedWeaponCannotUpgrade()
        {
            Assert.AreEqual(0, weapon.Level);
            Assert.AreEqual(0, weapon.MaxLevel);
            Assert.IsFalse(weapon.TryUpgrade());
        }

        [Test]
        public void InitializeStartsAtLevelOne()
        {
            weapon.Initialize(data, new PlayerStats());

            Assert.AreEqual(1, weapon.Level);
            Assert.AreEqual(3, weapon.MaxLevel);
            Assert.IsTrue(weapon.CanUpgrade);
        }

        [Test]
        public void UpgradesStopAtMaxLevel()
        {
            weapon.Initialize(data, new PlayerStats());

            Assert.IsTrue(weapon.TryUpgrade());
            Assert.AreEqual(2, weapon.Level);

            Assert.IsTrue(weapon.TryUpgrade());
            Assert.AreEqual(3, weapon.Level);

            Assert.IsFalse(weapon.TryUpgrade());
            Assert.IsFalse(weapon.CanUpgrade);
            Assert.AreEqual(3, weapon.Level);
        }

        [Test]
        public void LevelChangedOnlyFiresOnSuccessfulUpgrade()
        {
            var levels = new List<int>();

            weapon.LevelChanged += level => levels.Add(level);

            weapon.Initialize(data, new PlayerStats());

            weapon.TryUpgrade();
            weapon.TryUpgrade();
            weapon.TryUpgrade();

            CollectionAssert.AreEqual(
                new[] { 2, 3 },
                levels);
        }

        [Test]
        public void ReinitializeResetsLevel()
        {
            weapon.Initialize(data, new PlayerStats());
            weapon.TryUpgrade();

            weapon.Initialize(data, new PlayerStats());

            Assert.AreEqual(1, weapon.Level);
            Assert.IsTrue(weapon.CanUpgrade);
        }

        [Test]
        public void LevelStatsDriveRuntimeCombatValues()
        {
            ConfigureLevelStats();

            weapon.Initialize(data, new PlayerStats());

            Assert.AreEqual(10f, weapon.EffectiveDamage, 0.0001f);
            Assert.AreEqual(1f, weapon.EffectiveCooldown, 0.0001f);
            Assert.AreEqual(1, weapon.EffectiveProjectileCount);

            weapon.TryUpgrade();

            Assert.AreEqual(12f, weapon.EffectiveDamage, 0.0001f);
            Assert.AreEqual(0.9f, weapon.EffectiveCooldown, 0.0001f);
            Assert.AreEqual(2, weapon.EffectiveProjectileCount);

            weapon.TryUpgrade();

            Assert.AreEqual(15f, weapon.EffectiveDamage, 0.0001f);
            Assert.AreEqual(0.8f, weapon.EffectiveCooldown, 0.0001f);
            Assert.AreEqual(3, weapon.EffectiveProjectileCount);
        }

        [Test]
        public void UpgradeDoesNotResetCurrentCooldown()
        {
            ConfigureLevelStats();

            weapon.Initialize(data, new PlayerStats());

            weapon.Tick(0f);

            Assert.AreEqual(
                1f,
                weapon.CooldownRemaining,
                0.0001f);

            weapon.TryUpgrade();

            Assert.AreEqual(
                1f,
                weapon.CooldownRemaining,
                0.0001f);

            weapon.Tick(0.5f);

            Assert.AreEqual(
                0.5f,
                weapon.CooldownRemaining,
                0.0001f);

            weapon.Tick(0.5f);

            Assert.AreEqual(
                0.9f,
                weapon.CooldownRemaining,
                0.0001f);
        }

        [Test]
        public void MissingLevelStatsUseNeutralValues()
        {
            weapon.Initialize(data, new PlayerStats());

            weapon.TryUpgrade();

            Assert.AreEqual(10f, weapon.EffectiveDamage, 0.0001f);
            Assert.AreEqual(1f, weapon.EffectiveCooldown, 0.0001f);
            Assert.AreEqual(1, weapon.EffectiveProjectileCount);
        }

        [Test]
        public void LevelStatsLookupRejectsInvalidLevels()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => data.GetLevelStats(0));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => data.GetLevelStats(4));
        }

        [Test]
        public void LevelStatsPersistWithoutRuntimeMutation()
        {
            ConfigureLevelStats();

            float originalMultiplier =
                data.GetLevelStats(2).DamageMultiplier;

            weapon.Initialize(data, new PlayerStats());

            weapon.TryUpgrade();
            weapon.TryUpgrade();

            Assert.AreEqual(
                originalMultiplier,
                data.GetLevelStats(2).DamageMultiplier);

            Assert.AreEqual(3, data.MaxLevel);
        }

        private void ConfigureLevelStats()
        {
            var serialized = new SerializedObject(data);

            var levels = serialized.FindProperty("levelStats");

            levels.arraySize = 3;

            SetLevel(levels, 0, 1f, 1f, 0);
            SetLevel(levels, 1, 1.2f, 0.9f, 1);
            SetLevel(levels, 2, 1.5f, 0.8f, 2);

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetLevel(
            SerializedProperty levels,
            int index,
            float damageMultiplier,
            float cooldownMultiplier,
            int extraProjectiles)
        {
            SerializedProperty entry =
                levels.GetArrayElementAtIndex(index);

            entry.FindPropertyRelative("damageMultiplier")
                .floatValue = damageMultiplier;

            entry.FindPropertyRelative("cooldownMultiplier")
                .floatValue = cooldownMultiplier;

            entry.FindPropertyRelative("extraProjectiles")
                .intValue = extraProjectiles;
        }
    }
}
