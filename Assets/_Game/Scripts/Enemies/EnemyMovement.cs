using UnityEngine;

namespace Eclipse.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class EnemyMovement : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        private Rigidbody2D body;

        public float MoveSpeed { get; private set; }

        public Transform Target => target;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void Initialize(float moveSpeed)
        {
            MoveSpeed = Mathf.Max(0f, moveSpeed);
            enabled = true;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void Stop()
        {
            target = null;

            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;

            enabled = false;
        }

        private void FixedUpdate()
        {
            if (target == null || MoveSpeed <= 0f)
            {
                return;
            }

            Vector2 nextPosition = Vector2.MoveTowards(
                body.position,
                target.position,
                MoveSpeed * Time.fixedDeltaTime);

            body.MovePosition(nextPosition);
        }
    }
}