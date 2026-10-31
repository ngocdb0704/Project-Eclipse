using Eclipse.World;
using UnityEngine;

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

        private void Awake()
        {
            random = new System.Random(randomSeed);
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

            enemyRegistry.Register(enemy);
        }

        private EnemyController CreateEnemy(
            Vector2 spawnPosition)
        {
            return Instantiate(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity,
                spawnParent);
        }
    }
}