
using Eclipse.Combat;
using Eclipse.Player;
using Eclipse.Weapons;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class KnifeWeaponTests
    {
        private const string KnifeDataPath =
            "Assets/_Game/Data/Weapons/KnifeData.asset";

        private const string MagicBoltDataPath =
            "Assets/_Game/Data/Weapons/MagicBoltData.asset";

        private WeaponData knifeData;
        private WeaponData magicBoltData;

        [SetUp]
        public void SetUp()
        {
            knifeData =
                AssetDatabase.LoadAssetAtPath<WeaponData>(
                    KnifeDataPath);

            magicBoltData =
                AssetDatabase.LoadAssetAtPath<WeaponData>(
                    MagicBoltDataPath);

            Assert.IsNotNull(knifeData);
            Assert.IsNotNull(magicBoltData);
        }

        [Test]
        public void KnifeData_HasValidWeaponConfiguration()
        {
            Assert.AreEqual("knife", knifeData.Id);
            Assert.AreEqual(
                WeaponType.Projectile,
                knifeData.WeaponType);

            Assert.AreEqual(1, knifeData.MaxLevel);
            Assert.IsNotNull(knifeData.Prefab);

            Assert.IsNotNull(
                knifeData.Prefab.GetComponent<KnifeWeapon>());
        }

        [Test]
        public void Knife_HasHighSpeedAndPierce()
        {
            Assert.Greater(
                knifeData.ProjectileSpeed,
                magicBoltData.ProjectileSpeed);

            Assert.AreEqual(2, knifeData.Pierce);
            Assert.Greater(knifeData.Cooldown, 0f);
        }

        [Test]
        public void KnifePrefab_UsesValidProjectile()
        {
            KnifeWeapon knife =
                knifeData.Prefab.GetComponent<KnifeWeapon>();

            Assert.IsNotNull(knife);

            var serialized = new SerializedObject(knife);

            SerializedProperty property =
                serialized.FindProperty("projectilePrefab");

            Assert.IsNotNull(property);

            Projectile projectile =
                property.objectReferenceValue as Projectile;

            Assert.IsNotNull(projectile);

            Assert.IsNotNull(
                projectile.GetComponent<Rigidbody2D>());

            Collider2D collider =
                projectile.GetComponent<Collider2D>();

            Assert.IsNotNull(collider);
            Assert.IsTrue(collider.isTrigger);
        }

        [Test]
        public void RuntimeModifiers_ApplyWithoutMutatingData()
        {
            GameObject instance =
                new GameObject("KnifeWeaponTest");

            try
            {
                KnifeWeapon knife =
                    instance.AddComponent<KnifeWeapon>();

                PlayerStats stats = new PlayerStats();

                knife.Initialize(knifeData, stats);

                float originalCooldown =
                    knifeData.Cooldown;

                int originalCount =
                    knifeData.ProjectileCount;

                Assert.AreEqual(
                    originalCooldown,
                    knife.EffectiveCooldown,
                    0.0001f);

                stats.AddProjectileCount(2);

                Assert.AreEqual(
                    originalCount + 2,
                    knife.EffectiveProjectileCount);

                stats.AddCooldownReduction(0.10f);

                Assert.AreEqual(
                    originalCooldown * 0.90f,
                    knife.EffectiveCooldown,
                    0.0001f);

                Assert.AreEqual(
                    originalCooldown,
                    knifeData.Cooldown);

                Assert.AreEqual(
                    originalCount,
                    knifeData.ProjectileCount);
            }
            finally
            {
                Object.DestroyImmediate(instance);
            }
        }
    }
}
