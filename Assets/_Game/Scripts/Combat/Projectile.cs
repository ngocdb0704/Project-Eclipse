using System;
using UnityEngine;

namespace Eclipse.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class Projectile : MonoBehaviour
    {
        private Rigidbody2D body;
        private Collider2D projectileCollider;

        private DamageContext damageContext;
        private Action<Projectile> releaseCallback;

        private Collider2D lastHitCollider;

        private float remainingLifetime;
        private bool isInitialized;
        private bool isReleased;

        public Vector2 Direction { get; private set; }

        public float Speed { get; private set; }

        public float RemainingLifetime =>
            remainingLifetime;

        public bool IsInitialized =>
            isInitialized;

        public bool IsReleased =>
            isReleased;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            projectileCollider = GetComponent<Collider2D>();

            if (projectileCollider == null)
            {
                Debug.LogError(
                    "Projectile requires a Collider2D.",
                    this);

                enabled = false;
                return;
            }

            if (!projectileCollider.isTrigger)
            {
                Debug.LogError(
                    "Projectile Collider2D must be configured as a trigger.",
                    this);

                enabled = false;
            }
        }

        private void Update()
        {
            if (!isInitialized ||
                isReleased)
            {
                return;
            }

            remainingLifetime -=
                Time.deltaTime;

            if (remainingLifetime <= 0f)
            {
                Release();
            }
        }

        public void Initialize(
            Vector2 direction,
            float speed,
            float lifetime,
            DamageContext context,
            Action<Projectile> onRelease = null)
        {
            if (!IsFinite(direction.x) ||
                !IsFinite(direction.y) ||
                direction.sqrMagnitude <=
                Mathf.Epsilon)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(direction),
                    "Direction must be finite and non-zero.");
            }

            if (!IsFinite(speed) ||
                speed < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(speed),
                    "Speed must be finite and non-negative.");
            }

            if (!IsFinite(lifetime) ||
                lifetime <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(lifetime),
                    "Lifetime must be finite and greater than zero.");
            }

            Direction = direction.normalized;
            Speed = speed;

            remainingLifetime = lifetime;
            damageContext = context;
            releaseCallback = onRelease;

            lastHitCollider = null;
            isReleased = false;
            isInitialized = true;

            body.linearVelocity =
                Direction * Speed;
        }

        public void Release()
        {
            if (isReleased)
            {
                return;
            }

            isReleased = true;
            isInitialized = false;

            body.linearVelocity =
                Vector2.zero;

            Action<Projectile> callback =
                releaseCallback;

            releaseCallback = null;

            if (callback != null)
            {
                callback(this);
                return;
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(
            Collider2D other)
        {
            if (!isInitialized ||
                isReleased ||
                other == null ||
                other == lastHitCollider)
            {
                return;
            }

            IDamageable damageable =
                other.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                return;
            }

            lastHitCollider = other;

            damageable.TakeDamage(
                damageContext);

            Release();
        }

        private static bool IsFinite(
            float value)
        {
            return
                !float.IsNaN(value) &&
                !float.IsInfinity(value);
        }
    }
}