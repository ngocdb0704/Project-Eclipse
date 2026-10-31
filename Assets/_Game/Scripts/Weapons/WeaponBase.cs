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

        public bool IsInitialized { get; private set; }

        public float CooldownRemaining => cooldownRemaining;

        public float EffectiveCooldown
        {
            get
            {
                if (!IsInitialized)
                {
                    return 0f;
                }

                float baseCooldown =
                    Mathf.Max(0f, data.Cooldown);

                float attackSpeed =
                    Mathf.Max(
                        MinimumAttackSpeedMultiplier,
                        playerStats.AttackSpeedMultiplier);

                float cooldownReduction =
                    Mathf.Clamp01(
                        playerStats.CooldownReduction);

                return Mathf.Max(
                    0f,
                    baseCooldown *
                    (1f - cooldownReduction) /
                    attackSpeed);
            }
        }

        public void Initialize(
            WeaponData weaponData,
            PlayerStats stats)
        {
            if (weaponData == null)
            {
                throw new ArgumentNullException(
                    nameof(weaponData));
            }

            data = weaponData;
            playerStats =
                stats ?? new PlayerStats();

            cooldownRemaining = 0f;
            IsInitialized = true;

            OnInitialized();
        }

        public void Tick(float deltaTime)
        {
            if (!IsInitialized)
            {
                return;
            }

            if (!IsFinite(deltaTime) ||
                deltaTime < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deltaTime),
                    "Delta time must be finite and non-negative.");
            }

            cooldownRemaining =
                Mathf.Max(
                    0f,
                    cooldownRemaining - deltaTime);

            if (cooldownRemaining > 0f)
            {
                return;
            }

            if (Attack())
            {
                cooldownRemaining =
                    EffectiveCooldown;
            }
        }

        protected virtual void OnInitialized()
        {
        }

        protected abstract bool Attack();

        private static bool IsFinite(float value)
        {
            return
                !float.IsNaN(value) &&
                !float.IsInfinity(value);
        }
    }
}