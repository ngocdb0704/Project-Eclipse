
using System;
using UnityEngine;

namespace Eclipse.Weapons
{
    public enum WeaponType
    {
        Projectile = 0,
        Area = 1,
        Targeted = 2,
        Orbiting = 3
    }

    [Serializable]
    public sealed class WeaponLevelStats
    {
        [SerializeField, Min(0.01f)]
        private float damageMultiplier = 1f;

        [SerializeField, Min(0.01f)]
        private float cooldownMultiplier = 1f;

        [SerializeField, Min(0)]
        private int extraProjectiles;

        public float DamageMultiplier =>
            Mathf.Max(0.01f, damageMultiplier);

        public float CooldownMultiplier =>
            Mathf.Max(0.01f, cooldownMultiplier);

        public int ExtraProjectiles =>
            Mathf.Max(0, extraProjectiles);
    }

    [CreateAssetMenu(
        fileName = "WeaponData",
        menuName = "Eclipse/Weapons/Weapon Data")]
    public sealed class WeaponData : ScriptableObject
    {
        private static readonly WeaponLevelStats NeutralLevelStats =
            new WeaponLevelStats();

        [Header("Identity")]
        [SerializeField]
        private string id = "weapon";

        [SerializeField]
        private string displayName = "Weapon";

        [SerializeField]
        private Sprite icon;

        [Header("Prefab")]
        [SerializeField]
        private GameObject prefab;

        [Header("Combat")]
        [SerializeField, Min(0f)]
        private float damage = 10f;

        [SerializeField, Min(0f)]
        private float cooldown = 1f;

        [SerializeField, Min(1)]
        private int projectileCount = 1;

        [SerializeField, Min(0f)]
        private float projectileSpeed = 8f;

        [SerializeField, Min(0.01f)]
        private float projectileSize = 1f;

        [SerializeField, Min(0)]
        private int pierce;

        [SerializeField, Min(0f)]
        private float knockback;

        [Header("Progression")]
        [SerializeField, Min(1)]
        private int maxLevel = 8;

        [Tooltip(
            "Index 0 = Level 1, Index 1 = Level 2, etc. " +
            "Each entry contains the TOTAL modifiers for that level.")]
        [SerializeField]
        private WeaponLevelStats[] levelStats =
            new WeaponLevelStats[0];

        [Header("Type")]
        [SerializeField]
        private WeaponType weaponType =
            WeaponType.Projectile;

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public GameObject Prefab => prefab;

        public float Damage => damage;
        public float Cooldown => cooldown;
        public int ProjectileCount => projectileCount;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileSize => projectileSize;
        public int Pierce => pierce;
        public float Knockback => knockback;

        public int MaxLevel => Mathf.Max(1, maxLevel);
        public WeaponType WeaponType => weaponType;

        public WeaponLevelStats GetLevelStats(int level)
        {
            if (level < 1 || level > MaxLevel)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(level));
            }

            if (levelStats == null || levelStats.Length == 0)
                return NeutralLevelStats;

            // Use the nearest configured previous level
            // when higher levels have no explicit data.
            int index = Mathf.Min(
                level - 1,
                levelStats.Length - 1);

            for (int i = index; i >= 0; i--)
            {
                if (levelStats[i] != null)
                    return levelStats[i];
            }

            return NeutralLevelStats;
        }
    }
}
