
using System;
using System.Collections.Generic;

namespace Eclipse.Progression
{
    /// <summary>
    /// Selects eligible upgrades without modifying their assets.
    /// A seeded Random can be injected for deterministic tests.
    /// </summary>
    public sealed class UpgradePool
    {
        private const int MaximumOptions = 3;

        private readonly List<UpgradeData> upgrades;
        private readonly System.Random random;

        public UpgradePool(
            IEnumerable<UpgradeData> upgrades,
            System.Random random = null)
        {
            if (upgrades == null)
                throw new ArgumentNullException(nameof(upgrades));

            this.upgrades = new List<UpgradeData>(upgrades);
            this.random = random ?? new System.Random();
        }

        /// <summary>
        /// Owned levels use upgrade IDs as keys.
        /// A missing ID means the upgrade is not owned.
        /// Owned upgrades remain eligible until MaxLevel.
        /// </summary>
        public IReadOnlyList<UpgradeData> GetOptions(
            int playerLevel,
            IReadOnlyDictionary<string, int> ownedLevels,
            int optionCount = MaximumOptions)
        {
            if (playerLevel < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(playerLevel));

            if (ownedLevels == null)
                throw new ArgumentNullException(
                    nameof(ownedLevels));

            if (optionCount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(optionCount));

            var eligible = new List<UpgradeData>();
            var seenIds = new HashSet<string>(
                StringComparer.Ordinal);

            foreach (UpgradeData upgrade in upgrades)
            {
                if (upgrade == null ||
                    string.IsNullOrWhiteSpace(upgrade.Id))
                    continue;

                if (seenIds.Contains(upgrade.Id))
                    continue;

                if (playerLevel < upgrade.RequiredPlayerLevel)
                    continue;

                int ownedLevel = GetOwnedLevel(
                    ownedLevels, upgrade.Id);

                if (upgrade.MaxLevel < 1 ||
                    ownedLevel >= upgrade.MaxLevel)
                    continue;

                if (!HasPrerequisites(upgrade, ownedLevels))
                    continue;

                seenIds.Add(upgrade.Id);
                eligible.Add(upgrade);
            }

            int count = Math.Min(
                Math.Min(optionCount, MaximumOptions),
                eligible.Count);

            var results = new List<UpgradeData>(count);

            // Weighted sampling without replacement.
            while (results.Count < count)
            {
                long totalWeight = 0;

                foreach (UpgradeData upgrade in eligible)
                {
                    totalWeight += GetRarityWeight(
                        upgrade.Rarity);
                }

                double roll =
                    random.NextDouble() * totalWeight;

                int selectedIndex = eligible.Count - 1;

                for (int i = 0; i < eligible.Count; i++)
                {
                    roll -= GetRarityWeight(
                        eligible[i].Rarity);

                    if (roll < 0d)
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                results.Add(eligible[selectedIndex]);
                eligible.RemoveAt(selectedIndex);
            }

            return results;
        }

        public static int GetRarityWeight(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Common:
                    return 100;

                case UpgradeRarity.Uncommon:
                    return 40;

                case UpgradeRarity.Rare:
                    return 15;

                case UpgradeRarity.Epic:
                    return 5;

                default:
                    return 1;
            }
        }

        private static int GetOwnedLevel(
            IReadOnlyDictionary<string, int> ownedLevels,
            string upgradeId)
        {
            return ownedLevels.TryGetValue(
                upgradeId, out int level)
                    ? Math.Max(0, level)
                    : 0;
        }

        private static bool HasPrerequisites(
            UpgradeData upgrade,
            IReadOnlyDictionary<string, int> ownedLevels)
        {
            UpgradeData[] prerequisites =
                upgrade.Prerequisites;

            if (prerequisites == null)
                return true;

            foreach (UpgradeData prerequisite in prerequisites)
            {
                if (prerequisite == null ||
                    string.IsNullOrWhiteSpace(prerequisite.Id))
                    return false;

                if (GetOwnedLevel(
                        ownedLevels,
                        prerequisite.Id) < 1)
                    return false;
            }

            return true;
        }
    }
}
