using System.Collections.Generic;
using Eclipse.Enemies;
using Eclipse.Weapons.Targeting;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class NearestTargetFinderTests
    {
        private const string BasicEnemyPrefabPath =
            "Assets/_Game/Prefabs/Enemies/BasicEnemy.prefab";

        private GameObject registryObject;
        private EnemyRegistry registry;

        private GameObject enemyPrefab;

        private readonly List<GameObject>
            enemyObjects =
                new List<GameObject>();

        [SetUp]
        public void SetUp()
        {
            registryObject =
                new GameObject(
                    "NearestTargetFinderTests");

            registry =
                registryObject
                    .AddComponent<EnemyRegistry>();

            enemyPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    BasicEnemyPrefabPath);

            Assert.IsNotNull(enemyPrefab);
        }

        [TearDown]
        public void TearDown()
        {
            registry.Clear();

            for (int i = 0;
                 i < enemyObjects.Count;
                 i++)
            {
                UnityEngine.Object.DestroyImmediate(
                    enemyObjects[i]);
            }

            enemyObjects.Clear();

            UnityEngine.Object.DestroyImmediate(
                registryObject);
        }

        [Test]
        public void TryFindNearest_ReturnsClosestEnemy()
        {
            EnemyController far =
                CreateEnemy(
                    new Vector2(5f, 0f));

            EnemyController nearest =
                CreateEnemy(
                    new Vector2(2f, 0f));

            EnemyController diagonal =
                CreateEnemy(
                    new Vector2(3f, 4f));

            Assert.IsTrue(registry.Register(far));
            Assert.IsTrue(registry.Register(nearest));
            Assert.IsTrue(registry.Register(diagonal));

            bool found =
                NearestTargetFinder.TryFindNearest(
                    registry,
                    Vector2.zero,
                    out EnemyController result);

            Assert.IsTrue(found);
            Assert.AreSame(
                nearest,
                result);
        }

        [Test]
        public void TryFindNearest_UsesProvidedOrigin()
        {
            EnemyController left =
                CreateEnemy(
                    new Vector2(0f, 0f));

            EnemyController right =
                CreateEnemy(
                    new Vector2(10f, 0f));

            Assert.IsTrue(registry.Register(left));
            Assert.IsTrue(registry.Register(right));

            bool found =
                NearestTargetFinder.TryFindNearest(
                    registry,
                    new Vector2(9f, 0f),
                    out EnemyController result);

            Assert.IsTrue(found);
            Assert.AreSame(
                right,
                result);
        }

        [Test]
        public void TryFindNearest_SkipsInactiveEnemy()
        {
            EnemyController nearest =
                CreateEnemy(
                    new Vector2(1f, 0f));

            EnemyController valid =
                CreateEnemy(
                    new Vector2(3f, 0f));

            Assert.IsTrue(
                registry.Register(nearest));

            Assert.IsTrue(
                registry.Register(valid));

            nearest.gameObject.SetActive(false);

            bool found =
                NearestTargetFinder.TryFindNearest(
                    registry,
                    Vector2.zero,
                    out EnemyController result);

            Assert.IsTrue(found);
            Assert.AreSame(
                valid,
                result);
        }

        [Test]
        public void TryFindNearest_NoTargets_ReturnsFalse()
        {
            bool found =
                NearestTargetFinder.TryFindNearest(
                    registry,
                    Vector2.zero,
                    out EnemyController result);

            Assert.IsFalse(found);
            Assert.IsNull(result);
        }

        [Test]
        public void TryFindNearest_NullRegistry_ReturnsFalse()
        {
            bool found =
                NearestTargetFinder.TryFindNearest(
                    null,
                    Vector2.zero,
                    out EnemyController result);

            Assert.IsFalse(found);
            Assert.IsNull(result);
        }

        private EnemyController CreateEnemy(
            Vector2 position)
        {
            GameObject instance =
                UnityEngine.Object.Instantiate(
                    enemyPrefab);

            instance.transform.position =
                position;

            enemyObjects.Add(instance);

            EnemyController enemy =
                instance.GetComponent<EnemyController>();

            Assert.IsNotNull(enemy);

            return enemy;
        }
    }
}