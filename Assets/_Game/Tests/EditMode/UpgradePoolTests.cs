
using System;
using System.Collections.Generic;
using System.Reflection;
using Eclipse.Progression;
using NUnit.Framework;
using UnityEngine;

namespace Eclipse.Tests.EditMode
{
    public sealed class UpgradePoolTests
    {
        private List<UpgradeData> createdAssets;

        [SetUp]
        public void SetUp()
        {
            createdAssets = new List<UpgradeData>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (UpgradeData asset in createdAssets)
            {
                if (asset != null)
                    UnityEngine.Object.DestroyImmediate(asset);
            }

            createdAssets.Clear();
        }

        [Test]
        public void ReturnsAtMostThreeUniqueOptions()
        {
            var upgrades = new[]
            {
                CreateUpgrade("a"),
                CreateUpgrade("b"),
                CreateUpgrade("c"),
                CreateUpgrade("d"),
                CreateUpgrade("e")
            };

            var pool = new UpgradePool(
                upgrades, new System.Random(10));

            var options = pool.GetOptions(
                1, new Dictionary<string, int>());

            Assert.AreEqual(3, options.Count);

            var ids = new HashSet<string>();

            foreach (UpgradeData option in options)
                Assert.IsTrue(ids.Add(option.Id));
        }

        [Test]
        public void MaxedUpgradeIsExcluded()
        {
            UpgradeData maxed = CreateUpgrade(
                "maxed", maxLevel: 2);

            UpgradeData available = CreateUpgrade(
                "available", maxLevel: 5);

            var owned = new Dictionary<string, int>
            {
                { "maxed", 2 },
                { "available", 2 }
            };

            var pool = new UpgradePool(
                new[] { maxed, available },
                new System.Random(11));

            var options = pool.GetOptions(1, owned);

            Assert.AreEqual(1, options.Count);
            Assert.AreSame(available, options[0]);
            Assert.AreEqual(2, owned["available"]);
        }

        [Test]
        public void RequiredPlayerLevelIsEnforced()
        {
            UpgradeData early = CreateUpgrade("early");

            UpgradeData late = CreateUpgrade(
                "late", requiredPlayerLevel: 10);

            var pool = new UpgradePool(
                new[] { early, late },
                new System.Random(12));

            var owned = new Dictionary<string, int>();

            var atLevelOne = pool.GetOptions(1, owned);

            Assert.AreEqual(1, atLevelOne.Count);
            Assert.AreSame(early, atLevelOne[0]);

            var atLevelTen = pool.GetOptions(10, owned);

            Assert.AreEqual(2, atLevelTen.Count);
            CollectionAssert.AreEquivalent(
                new[] { early, late },
                atLevelTen);
        }

        [Test]
        public void AllPrerequisitesMustBeOwned()
        {
            UpgradeData first = CreateUpgrade("first");
            UpgradeData second = CreateUpgrade("second");

            UpgradeData gated = CreateUpgrade(
                "gated",
                prerequisites: new[] { first, second });

            var pool = new UpgradePool(
                new[] { first, second, gated },
                new System.Random(13));

            var owned = new Dictionary<string, int>
            {
                { "first", 1 }
            };

            var before = pool.GetOptions(5, owned);

            CollectionAssert.DoesNotContain(before, gated);

            owned["second"] = 1;

            var after = pool.GetOptions(5, owned);

            CollectionAssert.Contains(after, gated);
        }

        [Test]
        public void DuplicateAssetAndDuplicateIdAreExcluded()
        {
            UpgradeData first = CreateUpgrade("same");
            UpgradeData duplicateId = CreateUpgrade("same");
            UpgradeData second = CreateUpgrade("other");

            var pool = new UpgradePool(
                new[]
                {
                    first,
                    first,
                    duplicateId,
                    second
                },
                new System.Random(14));

            var options = pool.GetOptions(
                1, new Dictionary<string, int>());

            Assert.AreEqual(2, options.Count);

            CollectionAssert.Contains(options, first);
            CollectionAssert.Contains(options, second);
            CollectionAssert.DoesNotContain(
                options, duplicateId);
        }

        [Test]
        public void EmptyPoolReturnsNoOptions()
        {
            var pool = new UpgradePool(
                new UpgradeData[0],
                new System.Random(15));

            var options = pool.GetOptions(
                1, new Dictionary<string, int>());

            Assert.AreEqual(0, options.Count);
        }

