using Eclipse.Enemies;
using Eclipse.Player;
using UnityEngine;

namespace Eclipse.Weapons
{
    public sealed class MagicBoltBootstrap :
        MonoBehaviour
    {
        [SerializeField]
        private WeaponData magicBoltData;

        [SerializeField]
        private PlayerController player;

        [SerializeField]
        private EnemyRegistry enemyRegistry;

        private void Start()
        {
            if (magicBoltData == null)
            {
                Debug.LogError(
                    "MagicBoltBootstrap requires WeaponData.",
                    this);

                return;
            }

            if (magicBoltData.Prefab == null)
            {
                Debug.LogError(
                    "Magic Bolt WeaponData requires a prefab.",
                    this);

                return;
            }

            if (player == null)
            {
                Debug.LogError(
                    "MagicBoltBootstrap requires PlayerController.",
                    this);

                return;
            }

            if (enemyRegistry == null)
            {
                Debug.LogError(
                    "MagicBoltBootstrap requires EnemyRegistry.",
                    this);

                return;
            }

            GameObject weaponObject =
                Instantiate(
                    magicBoltData.Prefab);

            MagicBoltWeapon weapon =
                weaponObject.GetComponent<
                    MagicBoltWeapon>();

            if (weapon == null)
            {
                Debug.LogError(
                    "Magic Bolt prefab requires MagicBoltWeapon.",
                    weaponObject);

                Destroy(weaponObject);
                return;
            }

            weapon.InitializeForPlayer(
                magicBoltData,
                player,
                enemyRegistry);
        }
    }
}