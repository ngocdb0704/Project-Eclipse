
using System;
using System.Collections.Generic;
using Eclipse.Core.Pooling;
using Eclipse.Enemies;
using UnityEngine;

namespace Eclipse.Debugging
{
    public sealed class EnemyStressTestController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyController enemyPrefab;
        [SerializeField] private EnemyRegistry enemyRegistry;
        [SerializeField] private Transform dummyTarget;

        [Header("Stress Configuration")]
        [SerializeField, Min(1)] private int enemyCount = 250;
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private int randomSeed = 12345;

        [Header("Spawn Distribution")]
        [SerializeField, Min(0f)]
        private float minSpawnRadius = 12f;

        [SerializeField, Min(0f)]
        private float maxSpawnRadius = 18f;

        [Header("Dummy Target")]
        [SerializeField] private bool moveTarget = true;

        [SerializeField, Min(0f)]
        private float orbitRadius = 5f;

        [SerializeField]
        private float orbitDegreesPerSecond = 25f;

        private GameObjectPool<EnemyController> pool;
        private Transform poolRoot;
        private Vector3 targetStartPosition;

        private Action<EnemyController> deathCallback;

        private readonly HashSet<EnemyController> activeEnemies =
            new HashSet<EnemyController>();

        private void Awake()
        {
            if (enemyPrefab == null ||
                enemyRegistry == null ||
                dummyTarget == null)
            {
                Debug.LogError(
                    "Stress test requires enemy prefab, registry and dummy target.",
                    this);

                enabled = false;
                return;
            }

            deathCallback = HandleEnemyDied;

            GameObject root = new GameObject("Enemy Stress Pool");
            poolRoot = root.transform;
            poolRoot.SetParent(transform, false);

            pool = new GameObjectPool<EnemyController>(
                enemyPrefab,
                poolRoot);

            pool.Prewarm(enemyCount);
        }

        private void Start()
        {
            if (pool == null)
                return;

            targetStartPosition = dummyTarget.position;

            if (spawnOnStart)
                SpawnConfiguredEnemies();
        }

        private void Update()
        {
            if (!moveTarget || dummyTarget == null)
                return;

            float angle =
                Time.time * orbitDegreesPerSecond *
                Mathf.Deg2Rad;

            dummyTarget.position =
                targetStartPosition +
                new Vector3(
                    Mathf.Cos(angle) * orbitRadius,
                    Mathf.Sin(angle) * orbitRadius,
                    0f);
        }

        [ContextMenu("Spawn Configured Enemies")]
        public void SpawnConfiguredEnemies()
        {
            if (!Application.isPlaying || pool == null)
            {
                Debug.LogWarning(
                    "Enter Play Mode before spawning enemies.",
                    this);
                return;
            }

            if (activeEnemies.Count > 0)
            {
                Debug.LogWarning(
                    "Clear active enemies before spawning a new batch.",
                    this);
                return;
            }

            pool.Prewarm(enemyCount);

            var random = new System.Random(randomSeed);
            Vector3 center = transform.position;

            for (int i = 0; i < enemyCount; i++)
            {
                float angle =
                    Mathf.PI * 2f * i / enemyCount;

                float radius = Mathf.Lerp(
                    minSpawnRadius,
                    maxSpawnRadius,
                    (float)random.NextDouble());

                Vector3 position = center +
                    new Vector3(
                        Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle) * radius,
                        0f);

                EnemyController enemy = pool.Get(
                    position,
                    Quaternion.identity);

                enemy.Initialize(dummyTarget);

                if (!enemyRegistry.Register(enemy))
                {
                    pool.Release(enemy);

                    Debug.LogError(
                        "Stress test failed to register an enemy.",
                        this);

                    continue;
                }

                enemy.Died += deathCallback;
                activeEnemies.Add(enemy);
            }

            PrintCounts();
        }

        private void HandleEnemyDied(EnemyController enemy)
        {
            if (enemy == null)
                return;

            enemy.Died -= deathCallback;
            activeEnemies.Remove(enemy);

            if (enemyRegistry != null)
                enemyRegistry.Unregister(enemy);

            if (pool != null && !pool.Release(enemy))
            {
                Debug.LogError(
                    "Stress pool rejected enemy release.",
                    this);
            }
        }

        [ContextMenu("Clear Active Enemies")]
        public void ClearActiveEnemies()
        {
            foreach (EnemyController enemy in activeEnemies)
            {
                if (enemy == null)
                    continue;

                enemy.Died -= deathCallback;

                if (enemyRegistry != null)
                    enemyRegistry.Unregister(enemy);

                if (pool != null)
                    pool.Release(enemy);
            }

            activeEnemies.Clear();
        }

        [ContextMenu("Print Stress Counts")]
        public void PrintCounts()
        {
            if (pool == null)
                return;

            Debug.Log(
                $"Stress Test | " +
                $"Active={pool.CountActive} | " +
                $"Inactive={pool.CountInactive} | " +
                $"Total={pool.CountAll} | " +
                $"Registry={enemyRegistry.Count}",
                this);
        }

        private void OnValidate()
        {
            enemyCount = Mathf.Max(1, enemyCount);

            minSpawnRadius =
                Mathf.Max(0f, minSpawnRadius);

            maxSpawnRadius =
                Mathf.Max(minSpawnRadius, maxSpawnRadius);
        }

        private void OnDestroy()
        {
            ClearActiveEnemies();

            if (poolRoot != null)
            {
                Destroy(poolRoot.gameObject);
                poolRoot = null;
            }
        }
    }
}
