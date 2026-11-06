using System;
using Eclipse.Combat;
using Eclipse.Enemies;
using Eclipse.Player;
using Eclipse.Weapons.Targeting;
using UnityEngine;
using Eclipse.Core.Pooling;

namespace Eclipse.Weapons
{
    public sealed class MagicBoltWeapon : WeaponBase
    {
        [SerializeField]
        private Projectile projectilePrefab;

        [SerializeField, Min(0.1f)]
        private float projectileLifetime = 5f;

        private PlayerController owner;
        private EnemyRegistry enemyRegistry;

        private const int ProjectilePoolPrewarmCount = 32;


        private const float DegreesPerProjectile = 12f;
        private const float MaximumFanAngle = 60f;

        private GameObjectPool<Projectile> projectilePool;

        private Action<Projectile> projectileReleaseCallback;

        public void InitializeForPlayer(
            WeaponData weaponData,
            PlayerController player,
            EnemyRegistry registry)
        {
            if (player == null)
            {
                throw new ArgumentNullException(
                    nameof(player));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(
                    nameof(registry));
            }

            if (player.Stats == null)
            {
                throw new InvalidOperationException(
                    "Player stats are not initialized.");
            }

            if (projectilePrefab == null)
            {
                throw new InvalidOperationException(
                    "Magic Bolt requires a projectile prefab.");
            }

            owner = player;
            enemyRegistry = registry;

            if (projectilePool == null)
            {
                projectilePool =
                    new GameObjectPool<Projectile>(
                        projectilePrefab);

                projectilePool.Prewarm(
                    ProjectilePoolPrewarmCount);

                projectileReleaseCallback =
    ReleaseProjectile;
            }



            transform.SetParent(
                player.transform,
                false);

            base.Initialize(
                weaponData,
                player.Stats);
        }

        private void ReleaseProjectile(
    Projectile projectile)
        {
            if (!projectilePool.Release(projectile))
            {
                Debug.LogWarning(
                    "Projectile release was rejected by its pool.",
                    projectile);
            }
        }

        private void Update()
        {
            Tick(Time.deltaTime);
        }

        protected override bool Attack()
        {
            if (owner == null ||
                enemyRegistry == null ||
                projectilePrefab == null)
            {
                return false;
            }

            Vector2 origin =
                transform.position;

            if (!NearestTargetFinder.TryFindNearest(
                    enemyRegistry,
                    origin,
                    out EnemyController target))
            {
                return false;
            }

            Vector2 direction =
                (Vector2)target.transform.position -
                origin;

            if (direction.sqrMagnitude <=
                Mathf.Epsilon)
            {
                direction = Vector2.right;
            }
            else
            {
                direction.Normalize();
            }

            float damage = EffectiveDamage;

            float speed =
                Mathf.Max(
                    0f,
                    Data.ProjectileSpeed *
                    RuntimeStats.ProjectileSpeed);

            int projectileCount = EffectiveProjectileCount;


            float angleStep = projectileCount > 1
                ? Mathf.Min(
                    DegreesPerProjectile,
                    MaximumFanAngle / (projectileCount - 1))
                : 0f;

            float firstAngle =
                -0.5f * (projectileCount - 1) * angleStep;

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = firstAngle + i * angleStep;

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
                        DamageType.Magical);

                Projectile projectile = projectilePool.Get(
                    origin,
                    Quaternion.identity);

                projectile.Initialize(
                    shotDirection,
                    speed,
                    projectileLifetime,
                    context,
                    Data.Pierce,
                    projectileReleaseCallback);
            }


            return true;
        }
    }
}