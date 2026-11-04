
using System;
using System.Collections;
using System.Reflection;
using Eclipse.Combat;
using Eclipse.Core.Pooling;
using Eclipse.Enemies;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Eclipse.Tests.PlayMode
{
    public sealed class EnemyPoolLifecycleTests
    {
        private const BindingFlags PrivateInstance =
            BindingFlags.Instance | BindingFlags.NonPublic;

        private GameObject prefabObject;
        private GameObject poolRoot;
        private GameObject targetObject;
        private GameObject registryObject;

        private EnemyData enemyData;
        private EnemyRegistry registry;

        private GameObjectPool<EnemyController> pool;

        private int deathCallbacks;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            enemyData =
                ScriptableObject.CreateInstance<EnemyData>();

            prefabObject =
                new GameObject("Enemy Test Prefab");

            prefabObject.SetActive(false);

            Rigidbody2D body =
                prefabObject.AddComponent<Rigidbody2D>();

            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;

            prefabObject.AddComponent<CircleCollider2D>();

            EnemyController prefab =
                prefabObject.AddComponent<EnemyController>();

            FieldInfo enemyDataField =
                typeof(EnemyController).GetField(
                    "enemyData",
                    PrivateInstance);

            Assert.IsNotNull(enemyDataField);
            enemyDataField.SetValue(prefab, enemyData);

            poolRoot = new GameObject("Enemy Pool Root");

            targetObject = new GameObject("Chase Target");
            targetObject.transform.position =
                new Vector3(5f, 0f, 0f);

            registryObject =
                new GameObject("Enemy Registry");

            registry =
                registryObject.AddComponent<EnemyRegistry>();

            pool = new GameObjectPool<EnemyController>(
                prefab,
                poolRoot.transform);

            pool.Prewarm(1);
            deathCallbacks = 0;

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (registry != null)
            {
                registry.Clear();
            }

            if (poolRoot != null)
                UnityEngine.Object.Destroy(poolRoot);

            if (prefabObject != null)
                UnityEngine.Object.Destroy(prefabObject);

            if (targetObject != null)
                UnityEngine.Object.Destroy(targetObject);

            if (registryObject != null)
                UnityEngine.Object.Destroy(registryObject);

            if (enemyData != null)
                UnityEngine.Object.Destroy(enemyData);

            yield return null;
        }

        [UnityTest]
        public IEnumerator SpawnKillReleaseRespawn_ResetsState()
        {
            int originalInstanceId = 0;

            for (int cycle = 0; cycle < 3; cycle++)
            {
                EnemyController enemy =
                    pool.Get(
                        Vector3.zero,
                        Quaternion.identity);

                if (cycle == 0)
                {
                    originalInstanceId =
                        enemy.GetInstanceID();
                }

                Assert.AreEqual(
                    originalInstanceId,
                    enemy.GetInstanceID());

                Health health =
                    enemy.GetComponent<Health>();

                Rigidbody2D body =
                    enemy.GetComponent<Rigidbody2D>();

                Collider2D collider =
                    enemy.GetComponent<Collider2D>();

                Assert.IsFalse(enemy.IsDead);
                Assert.IsFalse(health.IsDead);

                Assert.AreEqual(
                    enemy.Data.MaxHP,
                    health.CurrentHealth);

                Assert.AreEqual(
                    0f,
                    health.InvulnerabilityDuration);

                Assert.IsTrue(collider.enabled);
                Assert.IsTrue(enemy.Movement.enabled);
                Assert.IsNull(enemy.Movement.Target);

                Assert.AreEqual(
                    Vector2.zero,
                    body.linearVelocity);

                Assert.AreEqual(
                    0f,
                    body.angularVelocity);

                Assert.AreEqual(
                    0f,
                    GetContactTimer(enemy));

                Assert.AreEqual(
                    1,
                    CountHealthDeathHandlers(health, enemy));

                enemy.Initialize(targetObject.transform);

                Assert.AreSame(
                    targetObject.transform,
                    enemy.Movement.Target);

                Assert.IsTrue(
                    registry.Register(enemy));

                enemy.Died += HandleEnemyDied;

                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();

                Assert.Greater(
                    body.position.x,
                    0f,
                    "Enemy did not chase its target.");

                // Simulate damage state configured during
                // a previous lifecycle.
                health.ConfigureInvulnerability(5f);

                health.TakeDamage(
                    enemy.Data.MaxHP);

                Assert.IsTrue(enemy.IsDead);
                Assert.IsTrue(health.IsDead);
                Assert.IsFalse(enemy.gameObject.activeSelf);

                Assert.AreEqual(
                    cycle + 1,
                    deathCallbacks);

                Assert.AreEqual(0, registry.Count);
                Assert.AreEqual(0, pool.CountActive);
                Assert.AreEqual(1, pool.CountInactive);

                Assert.IsFalse(collider.enabled);
                Assert.IsFalse(enemy.Movement.enabled);
                Assert.IsNull(enemy.Movement.Target);

                Assert.AreEqual(
                    Vector2.zero,
                    body.linearVelocity);

                Assert.AreEqual(
                    0,
                    CountHealthDeathHandlers(health, enemy));

                Assert.IsTrue(
                    float.IsPositiveInfinity(
                        GetContactTimer(enemy)));
            }

            Assert.AreEqual(3, deathCallbacks);
            Assert.AreEqual(1, pool.CountAll);
        }

        private void HandleEnemyDied(
            EnemyController enemy)
        {
            deathCallbacks++;

            enemy.Died -= HandleEnemyDied;
            registry.Unregister(enemy);

            Assert.IsTrue(
                pool.Release(enemy));
        }

        private static float GetContactTimer(
            EnemyController enemy)
        {
            FieldInfo field =
                typeof(EnemyController).GetField(
                    "nextContactDamageAttemptTime",
                    PrivateInstance);

            Assert.IsNotNull(field);

            return (float)field.GetValue(enemy);
        }

        private static int CountHealthDeathHandlers(
            Health health,
            EnemyController enemy)
        {
            FieldInfo field =
                typeof(Health).GetField(
                    "Died",
                    PrivateInstance);

            Assert.IsNotNull(field);

            Delegate callbacks =
                field.GetValue(health) as Delegate;

            if (callbacks == null)
            {
                return 0;
            }

            int count = 0;

            foreach (Delegate callback
                     in callbacks.GetInvocationList())
            {
                if (ReferenceEquals(callback.Target, enemy) &&
                    callback.Method.Name == "HandleDeath")
                {
                    count++;
                }
            }

            return count;
        }
    }
}
