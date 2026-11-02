using Eclipse.Core.Pooling;
using NUnit.Framework;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class GameObjectPoolTests
    {
        private GameObject prefabObject;
        private TestPoolableComponent prefab;

        private GameObject poolRoot;

        [SetUp]
        public void SetUp()
        {
            prefabObject =
                new GameObject(
                    "Pool Test Prefab");

            prefab =
                prefabObject.AddComponent<
                    TestPoolableComponent>();

            poolRoot =
                new GameObject(
                    "Pool Root");
        }

        [TearDown]
        public void TearDown()
        {
            if (poolRoot != null)
            {
                Object.DestroyImmediate(
                    poolRoot);
            }

            if (prefabObject != null)
            {
                Object.DestroyImmediate(
                    prefabObject);
            }
        }

        [Test]
        public void Prewarm_CreatesRequestedInactiveCount()
        {
            GameObjectPool<TestPoolableComponent>
                pool =
                    CreatePool();

            pool.Prewarm(3);

            Assert.AreEqual(
                3,
                pool.CountAll);

            Assert.AreEqual(
                3,
                pool.CountInactive);

            Assert.AreEqual(
                0,
                pool.CountActive);

            Assert.AreEqual(
                3,
                poolRoot.transform.childCount);
        }

        [Test]
        public void Prewarm_DoesNotDuplicateExistingTargetCount()
        {
            GameObjectPool<TestPoolableComponent>
                pool =
                    CreatePool();

            pool.Prewarm(3);
            pool.Prewarm(3);

            Assert.AreEqual(
                3,
                pool.CountAll);

            Assert.AreEqual(
                3,
                pool.CountInactive);

            Assert.AreEqual(
                0,
                pool.CountActive);
        }

        [Test]
        public void Get_ActivatesAndNotifiesPoolable()
        {
            GameObjectPool<TestPoolableComponent>
                pool =
                    CreatePool();

            pool.Prewarm(1);

            TestPoolableComponent instance =
                pool.Get();

            Assert.IsTrue(
                instance.gameObject.activeSelf);

            Assert.AreEqual(
                1,
                instance.SpawnedCount);

            Assert.AreEqual(
                1,
                pool.CountActive);

            Assert.AreEqual(
                0,
                pool.CountInactive);
        }

        [Test]
        public void Release_DeactivatesNotifiesAndReusesInstance()
        {
            GameObjectPool<TestPoolableComponent>
                pool =
                    CreatePool();

            pool.Prewarm(1);

            TestPoolableComponent first =
                pool.Get();

            bool released =
                pool.Release(first);

            Assert.IsTrue(released);

            Assert.IsFalse(
                first.gameObject.activeSelf);

            Assert.AreEqual(
                1,
                first.DespawnedCount);

            Assert.AreEqual(
                0,
                pool.CountActive);

            Assert.AreEqual(
                1,
                pool.CountInactive);

            TestPoolableComponent second =
                pool.Get();

            Assert.AreSame(
                first,
                second);

            Assert.AreEqual(
                2,
                second.SpawnedCount);

            Assert.AreEqual(
                1,
                second.DespawnedCount);
        }

        [Test]
        public void DoubleRelease_IsRejectedWithoutChangingCounts()
        {
            GameObjectPool<TestPoolableComponent>
                pool =
                    CreatePool();

            TestPoolableComponent instance =
                pool.Get();

            Assert.IsTrue(
                pool.Release(instance));

            Assert.IsFalse(
                pool.Release(instance));

            Assert.AreEqual(
                0,
                pool.CountActive);

            Assert.AreEqual(
                1,
                pool.CountInactive);

            Assert.AreEqual(
                1,
                pool.CountAll);

            Assert.AreEqual(
                1,
                instance.DespawnedCount);
        }

        [Test]
        public void ActiveTracking_ReturnsToZeroAfterAllReleased()
        {
            GameObjectPool<TestPoolableComponent>
                pool =
                    CreatePool();

            pool.Prewarm(2);

            TestPoolableComponent first =
                pool.Get();

            TestPoolableComponent second =
                pool.Get();

            // Pool is allowed to grow when exhausted.
            TestPoolableComponent third =
                pool.Get();

            Assert.AreEqual(
                3,
                pool.CountActive);

            Assert.AreEqual(
                3,
                pool.CountAll);

            Assert.IsTrue(
                pool.Release(first));

            Assert.IsTrue(
                pool.Release(second));

            Assert.IsTrue(
                pool.Release(third));

            Assert.AreEqual(
                0,
                pool.CountActive);

            Assert.AreEqual(
                3,
                pool.CountInactive);

            Assert.AreEqual(
                3,
                pool.CountAll);
        }

        private GameObjectPool<
            TestPoolableComponent> CreatePool()
        {
            return new GameObjectPool<
                TestPoolableComponent>(
                prefab,
                poolRoot.transform);
        }
    }

    public sealed class TestPoolableComponent :
        MonoBehaviour,
        IPoolable
    {
        public int SpawnedCount { get; private set; }

        public int DespawnedCount { get; private set; }

        public void OnSpawned()
        {
            SpawnedCount++;
        }

        public void OnDespawned()
        {
            DespawnedCount++;
        }
    }
}