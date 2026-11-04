using System;
using Eclipse.Combat;
using UnityEngine;
using Eclipse.Core.Pooling;

namespace Eclipse.Enemies
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(EnemyMovement))]
    public sealed class EnemyController :
    MonoBehaviour,
    IDamageable,
    IPoolable
    {
        private const float ContactDamageRetryInterval = 0.1f;

        [SerializeField]
        private EnemyData enemyData;

        private Health health;
        private EnemyMovement movement;
        private Collider2D collisionCollider;

        private bool hasBeenSpawnedFromPool;

        private int playerLayer;
        private float nextContactDamageAttemptTime;

        public EnemyData Data => enemyData;

        public IHealth Health => health;

        public EnemyMovement Movement => movement;

        public bool IsDead { get; private set; }

        public event Action<EnemyController> Died;

        private bool healthDeathSubscribed;

        private void Awake()
        {
            health = GetComponent<Health>();
            movement = GetComponent<EnemyMovement>();
            collisionCollider = GetComponent<Collider2D>();

            playerLayer = LayerMask.NameToLayer("Player");

            if (enemyData == null)
            {
                Debug.LogError(
                    "EnemyController requires EnemyData.",
                    this);

                enabled = false;
                return;
            }

            if (collisionCollider == null)
            {
                Debug.LogError(
                    "EnemyController requires Collider2D.",
                    this);

                enabled = false;
                return;
            }

            if (playerLayer < 0)
            {
                Debug.LogError(
                    "EnemyController requires the Player physics layer.",
                    this);

                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (health == null || healthDeathSubscribed)
            {
                return;
            }

            health.Died += HandleDeath;
            healthDeathSubscribed = true;
        }

        private void OnDisable()
        {
            UnsubscribeFromHealth();
        }

        private void OnDestroy()
        {
            UnsubscribeFromHealth();
        }

        private void UnsubscribeFromHealth()
        {
            if (!healthDeathSubscribed)
            {
                return;
            }

            if (health != null)
            {
                health.Died -= HandleDeath;
            }

            healthDeathSubscribed = false;
        }

        private void Start()
        {
            if (!enabled || hasBeenSpawnedFromPool)
            {
                return;
            }

            // Preserve initialization for standalone,
            // non-pooled enemies used by existing tests.
            IsDead = false;
            nextContactDamageAttemptTime = 0f;

            health.Initialize(enemyData.MaxHP);
            movement.Initialize(enemyData.MoveSpeed);
        }

        public void Initialize(Transform target)
        {
            if (IsDead)
            {
                return;
            }

            if (target == null)
            {
                Debug.LogError(
                    "EnemyController requires a chase target.",
                    this);

                movement.SetTarget(null);
                return;
            }

            movement.SetTarget(target);
        }

        public void TakeDamage(DamageContext context)
        {
            if (!isActiveAndEnabled || IsDead)
            {
                return;
            }

            if (health == null)
            {
                health = GetComponent<Health>();
            }

            if (health == null)
            {
                return;
            }

            health.TakeDamage(context.FinalDamage);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryApplyContactDamage(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TryApplyContactDamage(collision);
        }

        private void TryApplyContactDamage(Collision2D collision)
        {
            if (!enabled ||
                IsDead ||
                enemyData == null ||
                collision.gameObject.layer != playerLayer ||
                Time.time < nextContactDamageAttemptTime)
            {
                return;
            }

            Health targetHealth =
                collision.collider.GetComponentInParent<Health>();

            if (targetHealth == null)
            {
                return;
            }

            nextContactDamageAttemptTime =
                Time.time + ContactDamageRetryInterval;

            targetHealth.TakeDamage(enemyData.Damage);
        }

        private void HandleDeath()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;

            nextContactDamageAttemptTime =
                float.PositiveInfinity;

            movement.Stop();
            collisionCollider.enabled = false;

            Died?.Invoke(this);

            // Fallback for enemies not managed by a pool.
            // Pooled enemies have already been released
            // by EnemySpawner's Died listener.
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        public void OnSpawned()
        {
            hasBeenSpawnedFromPool = true;

            IsDead = false;
            nextContactDamageAttemptTime = 0f;

            // Clears target and Rigidbody2D velocity.
            movement.Stop();

            // Restores health and clears damage immunity.
            health.Initialize(enemyData.MaxHP);
            health.ConfigureInvulnerability(0f);

            collisionCollider.enabled = true;
            movement.Initialize(enemyData.MoveSpeed);
        }

        public void OnDespawned()
        {
            IsDead = true;
            nextContactDamageAttemptTime =
                float.PositiveInfinity;

            movement.Stop();
            collisionCollider.enabled = false;
        }
    }
}