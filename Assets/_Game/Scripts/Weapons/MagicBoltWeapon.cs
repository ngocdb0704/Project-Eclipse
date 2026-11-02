using System;
using Eclipse.Combat;
using Eclipse.Enemies;
using Eclipse.Player;
using Eclipse.Weapons.Targeting;
using UnityEngine;

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

            transform.SetParent(
                player.transform,
                false);

            base.Initialize(
                weaponData,
                player.Stats);
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

            float damage =
                Mathf.Max(
                    0f,
                    Data.Damage *
                    RuntimeStats.DamageMultiplier);

            float speed =
                Mathf.Max(
                    0f,
                    Data.ProjectileSpeed *
                    RuntimeStats.ProjectileSpeed);

            int projectileBonus =
                Mathf.Max(
                    0,
                    RuntimeStats.ProjectileCount -
                    PlayerStats.DefaultProjectileCount);

            int projectileCount =
                Mathf.Max(
                    1,
                    Data.ProjectileCount +
                    projectileBonus);

            DamageContext context =
                new DamageContext(
                    owner,
                    damage,
                    false,
                    1f,
                    direction * Data.Knockback,
                    DamageType.Magical);

            for (int i = 0;
                 i < projectileCount;
                 i++)
            {
                Projectile projectile =
                    Instantiate(
                        projectilePrefab,
                        origin,
                        Quaternion.identity);

                projectile.Initialize(
                    direction,
                    speed,
                    projectileLifetime,
                    context);
            }

            return true;
        }
    }
}