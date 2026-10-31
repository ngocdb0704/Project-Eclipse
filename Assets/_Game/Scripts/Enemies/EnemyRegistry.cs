using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Eclipse.Enemies
{
    public sealed class EnemyRegistry : MonoBehaviour
    {
        private readonly List<EnemyController> activeEnemies =
            new List<EnemyController>();

        private ReadOnlyCollection<EnemyController>
            readOnlyActiveEnemies;

        public int Count => activeEnemies.Count;

        public IReadOnlyList<EnemyController> ActiveEnemies
        {
            get
            {
                if (readOnlyActiveEnemies == null)
                {
                    readOnlyActiveEnemies =
                        activeEnemies.AsReadOnly();
                }

                return readOnlyActiveEnemies;
            }
        }

        public bool Register(EnemyController enemy)
        {
            if (enemy == null ||
                enemy.IsDead ||
                !enemy.isActiveAndEnabled ||
                activeEnemies.Contains(enemy))
            {
                return false;
            }

            activeEnemies.Add(enemy);
            enemy.Died += HandleEnemyDied;

            return true;
        }

        public bool Unregister(EnemyController enemy)
        {
            if (enemy == null ||
                !activeEnemies.Remove(enemy))
            {
                return false;
            }

            enemy.Died -= HandleEnemyDied;

            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                EnemyController enemy = activeEnemies[i];

                if (enemy != null)
                {
                    enemy.Died -= HandleEnemyDied;
                }
            }

            activeEnemies.Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void HandleEnemyDied(
            EnemyController enemy)
        {
            Unregister(enemy);
        }
    }
}