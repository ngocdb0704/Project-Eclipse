
using System;
using Eclipse.Player;
using UnityEngine;

namespace Eclipse.Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        private const float MinimumAttackSpeedMultiplier = 0.01f;

        private WeaponData data;
        private PlayerStats playerStats;
        private float cooldownRemaining;

        public WeaponData Data => data;

        protected PlayerStats RuntimeStats => playerStats;

        protected WeaponLevelStats CurrentLevelStats =>
            data.GetLevelStats(Level);

        public bool IsInitialized { get; private set; }

        public int Level { get; private set; }

        public int MaxLevel =>
            IsInitialized ? data.MaxLevel : 0;

        public bool CanUpgrade =>
            IsInitialized && Level < MaxLevel;

        public event Action<int> LevelChanged;

        public float CooldownRemaining => cooldownRemaining;

        public float EffectiveDamage
        {
            get
            {
                if (!IsInitialized)
                    return 0f;

                return Mathf.Max(
                    0f,
                    data.Damage *
                    playerStats.DamageMultiplier *
                    CurrentLevelStats.DamageMultiplier);
            }
        }

        public int EffectiveProjectileCount
        {
            get
            {
                if (!IsInitialized)
                    return 0;

                int playerBonus = Mathf.Max(
                    0,
                    playerStats.ProjectileCount -
                    PlayerStats.DefaultProjectileCount);

                return Mathf.Max(
                    1,
                    data.ProjectileCount +
                    playerBonus +
                    CurrentLevelStats.ExtraProjectiles);
            }
        }

        public float EffectiveCooldown
        {
            get
            {
                if (!IsInitialized)
                    return 0f;

                float baseCooldown =
                    Mathf.Max(0f, data.Cooldown);

                float attackSpeed = Mathf.Max(
                    MinimumAttackSpeedMultiplier,
                    playerStats.AttackSpeedMultiplier);

                float cooldownReduction =
                    Mathf.Clamp01(
                        playerStats.CooldownReduction);

                return Mathf.Max(
                    0f,
                    baseCooldown *
                    CurrentLevelStats.CooldownMultiplier *
                    (1f - cooldownReduction) /
                    attackSpeed);
            }
        }

        public void Initialize(
            WeaponData weaponData,
            PlayerStats stats)
        {
            if (weaponData == null)
                throw new ArgumentNullException(
                    nameof(weaponData));

            data = weaponData;
            playerStats = stats ?? new PlayerStats();

            Level = 1;
            cooldownRemaining = 0f;
            IsInitialized = true;

            OnInitialized();
        }

        public bool TryUpgrade()
        {
            if (!CanUpgrade)
                return false;

            Level++;

            LevelChanged?.Invoke(Level);

            return true;
        }

        public void Tick(float deltaTime)
        {
            if (!IsInitialized)
                return;

            if (!IsFinite(deltaTime) ||
                deltaTime < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deltaTime),
                    "Delta time must be finite and non-negative.");
            }

            cooldownRemaining = Mathf.Max(
                0f,
                cooldownRemaining - deltaTime);

            if (cooldownRemaining > 0f)
                return;

            if (Attack())
                cooldownRemaining = EffectiveCooldown;
        }

        protected virtual void OnInitialized()
        {
        }

        protected abstract bool Attack();

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) &&
                   !float.IsInfinity(value);
        }
    }
}
