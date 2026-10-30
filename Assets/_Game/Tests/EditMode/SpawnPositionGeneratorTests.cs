using System;
using Eclipse.World;
using NUnit.Framework;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class SpawnPositionGeneratorTests
    {
        [Test]
        public void Generate_OneHundredSamples_AreOutsideCameraBoundsWithMargin()
        {
            Vector2 center =
                new Vector2(3f, -2f);

            Vector2 halfExtents =
                new Vector2(8f, 4.5f);

            const float margin = 2f;

            float minX =
                center.x - halfExtents.x - margin;

            float maxX =
                center.x + halfExtents.x + margin;

            float minY =
                center.y - halfExtents.y - margin;

            float maxY =
                center.y + halfExtents.y + margin;

            var random =
                new System.Random(12345);

            for (int i = 0; i < 100; i++)
            {
                Vector2 point =
                    SpawnPositionGenerator.Generate(
                        center,
                        halfExtents,
                        margin,
                        random);

                bool onOrOutsideExpandedBoundary =
                    point.x <= minX ||
                    point.x >= maxX ||
                    point.y <= minY ||
                    point.y >= maxY;

                bool insideExpandedRectangleRange =
                    point.x >= minX &&
                    point.x <= maxX &&
                    point.y >= minY &&
                    point.y <= maxY;

                Assert.IsTrue(
                    onOrOutsideExpandedBoundary,
                    $"Sample {i} was inside the expanded camera bounds: {point}");

                Assert.IsTrue(
                    insideExpandedRectangleRange,
                    $"Sample {i} was outside the spawn perimeter range: {point}");
            }
        }

        [Test]
        public void Generate_OneHundredSamples_AreFinite()
        {
            var random =
                new System.Random(67890);

            for (int i = 0; i < 100; i++)
            {
                Vector2 point =
                    SpawnPositionGenerator.Generate(
                        Vector2.zero,
                        new Vector2(10f, 6f),
                        2f,
                        random);

                Assert.IsFalse(float.IsNaN(point.x));
                Assert.IsFalse(float.IsNaN(point.y));
                Assert.IsFalse(float.IsInfinity(point.x));
                Assert.IsFalse(float.IsInfinity(point.y));
            }
        }

        [Test]
        public void Generate_SameSeed_ProducesSameSequence()
        {
            var firstRandom =
                new System.Random(42);

            var secondRandom =
                new System.Random(42);

            for (int i = 0; i < 100; i++)
            {
                Vector2 first =
                    SpawnPositionGenerator.Generate(
                        Vector2.zero,
                        new Vector2(8f, 4.5f),
                        2f,
                        firstRandom);

                Vector2 second =
                    SpawnPositionGenerator.Generate(
                        Vector2.zero,
                        new Vector2(8f, 4.5f),
                        2f,
                        secondRandom);

                Assert.AreEqual(first, second);
            }
        }

        [Test]
        public void Generate_InvalidGeometry_Throws()
        {
            var random =
                new System.Random(1);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => SpawnPositionGenerator.Generate(
                    Vector2.zero,
                    Vector2.zero,
                    2f,
                    random));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => SpawnPositionGenerator.Generate(
                    Vector2.zero,
                    Vector2.one,
                    -1f,
                    random));
        }
    }
}