        [Test]
        public void ZeroRequestedOptionsReturnsEmptyResult()
        {
            UpgradeData upgrade = CreateUpgrade("a");

            var pool = new UpgradePool(
                new[] { upgrade },
                new System.Random(16));

            var options = pool.GetOptions(
                1,
                new Dictionary<string, int>(),
                optionCount: 0);

            Assert.AreEqual(0, options.Count);
        }

        [Test]
        public void OptionCountCannotExceedThree()
        {
            var upgrades = new[]
            {
                CreateUpgrade("a"),
                CreateUpgrade("b"),
                CreateUpgrade("c"),
                CreateUpgrade("d")
            };

            var pool = new UpgradePool(
                upgrades, new System.Random(17));

            var options = pool.GetOptions(
                1,
                new Dictionary<string, int>(),
                optionCount: 10);

            Assert.AreEqual(3, options.Count);
        }

        [Test]
        public void SameSeedProducesSameSelections()
        {
            var upgrades = new[]
            {
                CreateUpgrade("common",
                    UpgradeRarity.Common),
                CreateUpgrade("uncommon",
                    UpgradeRarity.Uncommon),
                CreateUpgrade("rare",
                    UpgradeRarity.Rare),
                CreateUpgrade("epic",
                    UpgradeRarity.Epic)
            };

            var firstPool = new UpgradePool(
                upgrades, new System.Random(12345));

            var secondPool = new UpgradePool(
                upgrades, new System.Random(12345));

            var owned = new Dictionary<string, int>();

            for (int round = 0; round < 10; round++)
            {
                var first = firstPool.GetOptions(1, owned);
                var second = secondPool.GetOptions(1, owned);

                CollectionAssert.AreEqual(first, second);
            }
        }

        [Test]
        public void RarityWeightsFavorCommonOverEpic()
        {
            Assert.AreEqual(
                100,
                UpgradePool.GetRarityWeight(
                    UpgradeRarity.Common));

            Assert.AreEqual(
                5,
                UpgradePool.GetRarityWeight(
                    UpgradeRarity.Epic));

            UpgradeData common = CreateUpgrade(
                "common", UpgradeRarity.Common);

            UpgradeData epic = CreateUpgrade(
                "epic", UpgradeRarity.Epic);

            var pool = new UpgradePool(
                new[] { common, epic },
                new System.Random(2026));

            var owned = new Dictionary<string, int>();

            int commonCount = 0;
            int epicCount = 0;

            for (int i = 0; i < 2000; i++)
            {
                var options = pool.GetOptions(
                    1, owned, optionCount: 1);

                if (options[0] == common)
                    commonCount++;
                else if (options[0] == epic)
                    epicCount++;
            }

            Assert.AreEqual(
                2000, commonCount + epicCount);

            Assert.Greater(
                commonCount,
                epicCount * 8);
        }

        [Test]
        public void InvalidArgumentsAreRejected()
        {
            Assert.Throws<ArgumentNullException>(
                () => new UpgradePool(null));

            var pool = new UpgradePool(
                new UpgradeData[0],
                new System.Random(18));

            var owned = new Dictionary<string, int>();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => pool.GetOptions(0, owned));

            Assert.Throws<ArgumentNullException>(
                () => pool.GetOptions(1, null));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => pool.GetOptions(
                    1, owned, optionCount: -1));
        }

        private UpgradeData CreateUpgrade(
            string id,
            UpgradeRarity rarity = UpgradeRarity.Common,
            int maxLevel = 5,
            int requiredPlayerLevel = 1,
            UpgradeData[] prerequisites = null)
        {
            UpgradeData asset =
                ScriptableObject.CreateInstance<UpgradeData>();

            createdAssets.Add(asset);

            SetField(asset, "id", id);
            SetField(asset, "rarity", rarity);
            SetField(asset, "maxLevel", maxLevel);
            SetField(
                asset,
                "requiredPlayerLevel",
                requiredPlayerLevel);

            SetField(
                asset,
                "prerequisites",
                prerequisites ?? new UpgradeData[0]);

            return asset;
        }

        private static void SetField(
            UpgradeData asset,
            string fieldName,
            object value)
        {
            FieldInfo field =
                typeof(UpgradeData).GetField(
                    fieldName,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.IsNotNull(
                field,
                $"Missing field: {fieldName}");

            field.SetValue(asset, value);
        }
    }
}
