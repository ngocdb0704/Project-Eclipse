
using System;
using Eclipse.Combat;
using Eclipse.Core.Pooling;
using Eclipse.Player;
using UnityEngine;

namespace Eclipse.Weapons
{
    public sealed class KnifeWeapon : WeaponBase
    {
        private const int PoolPrewarmCount = 16;
        private const float MovementThresholdSqr = 0.000001f;

        private const float DegreesPerKnife = 10f;
        private const float MaximumSpreadAngle = 60f;

        [SerializeField]
        private Projectile projectilePrefab;

        [SerializeField, Min(0.1f)]
        private float projectileLifetime = 3f;

        private PlayerController owner;
        private GameObjectPool<Projectile> projectilePool;
        private Action<Projectile> releaseCallback;

        private Vector2 previousOwnerPosition;
        private Vector2 lastMovementDirection = Vector2.right;

        public Vector2 LastMovementDirection =>
            lastMovementDirection;

        public void InitializeForPlayer(
            WeaponData weaponData,
            PlayerController player)
        {
            if (weaponData == null)
                throw new ArgumentNullException(
                    nameof(weaponData));

            if (player == null)
                throw new ArgumentNullException(
                    nameof(player));

            if (player.Stats == null)
                throw new InvalidOperationException(
                    "Player runtime stats are unavailable.");

            if (projectilePrefab == null)
                throw new InvalidOperationException(
                    "Knife requires a projectile prefab.");

            owner = player;

            previousOwnerPosition =
                player.transform.position;

            lastMovementDirection = Vector2.right;

            if (projectilePool == null)
            {
                projectilePool =
                    new GameObjectPool<Projectile>(
                        projectilePrefab);

                projectilePool.Prewarm(
                    PoolPrewarmCount);

                releaseCallback = ReleaseProjectile;
            }

            transform.SetParent(
                player.transform,
                false);

            base.Initialize(
                weaponData,
                player.Stats);
        }

        private void Update()
        {
            if (!IsInitialized)
                return;

            UpdateMovementDirection();

            Tick(Time.deltaTime);
        }

        private void UpdateMovementDirection()
        {
            if (owner == null)
                return;

            Vector2 currentPosition =
                owner.transform.position;

            Vector2 movement =
                currentPosition - previousOwnerPosition;

            if (movement.sqrMagnitude >
                MovementThresholdSqr)
            {
                lastMovementDirection =
                    movement.normalized;
            }

            previousOwnerPosition =
                currentPosition;
        }

        protected override bool Attack()
        {
            if (owner == null ||
                projectilePool == null)
            {
                return false;
            }

            Vector2 origin = transform.position;
            Vector2 direction = lastMovementDirection;

            float damage = EffectiveDamage;

            float speed = Mathf.Max(
                0f,
                Data.ProjectileSpeed *
                RuntimeStats.ProjectileSpeed);

            int count = EffectiveProjectileCount;

            float angleStep = count > 1
                ? Mathf.Min(
                    DegreesPerKnife,
                    MaximumSpreadAngle / (count - 1))
                : 0f;

            float firstAngle =
                -0.5f * (count - 1) * angleStep;

            for (int i = 0; i < count; i++)
            {
                float angle =
                    firstAngle + i * angleStep;

                Vector2 shotDirection = (Vector2)(
                    Quaternion.Euler(0f, 0f, angle) *
                    (Vector3)direction);

                DamageContext context =
                    DamageCalculator.Calculate(
                        owner,
                        damage,
                        RuntimeStats.CriticalChance,
                        RuntimeStats.CriticalDamage,
                        UnityEngine.Random.value,
                        shotDirection * Data.Knockback,
                        DamageType.Physical);

                float rotationDegrees =
                    Mathf.Atan2(
                        shotDirection.y,
                        shotDirection.x) *
                    Mathf.Rad2Deg;

                Quaternion rotation =
                    Quaternion.Euler(
                        0f,
                        0f,
                        rotationDegrees);

                Projectile projectile =
                    projectilePool.Get(
                        origin,
                        rotation);

                projectile.Initialize(
                    shotDirection,
                    speed,
                    projectileLifetime,
                    context,
                    Data.Pierce,
                    releaseCallback);
            }

            return true;
        }

        private void ReleaseProjectile(
            Projectile projectile)
        {
            if (!projectilePool.Release(projectile))
            {
                Debug.LogWarning(
                    "Knife projectile release was rejected.",
                    projectile);
            }
        }
    }
}
