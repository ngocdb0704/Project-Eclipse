
using UnityEngine;

namespace Eclipse.Progression
{
    public enum UpgradeRarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3
    }


    public enum UpgradeEffectType
    {
        DamageMultiplier = 0,
        CooldownReduction = 1,
        ProjectileCount = 2
    }


    [CreateAssetMenu(
        fileName = "NewUpgrade",
        menuName = "Eclipse/Progression/Upgrade Data")]
    public sealed class UpgradeData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string id = "new_upgrade";

        [SerializeField]
        private string displayName = "New Upgrade";

        [SerializeField, TextArea(2, 5)]
        private string description;

        [SerializeField]
        private Sprite icon;

        [Header("Rarity")]
        [SerializeField]
        private UpgradeRarity rarity = UpgradeRarity.Common;

        [Header("Progression")]
        [SerializeField, Min(1)]
        private int maxLevel = 5;

        [SerializeField, Min(1)]
        private int requiredPlayerLevel = 1;

        [Tooltip("All listed upgrades must be owned to unlock this upgrade.")]
        [SerializeField]
        private UpgradeData[] prerequisites =
            new UpgradeData[0];

        [Header("Effect")]
        [SerializeField]
        private UpgradeEffectType effectType =
            UpgradeEffectType.DamageMultiplier;

        [Tooltip("Additive modifier gained per upgrade level.")]
        [SerializeField, Min(0f)]
        private float valuePerLevel = 0.1f;


        [Header("Projectile Count Effect")]
        [SerializeField, Min(1)]
        private int projectilesPerLevel = 1;


        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;

        public UpgradeRarity Rarity => rarity;

        public int MaxLevel => maxLevel;
        public int RequiredPlayerLevel => requiredPlayerLevel;
        public UpgradeData[] Prerequisites => prerequisites;

        public UpgradeEffectType EffectType => effectType;
        public float ValuePerLevel => valuePerLevel;


        public int ProjectilesPerLevel =>
            Mathf.Max(1, projectilesPerLevel);

    }
}
