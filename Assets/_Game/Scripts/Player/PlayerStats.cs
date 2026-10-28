namespace Eclipse.Player
{
    /// <summary>
    /// Runtime stat values for the player.
    /// Configuration assets will provide base values in a later task.
    /// </summary>
    public sealed class PlayerStats
    {
        public const float DefaultMaxHealth = 100f;
        public const float DefaultMoveSpeed = 5f;

        public const float DefaultDamageMultiplier = 1f;
        public const float DefaultAttackSpeedMultiplier = 1f;
        public const float DefaultCooldownReduction = 0f;

        public const float DefaultCriticalChance = 0.05f;
        public const float DefaultCriticalDamage = 2f;

        public const float DefaultArmor = 0f;
        public const float DefaultPickupRadius = 1.5f;

        public const int DefaultProjectileCount = 1;
        public const float DefaultProjectileSpeed = 1f;
        public const float DefaultProjectileSize = 1f;

        public const float DefaultDurationMultiplier = 1f;
        public const float DefaultLuck = 1f;

        public float MaxHealth { get; private set; }
        public float MoveSpeed { get; private set; }

        public float DamageMultiplier { get; private set; }
        public float AttackSpeedMultiplier { get; private set; }
        public float CooldownReduction { get; private set; }

        public float CriticalChance { get; private set; }
        public float CriticalDamage { get; private set; }

        public float Armor { get; private set; }
        public float PickupRadius { get; private set; }

        public int ProjectileCount { get; private set; }
        public float ProjectileSpeed { get; private set; }
        public float ProjectileSize { get; private set; }

        public float DurationMultiplier { get; private set; }
        public float Luck { get; private set; }

        public PlayerStats()
        {
            ResetToDefaults();
        }

        public void ResetToDefaults()
        {
            MaxHealth = DefaultMaxHealth;
            MoveSpeed = DefaultMoveSpeed;

            DamageMultiplier = DefaultDamageMultiplier;
            AttackSpeedMultiplier = DefaultAttackSpeedMultiplier;
            CooldownReduction = DefaultCooldownReduction;

            CriticalChance = DefaultCriticalChance;
            CriticalDamage = DefaultCriticalDamage;

            Armor = DefaultArmor;
            PickupRadius = DefaultPickupRadius;

            ProjectileCount = DefaultProjectileCount;
            ProjectileSpeed = DefaultProjectileSpeed;
            ProjectileSize = DefaultProjectileSize;

            DurationMultiplier = DefaultDurationMultiplier;
            Luck = DefaultLuck;
        }
    }
}