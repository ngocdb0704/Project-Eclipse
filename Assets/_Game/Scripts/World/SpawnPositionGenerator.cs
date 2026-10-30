using System;
using UnityEngine;

namespace Eclipse.World
{
    public static class SpawnPositionGenerator
    {
        public static Vector2 Generate(
            Vector2 center,
            Vector2 halfExtents,
            float margin,
            System.Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            ValidateFinite(center, nameof(center));

            if (!IsFinite(halfExtents.x) ||
                !IsFinite(halfExtents.y) ||
                halfExtents.x <= 0f ||
                halfExtents.y <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(halfExtents),
                    "Half extents must be finite and greater than zero.");
            }

            if (!IsFinite(margin) || margin < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(margin),
                    "Margin must be finite and non-negative.");
            }

            float expandedHalfWidth =
                halfExtents.x + margin;

            float expandedHalfHeight =
                halfExtents.y + margin;

            if (!IsFinite(expandedHalfWidth) ||
                !IsFinite(expandedHalfHeight))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(margin),
                    "Expanded bounds must remain finite.");
            }

            float minX = center.x - expandedHalfWidth;
            float maxX = center.x + expandedHalfWidth;
            float minY = center.y - expandedHalfHeight;
            float maxY = center.y + expandedHalfHeight;

            float width = expandedHalfWidth * 2f;
            float height = expandedHalfHeight * 2f;
            float perimeter = 2f * (width + height);

            if (!IsFinite(minX) ||
                !IsFinite(maxX) ||
                !IsFinite(minY) ||
                !IsFinite(maxY) ||
                !IsFinite(perimeter))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(halfExtents),
                    "Spawn rectangle must remain finite.");
            }

            float distance =
                (float)random.NextDouble() * perimeter;

            if (distance < width)
            {
                return new Vector2(
                    minX + distance,
                    minY);
            }

            distance -= width;

            if (distance < height)
            {
                return new Vector2(
                    maxX,
                    minY + distance);
            }

            distance -= height;

            if (distance < width)
            {
                return new Vector2(
                    maxX - distance,
                    maxY);
            }

            distance -= width;

            return new Vector2(
                minX,
                maxY - distance);
        }

        private static void ValidateFinite(
            Vector2 value,
            string parameterName)
        {
            if (!IsFinite(value.x) ||
                !IsFinite(value.y))
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    "Vector components must be finite.");
            }
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) &&
                   !float.IsInfinity(value);
        }
    }
}