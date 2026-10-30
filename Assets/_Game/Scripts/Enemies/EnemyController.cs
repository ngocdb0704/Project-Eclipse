using Eclipse.Combat;
using UnityEngine;

namespace Eclipse.Enemies
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(EnemyMovement))]
    public sealed class EnemyController : MonoBehaviour
    {
        private const float ContactDamageRetryInterval = 0.1f;

        [SerializeField]
        private EnemyData enemyData;

        private Health health;
        private EnemyMovement movement;

        private int playerLayer;
        private float nextContactDamageAttemptTime;

        public EnemyData Data => enemyData;

        public IHealth Health => health;

        public EnemyMovement Movement => movement;

        private void Awake()
        {
            health = GetComponent<Health>();
            movement = GetComponent<EnemyMovement>();

            playerLayer = LayerMask.NameToLayer("Player");

            if (enemyData == null)
            {
                Debug.LogError(
                    "EnemyController requires EnemyData.",
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

        private void Start()
        {
            if (!enabled)
            {
                return;
            }

            health.Initialize(enemyData.MaxHP);
            movement.Initialize(enemyData.MoveSpeed);
        }

        public void Initialize(Transform target)
        {
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
    }
}