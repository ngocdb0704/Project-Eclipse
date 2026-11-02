using UnityEngine;

namespace Eclipse.Combat
{
    public readonly struct DamageEventData
    {
        public Vector3 WorldPosition { get; }

        public float Amount { get; }

        public bool IsCritical { get; }

        public DamageEventData(
            Vector3 worldPosition,
            float amount,
            bool isCritical)
        {
            WorldPosition = worldPosition;
            Amount = Mathf.Max(0f, amount);
            IsCritical = isCritical;
        }
    }
}