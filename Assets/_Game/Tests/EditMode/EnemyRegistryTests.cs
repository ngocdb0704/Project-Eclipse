using System.Collections.Generic;
using Eclipse.Enemies;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;


namespace Eclipse.Tests.EditMode
{
    public sealed class EnemyRegistryTests
    {
        private const string BasicEnemyPrefabPath =
            "Assets/_Game/Prefabs/Enemies/BasicEnemy.prefab";

        private GameObject registryObject;
        private EnemyRegistry registry;

        private GameObject enemyObject;
        private EnemyController enemy;

        [SetUp]
        public void SetUp()
        {
            registryObject =
                new GameObject("EnemyRegistryTests");

            registry =
                registryObject.AddComponent<EnemyRegistry>();

            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    BasicEnemyPrefabPath);

            Assert.IsNotNull(prefab);

            enemyObject =
                Object.Instantiate(prefab);

            enemy =
                enemyObject.GetComponent<EnemyController>();

            Assert.IsNotNull(enemy);
        }

        [TearDown]
        public void TearDown()
        {
            registry.Clear();

            Object.DestroyImmediate(enemyObject);
            Object.DestroyImmediate(registryObject);
        }

        [Test]
        public void Register_ActiveEnemy_IncreasesCount()
        {
            bool registered = registry.Register(enemy);

            Assert.IsTrue(registered);
            Assert.AreEqual(1, registry.Count);
            Assert.AreSame(
                enemy,
                registry.ActiveEnemies[0]);
        }

        [Test]
        public void Register_SameEnemyTwice_DoesNotDuplicate()
        {
            bool first = registry.Register(enemy);
            bool second = registry.Register(enemy);

            Assert.IsTrue(first);
            Assert.IsFalse(second);
            Assert.AreEqual(1, registry.Count);
        }

        [Test]
        public void Unregister_RegisteredEnemy_DecreasesCount()
        {
            registry.Register(enemy);

            bool removed = registry.Unregister(enemy);

            Assert.IsTrue(removed);
            Assert.AreEqual(0, registry.Count);
        }

        [Test]
        public void ActiveEnemies_DoesNotExposeMutableList()
        {
            registry.Register(enemy);

            Assert.IsFalse(
                registry.ActiveEnemies
                is List<EnemyController>);
        }
    }
}