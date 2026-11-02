using Eclipse.Player;
using Eclipse.Weapons;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class TestWeapon : WeaponBase
    {
        public int AttackAttempts { get; private set; }

        public bool AttackSucceeds { get; set; } = true;

        protected override bool Attack()
        {
            AttackAttempts++;
            return AttackSucceeds;
        }
    }

    public sealed class WeaponBaseTests
    {
        private const string MagicBoltDataPath =
            "Assets/_Game/Data/Weapons/MagicBoltData.asset";

        private GameObject gameObject;
        private TestWeapon weapon;
        private WeaponData weaponData;

        [SetUp]
        public void SetUp()
        {
            weaponData =
                AssetDatabase.LoadAssetAtPath<WeaponData>(
                    MagicBoltDataPath);

            Assert.IsNotNull(
                weaponData,
                "MagicBoltData could not be loaded.");

            gameObject =
                new GameObject("WeaponBaseTests");

            weapon =
                gameObject.AddComponent<TestWeapon>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(
                gameObject);
        }

        [Test]
        public void Tick_TriggersAttackAndRespectsCooldown()
        {
            weapon.Initialize(
                weaponData,
                new PlayerStats());

            weapon.Tick(0f);

            Assert.AreEqual(
                1,
                weapon.AttackAttempts);

            Assert.AreEqual(
                1f,
                weapon.CooldownRemaining,
                0.0001f);

            weapon.Tick(0.5f);

            Assert.AreEqual(
                1,
                weapon.AttackAttempts);

            Assert.AreEqual(
                0.5f,
                weapon.CooldownRemaining,
                0.0001f);

            weapon.Tick(0.5f);

            Assert.AreEqual(
                2,
                weapon.AttackAttempts);
        }

        [Test]
        public void FailedAttack_DoesNotConsumeCooldown()
        {
            weapon.Initialize(
                weaponData,
                new PlayerStats());

            weapon.AttackSucceeds = false;

            weapon.Tick(0f);

            Assert.AreEqual(
                1,
                weapon.AttackAttempts);

            Assert.AreEqual(
                0f,
                weapon.CooldownRemaining);
        }

        [Test]
        public void EffectiveCooldown_UsesPlayerModifiers()
        {
            PlayerData playerData =
                ScriptableObject.CreateInstance<PlayerData>();

            try
            {
                var serialized =
                    new SerializedObject(playerData);

                serialized
                    .FindProperty("attackSpeedMultiplier")
                    .floatValue = 2f;

                serialized
                    .FindProperty("cooldownReduction")
                    .floatValue = 0.25f;

                serialized.ApplyModifiedPropertiesWithoutUndo();

                var stats =
                    new PlayerStats(playerData);

                weapon.Initialize(
                    weaponData,
                    stats);

                Assert.AreEqual(
                    0.375f,
                    weapon.EffectiveCooldown,
                    0.0001f);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    playerData);
            }
        }

        [Test]
        public void Cooldown_NeverBecomesNegative()
        {
            weapon.Initialize(
                weaponData,
                new PlayerStats());

            weapon.Tick(0f);

            weapon.AttackSucceeds = false;

            weapon.Tick(100f);

            Assert.GreaterOrEqual(
                weapon.CooldownRemaining,
                0f);

            Assert.AreEqual(
                0f,
                weapon.CooldownRemaining);
        }
    }
}