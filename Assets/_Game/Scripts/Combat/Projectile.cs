using System;
using System.Collections.Generic;
using UnityEngine;
using Eclipse.Core.Pooling;

namespace Eclipse.Combat
{
    /// <summary>
    /// Tracks unique damageable targets hit by one projectile.
    ///
    /// Pierce convention:
    /// Pierce = 0 -> 1 total target.
    /// Pierce = 1 -> 2 total targets.
    /// Pierce = N -> N + 1 total targets.
    /// </summary>
    public sealed class ProjectileHitTracker
    {
        private readonly List<IDamageable> hitTargets =
            new List<IDamageable>(4);

        public int Pierce { get; private set; }

        public int RemainingHits { get; private set; }

        public void Reset(int pierce)
        {
            if (pierce < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pierce),
                    "Pierce must be non-negative.");
            }

            Pierce = pierce;
            RemainingHits = pierce + 1;

            hitTargets.Clear();
        }

        public void Clear()
        {
            Pierce = 0;
            RemainingHits = 0;
            hitTargets.Clear();
        }

        public bool TryRegister(
            IDamageable target)
        {
            if (target == null ||
                RemainingHits <= 0)
            {
                return false;
            }

            for (int i = 0;
                 i < hitTargets.Count;
                 i++)
            {
                if (ReferenceEquals(
                        hitTargets[i],
                        target))
                {
                    return false;
                }
            }

            hitTargets.Add(target);

            RemainingHits--;

            return true;
        }
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class Projectile :
    MonoBehaviour,
    IPoolable
    {
        private Rigidbody2D body;
        private Collider2D projectileCollider;

        private DamageContext damageContext;
        private Action<Projectile> releaseCallback;

        private readonly ProjectileHitTracker hitTracker =
            new ProjectileHitTracker();

        private float remainingLifetime;
        private bool isInitialized;
        private bool isReleased;

        public Vector2 Direction { get; private set; }

        public float Speed { get; private set; }

        public float RemainingLifetime =>
            remainingLifetime;

        public int Pierce =>
            hitTracker.Pierce;

        public int RemainingHits =>
            hitTracker.RemainingHits;

        public bool IsInitialized =>
            isInitialized;

        public bool IsReleased =>
            isReleased;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            projectileCollider =
                GetComponent<Collider2D>();

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

        /// <summary>
        /// Backward-compatible initialization.
        /// Pierce defaults to zero, meaning one total hit.
        /// </summary>
        public void Initialize(
            Vector2 direction,
            float speed,
            float lifetime,
            DamageContext context,
            Action<Projectile> onRelease = null)
        {
            Initialize(
                direction,
                speed,
                lifetime,
                context,
                0,
                onRelease);
        }

        public void Initialize(
            Vector2 direction,
            float speed,
            float lifetime,
            DamageContext context,
            int pierce,
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

            hitTracker.Reset(pierce);

            Direction = direction.normalized;
            Speed = speed;

            remainingLifetime = lifetime;
            damageContext = context;
            releaseCallback = onRelease;

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

            Debug.LogError(
                "Projectile released without a pool release callback.",
                this);

            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(
            Collider2D other)
        {
            if (!isInitialized ||
                isReleased ||
                other == null)
            {
                return;
            }

            IDamageable damageable =
                other.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                return;
            }

            if (!hitTracker.TryRegister(
                    damageable))
            {
                return;
            }

            Vector2 hitPosition =
    other.ClosestPoint(
        transform.position);

            damageable.TakeDamage(
                damageContext);

            DamageEvents.Raise(
                new DamageEventData(
                    hitPosition,
                    damageContext.FinalDamage,
                    damageContext.IsCritical));

            if (hitTracker.RemainingHits <= 0)
            {
                Release();
            }
        }

        private static bool IsFinite(
            float value)
        {
            return
                !float.IsNaN(value) &&
                !float.IsInfinity(value);
        }

        public void OnSpawned()
        {
            Direction = Vector2.zero;
            Speed = 0f;

            remainingLifetime = 0f;
            damageContext = default;
            releaseCallback = null;

            hitTracker.Clear();

            isInitialized = false;
            isReleased = false;

            if (body != null)
            {
                body.linearVelocity =
                    Vector2.zero;
            }
        }

        public void OnDespawned()
        {
            Direction = Vector2.zero;
            Speed = 0f;

            remainingLifetime = 0f;
            damageContext = default;
            releaseCallback = null;

            hitTracker.Clear();

            isInitialized = false;
            isReleased = true;

            if (body != null)
            {
                body.linearVelocity =
                    Vector2.zero;
            }
        }
    }
}