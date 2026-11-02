using Eclipse.Combat;
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

        private void Awake()
        {
            if (popupPrefab == null)
            {
                Debug.LogError(
                    "DamageNumberSpawner requires a popup prefab.",
                    this);

                enabled = false;
            }
        }

        private void OnEnable()
        {
            DamageEvents.DamageDealt +=
                HandleDamageDealt;
        }

        private void OnDisable()
        {
            DamageEvents.DamageDealt -=
                HandleDamageDealt;
        }

        private void HandleDamageDealt(
            DamageEventData data)
        {
            DamageNumberPopup popup =
                Instantiate(
                    popupPrefab,
                    data.WorldPosition +
                    worldOffset,
                    Quaternion.identity);

            popup.Show(
                data.Amount,
                data.IsCritical);
        }
    }
}