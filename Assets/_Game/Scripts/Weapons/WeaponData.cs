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

    [CreateAssetMenu(
        fileName = "WeaponData",
        menuName = "Eclipse/Weapons/Weapon Data")]
    public sealed class WeaponData : ScriptableObject
    {
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
        private int pierce = 0;

        [SerializeField, Min(0f)]
        private float knockback = 0f;

        [Header("Progression")]
        [SerializeField, Min(1)]
        private int maxLevel = 8;

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

        public int ProjectileCount =>
            projectileCount;

        public float ProjectileSpeed =>
            projectileSpeed;

        public float ProjectileSize =>
            projectileSize;

        public int Pierce => pierce;

        public float Knockback => knockback;

        public int MaxLevel => maxLevel;

        public WeaponType WeaponType =>
            weaponType;
    }
}