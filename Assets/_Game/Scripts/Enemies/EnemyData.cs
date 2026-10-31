using UnityEngine;

namespace Eclipse.Enemies
{
    [CreateAssetMenu(
        fileName = "EnemyData",
        menuName = "Eclipse/Enemies/Enemy Data")]
    public sealed class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string id = "basic_enemy";

        [SerializeField]
        private string displayName = "Basic Enemy";

        [Header("Core Stats")]
        [SerializeField, Min(1f)]
        private float maxHP = 30f;

        [SerializeField, Min(0f)]
        private float moveSpeed = 2.5f;

        [SerializeField, Min(0f)]
        private float damage = 10f;

        [SerializeField, Min(0f)]
        private float armor = 0f;

        [Header("Rewards")]
        [SerializeField, Min(0)]
        private int expReward = 5;

        [SerializeField, Min(0)]
        private int goldReward = 1;

        [Header("Resistance")]
        [SerializeField, Range(0f, 1f)]
        private float knockbackResistance = 0f;

        public string Id => id;

        public string DisplayName => displayName;

        public float MaxHP => maxHP;

        public float MoveSpeed => moveSpeed;

        public float Damage => damage;

        public float Armor => armor;

        public int ExpReward => expReward;

        public int GoldReward => goldReward;

        public float KnockbackResistance =>
            knockbackResistance;
    }
}