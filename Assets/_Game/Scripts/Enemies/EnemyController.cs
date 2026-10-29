using Eclipse.Combat;
using UnityEngine;

namespace Eclipse.Enemies
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(EnemyMovement))]
    public sealed class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private EnemyData enemyData;

        private Health health;
        private EnemyMovement movement;

        public EnemyData Data => enemyData;

        public IHealth Health => health;

        public EnemyMovement Movement => movement;

        private void Awake()
        {
            health = GetComponent<Health>();
            movement = GetComponent<EnemyMovement>();

            if (enemyData == null)
            {
                Debug.LogError(
                    "EnemyController requires EnemyData.",
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
    }
}