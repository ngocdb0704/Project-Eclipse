
using System;
using Eclipse.Combat;
using Eclipse.Core.Pooling;
using UnityEngine;

namespace Eclipse.UI
{
    public sealed class DamageNumberSpawner :
        MonoBehaviour
    {
        [SerializeField]
        private DamageNumberPopup popupPrefab;

        [SerializeField]
        private Vector3 worldOffset =
            new Vector3(0f, 0.5f, 0f);

        [SerializeField, Min(0)]
        private int prewarmCount = 32;

        private GameObjectPool<DamageNumberPopup> popupPool;

        private Action<DamageNumberPopup> releaseCallback;

        private Transform poolRoot;

        private void Awake()
        {
            if (popupPrefab == null)
            {
                Debug.LogError(
                    "DamageNumberSpawner requires a popup prefab.",
                    this);

                enabled = false;
                return;
            }

            GameObject rootObject =
                new GameObject("Damage Number Pool");

            poolRoot = rootObject.transform;
            poolRoot.SetParent(transform, false);

            popupPool =
                new GameObjectPool<DamageNumberPopup>(
                    popupPrefab,
                    poolRoot);

            releaseCallback = ReleasePopup;

            popupPool.Prewarm(
                Mathf.Max(0, prewarmCount));
        }

        private void OnEnable()
        {
            if (popupPool != null)
            {
                DamageEvents.DamageDealt +=
                    HandleDamageDealt;
            }
        }

        private void OnDisable()
        {
            DamageEvents.DamageDealt -=
                HandleDamageDealt;
        }

        private void OnDestroy()
        {
            if (poolRoot != null)
            {
                Destroy(poolRoot.gameObject);
                poolRoot = null;
            }
        }

        private void OnValidate()
        {
            prewarmCount =
                Mathf.Max(0, prewarmCount);
        }

        private void HandleDamageDealt(
            DamageEventData data)
        {
            if (popupPool == null)
            {
                return;
            }

            DamageNumberPopup popup =
                popupPool.Get(
                    data.WorldPosition + worldOffset,
                    Quaternion.identity);

            popup.Show(
                data.Amount,
                data.IsCritical,
                releaseCallback);
        }

        private void ReleasePopup(
            DamageNumberPopup popup)
        {
            if (!popupPool.Release(popup))
            {
                Debug.LogWarning(
                    "Damage popup pool rejected release.",
                    popup);
            }
        }
    }
}
