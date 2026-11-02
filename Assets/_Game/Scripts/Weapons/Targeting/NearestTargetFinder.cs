using Eclipse.Enemies;
using UnityEngine;

namespace Eclipse.Weapons.Targeting
{
    public static class NearestTargetFinder
    {
        public static bool TryFindNearest(
            EnemyRegistry registry,
            Vector2 origin,
            out EnemyController target)
        {
            target = null;

            if (registry == null)
            {
                return false;
            }

            var enemies =
                registry.ActiveEnemies;

            float nearestSqrDistance =
                float.PositiveInfinity;

            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyController enemy =
                    enemies[i];

                if (enemy == null ||
                    enemy.IsDead ||
                    !enemy.isActiveAndEnabled)
                {
                    continue;
                }

                Vector2 offset =
                    (Vector2)enemy.transform.position -
                    origin;

                float sqrDistance =
                    offset.sqrMagnitude;

                if (sqrDistance >=
                    nearestSqrDistance)
                {
                    continue;
                }

                nearestSqrDistance =
                    sqrDistance;

                target = enemy;
            }

            return target != null;
        }
    }
}