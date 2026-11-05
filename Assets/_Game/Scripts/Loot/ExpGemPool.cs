
using System;
using Eclipse.Core.Pooling;
using UnityEngine;

namespace Eclipse.Loot
{
    public sealed class ExpGemPool : MonoBehaviour
    {
        [SerializeField]
        private ExpGem gemPrefab;

        [SerializeField, Min(0)]
        private int prewarmCount = 32;

        private GameObjectPool<ExpGem> pool;
        private Transform poolRoot;

        public bool IsReady => pool != null;

        public int CountActive =>
            pool != null ? pool.CountActive : 0;

        public int CountInactive =>
            pool != null ? pool.CountInactive : 0;

        public int CountAll =>
            pool != null ? pool.CountAll : 0;

        public event Action<int> ExperienceCollected;

        private void Awake()
        {
            if (gemPrefab == null)
            {
                Debug.LogError(
                    "ExpGemPool requires an ExpGem prefab.",
                    this);

                enabled = false;
                return;
            }

            GameObject root =
                new GameObject("EXP Gem Pool");

            poolRoot = root.transform;
            poolRoot.SetParent(transform, false);

            pool = new GameObjectPool<ExpGem>(
                gemPrefab,
                poolRoot);

            pool.Prewarm(
                Mathf.Max(0, prewarmCount));
        }

        public ExpGem Spawn(
            Vector3 worldPosition,
            int expValue)
        {
            if (pool == null)
            {
                throw new InvalidOperationException(
                    "EXP gem pool is not initialized.");
            }

            // Validate before acquiring an instance.
            // This prevents leaking an active object
            // when the supplied EXP value is invalid.
            if (expValue <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(expValue));
            }

            ExpGem gem = pool.Get(
    worldPosition,
    Quaternion.identity);

            gem.BindPool(this);
            gem.Initialize(expValue);

            return gem;
        }

        public bool Release(ExpGem gem)
        {
            return pool != null &&
                   pool.Release(gem);
        }

        private void OnValidate()
        {
            prewarmCount =
                Mathf.Max(0, prewarmCount);
        }

        internal void Collect(ExpGem gem, int expValue)
        {
            if (gem == null || expValue <= 0 || pool == null)
                return;

            try
            {
                ExperienceCollected?.Invoke(expValue);
            }
            finally
            {
                if (!Release(gem))
                {
                    Debug.LogError(
                        "Failed to release collected EXP gem.",
                        this);
                }
            }
        }
    }
}
