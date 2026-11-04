using Eclipse.World;
using UnityEngine;
using System;
using Eclipse.Core.Pooling;

namespace Eclipse.Enemies
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        private const float MinimumSpawnInterval = 0.05f;

        [Header("References")]
        [SerializeField]
        private EnemyController enemyPrefab;

        [SerializeField]
        private Camera spawnCamera;

        [SerializeField]
        private Transform playerTarget;

        [SerializeField]
        private Transform spawnParent;

        [SerializeField]
        private EnemyRegistry enemyRegistry;

        [Header("Spawn")]
        [SerializeField, Min(MinimumSpawnInterval)]
        private float spawnInterval = 1f;

        [SerializeField, Min(0f)]
        private float spawnMargin = 2f;

        [SerializeField]
        private int randomSeed = 12345;

        private System.Random random;
        private float spawnTimer;

        [Header("Pooling")]
        [SerializeField, Min(0)]
        private int prewarmCount = 16;

        private GameObjectPool<EnemyController> enemyPool;

        private Action<EnemyController> enemyDiedHandler;

        private void Awake()
        {
            random = new System.Random(randomSeed);

            // Cached once; no new delegate per enemy spawn.
            enemyDiedHandler = HandleEnemyDied;
        }

        private void Start()
        {
            if (enemyPrefab == null || enemyPrefab.Data == null)
            {
                Debug.LogError(
                    "EnemySpawner requires a valid Enemy prefab.",
                    this);

                enabled = false;
                return;
            }

            Transform parent =
                spawnParent != null
                    ? spawnParent
                    : transform;

            enemyPool =
                new GameObjectPool<EnemyController>(
                    enemyPrefab,
                    parent);

            enemyPool.Prewarm(
                Mathf.Max(0, prewarmCount));
        }

        private void OnEnable()
        {
            spawnTimer = 0f;
        }

        private void OnDisable()
        {
            spawnTimer = 0f;
        }

        private void OnValidate()
        {
            spawnInterval =
                Mathf.Max(
                    MinimumSpawnInterval,
                    spawnInterval);

            spawnMargin =
                Mathf.Max(0f, spawnMargin);

            prewarmCount = Mathf.Max(0, prewarmCount);
        }

        private void Update()
        {
            if (!CanSpawn())
            {
                spawnTimer = 0f;
                return;
            }

            spawnTimer += Time.deltaTime;

            if (spawnTimer < spawnInterval)
            {
                return;
            }

            spawnTimer -= spawnInterval;

            SpawnEnemy();
        }

        private bool CanSpawn()
        {
            return
                enemyPrefab != null &&
                enemyPrefab.Data != null &&
                spawnCamera != null &&
                spawnCamera.isActiveAndEnabled &&
                spawnCamera.orthographic &&
                spawnCamera.orthographicSize > 0f &&
                spawnCamera.aspect > 0f &&
                playerTarget != null &&
                playerTarget.gameObject.activeInHierarchy &&
                enemyRegistry != null &&
                enemyPool != null &&
                enemyRegistry.isActiveAndEnabled;
        }

        private void SpawnEnemy()
        {
            float halfHeight =
                spawnCamera.orthographicSize;

            float halfWidth =
                halfHeight * spawnCamera.aspect;

            Vector3 cameraPosition =
                spawnCamera.transform.position;

            Vector2 spawnPosition =
                SpawnPositionGenerator.Generate(
                    new Vector2(
                        cameraPosition.x,
                        cameraPosition.y),
                    new Vector2(
                        halfWidth,
                        halfHeight),
                    spawnMargin,
                    random);

            EnemyController enemy =
    CreateEnemy(spawnPosition);

            enemy.Initialize(playerTarget);

            if (!enemyRegistry.Register(enemy))
            {
                Debug.LogError(
                    "Failed to register pooled enemy.",
                    enemy);

                enemyPool.Release(enemy);
                return;
            }

            // Register first, then subscribe to death.
            // Registry must unregister before pool release.
            enemy.Died += enemyDiedHandler;
        }

        private EnemyController CreateEnemy(
    Vector2 spawnPosition)
        {
            return enemyPool.Get(
                spawnPosition,
                Quaternion.identity);
        }

        private void HandleEnemyDied(
    EnemyController enemy)
        {
            if (enemy == null || enemyPool == null)
            {
                return;
            }

            enemy.Died -= enemyDiedHandler;

            // Defensive cleanup. If the registry already
            // handled Died, this simply returns false.
            enemyRegistry.Unregister(enemy);

            if (!enemyPool.Release(enemy))
            {
                Debug.LogError(
                    "Enemy pool rejected release.",
                    enemy);
            }
        }
    }
}