using Eclipse.Combat;
using Eclipse.Enemies;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class EnemyDamageableTests
    {
        private const string BasicEnemyPrefabPath =
            "Assets/_Game/Prefabs/Enemies/BasicEnemy.prefab";

        private GameObject enemyInstance;
        private EnemyController enemyController;
        private Health health;

        [SetUp]
        public void SetUp()
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    BasicEnemyPrefabPath);

            Assert.IsNotNull(
                prefab,
                "BasicEnemy prefab could not be loaded.");

            enemyInstance =
                UnityEngine.Object.Instantiate(prefab);

            enemyController =
                enemyInstance.GetComponent<EnemyController>();

            health =
                enemyInstance.GetComponent<Health>();

            Assert.IsNotNull(enemyController);
            Assert.IsNotNull(health);

            health.Initialize(30f);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(
                enemyInstance);
        }

        [Test]
        public void EnemyController_ImplementsIDamageable()
        {
            Assert.IsInstanceOf<IDamageable>(
                enemyController);
        }

        [Test]
        public void TakeDamage_RoutesFinalDamageToHealth()
        {
            IDamageable damageable =
                enemyController;

            var context =
                new DamageContext(
                    null,
                    5f,
                    true,
                    2f);

            damageable.TakeDamage(context);

            Assert.AreEqual(
                20f,
                health.CurrentHealth);
        }
    }
}