using UnityEngine;

namespace Eclipse.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class EnemyMovement : MonoBehaviour
    {
        public float MoveSpeed { get; private set; }

        public void Initialize(float moveSpeed)
        {
            MoveSpeed = Mathf.Max(0f, moveSpeed);
        }
    }
}