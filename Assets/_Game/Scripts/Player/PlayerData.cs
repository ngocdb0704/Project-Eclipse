using UnityEngine;

namespace Eclipse.Player
{
    [CreateAssetMenu(
        fileName = "PlayerData",
        menuName = "Eclipse/Player/Player Data")]
    public sealed class PlayerData : ScriptableObject
    {
        [Header("Core")]
        [SerializeField, Min(1f)]
        private float maxHealth = PlayerStats.DefaultMaxHealth;

        [SerializeField, Min(0f)]
        private float moveSpeed = PlayerStats.DefaultMoveSpeed;

        [Header("Combat")]
        [SerializeField, Min(0f)]
        private float damageMultiplier =
            PlayerStats.DefaultDamageMultiplier;

        [SerializeField, Min(0f)]
        private float attackSpeedMultiplier =
            PlayerStats.DefaultAttackSpeedMultiplier;

        [SerializeField, Range(0f, 1f)]
        private float cooldownReduction =
            PlayerStats.DefaultCooldownReduction;

        [SerializeField, Range(0f, 1f)]
        private float criticalChance =
            PlayerStats.DefaultCriticalChance;

        [SerializeField, Min(1f)]
        private float criticalDamage =
            PlayerStats.DefaultCriticalDamage;

        [SerializeField, Min(0f)]
        private float armor = PlayerStats.DefaultArmor;

        [Header("Utility")]
        [SerializeField, Min(0f)]
        private float pickupRadius =
            PlayerStats.DefaultPickupRadius;

        [SerializeField, Min(1)]
        private int projectileCount =
            PlayerStats.DefaultProjectileCount;

        [SerializeField, Min(0f)]
        private float projectileSpeed =
            PlayerStats.DefaultProjectileSpeed;

        [SerializeField, Min(0f)]
        private float projectileSize =
            PlayerStats.DefaultProjectileSize;

        [SerializeField, Min(0f)]
        private float durationMultiplier =
            PlayerStats.DefaultDurationMultiplier;

        [SerializeField, Min(0f)]
        private float luck = PlayerStats.DefaultLuck;

        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;

        public float DamageMultiplier => damageMultiplier;
        public float AttackSpeedMultiplier => attackSpeedMultiplier;
        public float CooldownReduction => cooldownReduction;

        public float CriticalChance => criticalChance;
        public float CriticalDamage => criticalDamage;
        public float Armor => armor;

        public float PickupRadius => pickupRadius;

        public int ProjectileCount => projectileCount;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileSize => projectileSize;

        public float DurationMultiplier => durationMultiplier;
        public float Luck => luck;
    }
}