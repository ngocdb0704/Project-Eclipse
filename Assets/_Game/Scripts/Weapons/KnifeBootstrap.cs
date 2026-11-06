
using Eclipse.Player;
using UnityEngine;

namespace Eclipse.Weapons
{
    public sealed class KnifeBootstrap : MonoBehaviour
    {
        [SerializeField]
        private WeaponData knifeData;

        [SerializeField]
        private PlayerController player;

        private void Start()
        {
            if (knifeData == null ||
                knifeData.Prefab == null)
            {
                Debug.LogError(
                    "KnifeBootstrap requires valid KnifeData.",
                    this);
                return;
            }

            if (player == null || player.Stats == null)
            {
                Debug.LogError(
                    "KnifeBootstrap requires an initialized Player.",
                    this);
                return;
            }

            GameObject weaponObject =
                Instantiate(knifeData.Prefab);

            if (!weaponObject.TryGetComponent(
                    out KnifeWeapon weapon))
            {
                Debug.LogError(
                    "Knife prefab requires KnifeWeapon.",
                    weaponObject);

                Destroy(weaponObject);
                return;
            }

            try
            {
                weapon.InitializeForPlayer(
                    knifeData,
                    player);
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception, this);
                Destroy(weaponObject);
            }
        }
    }
}
