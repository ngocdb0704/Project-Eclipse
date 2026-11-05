
using System;
using System.Collections;
using System.Collections.Generic;
using Eclipse.Player;
using Eclipse.Progression;
using UnityEngine;

namespace Eclipse.UI
{
    public sealed class UpgradeSelectionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private LevelUpPauseController pauseController;

        [SerializeField]
        private PlayerController player;

        [Header("Available Upgrades")]
        [SerializeField]
        private UpgradeData[] availableUpgrades;

        [Header("UI Cards")]
        [SerializeField]
        private UpgradeOptionCard[] optionCards;

        [Header("Random")]
        [SerializeField]
        private int randomSeed = 12345;

        private UpgradePool upgradePool;

        private readonly Dictionary<string, int> ownedLevels =
            new Dictionary<string, int>(
                StringComparer.Ordinal);

        private readonly List<UpgradeOptionCard> validCards =
            new List<UpgradeOptionCard>(3);

        private readonly List<UpgradeData> displayedOptions =
            new List<UpgradeData>(3);

        private Coroutine autoAdvanceCoroutine;
        private int displayedPlayerLevel;
        private bool selectionCommitted;

        private void Awake()
        {
            upgradePool = new UpgradePool(
                availableUpgrades ?? new UpgradeData[0],
                new System.Random(randomSeed));

            if (optionCards != null)
            {
                foreach (UpgradeOptionCard card in optionCards)
                {
                    if (card == null)
                        continue;

                    card.Hide();

                    if (validCards.Count < 3 &&
                        !validCards.Contains(card))
                    {
                        validCards.Add(card);
                    }
                }
            }

            if (validCards.Count == 0)
            {
                Debug.LogError(
                    "UpgradeSelectionController requires UI cards.",
                    this);
            }
        }

        private void OnEnable()
        {
            if (pauseController == null || player == null)
            {
                Debug.LogError(
                    "UpgradeSelectionController is missing references.",
                    this);

                enabled = false;
                return;
            }

            pauseController.UpgradeRequested +=
                HandleUpgradeRequested;

            // Recover if this UI was enabled during
            // an already-active level-up selection.
            if (pauseController.CurrentUpgradeLevel > 0)
            {
                HandleUpgradeRequested(
                    pauseController.CurrentUpgradeLevel);
            }
        }

        private void OnDisable()
        {
            if (pauseController != null)
            {
                pauseController.UpgradeRequested -=
                    HandleUpgradeRequested;
            }

            if (autoAdvanceCoroutine != null)
            {
                StopCoroutine(autoAdvanceCoroutine);
                autoAdvanceCoroutine = null;
            }

            HideAllCards();
            displayedOptions.Clear();
            displayedPlayerLevel = 0;
        }

        private void HandleUpgradeRequested(int newLevel)
        {
            displayedPlayerLevel = newLevel;
            selectionCommitted = false;

            displayedOptions.Clear();
            HideAllCards();

            IReadOnlyList<UpgradeData> options =
                upgradePool.GetOptions(
                    newLevel,
                    ownedLevels,
                    validCards.Count);

            for (int i = 0; i < options.Count; i++)
            {
                UpgradeData upgrade = options[i];

                displayedOptions.Add(upgrade);

                int ownedLevel = GetOwnedLevel(upgrade.Id);

                validCards[i].Show(
                    upgrade,
                    ownedLevel + 1,
                    SelectUpgrade);
            }

            if (displayedOptions.Count == 0)
            {
                Debug.Log(
                    $"No upgrade available at player level {newLevel}.",
                    this);

                // Avoid recursively completing thousands of
                // queued level-ups in the same call stack.
                if (autoAdvanceCoroutine == null)
                {
                    autoAdvanceCoroutine =
                        StartCoroutine(AutoAdvanceEmpty());
                }

                return;
            }

            if (autoAdvanceCoroutine != null)
            {
                StopCoroutine(autoAdvanceCoroutine);
                autoAdvanceCoroutine = null;
            }

            validCards[0].Focus();
        }

        private IEnumerator AutoAdvanceEmpty()
        {
            while (true)
            {
                // yield null continues even while
                // Time.timeScale is zero.
                yield return null;

                if (pauseController == null ||
                    pauseController.CurrentUpgradeLevel == 0 ||
                    displayedOptions.Count > 0)
                {
                    break;
                }

                pauseController.CompleteUpgradeSelection();
            }

            autoAdvanceCoroutine = null;
        }

        private void SelectUpgrade(UpgradeData upgrade)
        {
            if (selectionCommitted ||
                upgrade == null ||
                displayedPlayerLevel <= 0 ||
                pauseController.CurrentUpgradeLevel !=
                    displayedPlayerLevel ||
                !displayedOptions.Contains(upgrade))
            {
                return;
            }

            if (!TryApplyUpgrade(upgrade))
                return;

            selectionCommitted = true;

            // The pause controller either opens the next
            // queued level-up or resumes gameplay.
            pauseController.CompleteUpgradeSelection();
        }

        private bool TryApplyUpgrade(UpgradeData upgrade)
        {
            PlayerStats stats = player.Stats;

            if (stats == null)
            {
                Debug.LogError(
                    "Player runtime stats are unavailable.",
                    this);

                return false;
            }

            int currentLevel = GetOwnedLevel(upgrade.Id);

            if (currentLevel >= upgrade.MaxLevel)
                return false;

            try
            {
                switch (upgrade.EffectType)
                {
                    case UpgradeEffectType.DamageMultiplier:
                        stats.AddDamageMultiplier(
                            upgrade.ValuePerLevel);
                        break;

                    case UpgradeEffectType.CooldownReduction:
                        stats.AddCooldownReduction(
                            upgrade.ValuePerLevel);
                        break;

                    default:
                        Debug.LogError(
                            $"Unsupported upgrade effect: {upgrade.EffectType}",
                            this);

                        return false;
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                return false;
            }

            ownedLevels[upgrade.Id] = currentLevel + 1;

            Debug.Log(
                $"Applied {upgrade.Id} Level {currentLevel + 1}. " +
                $"Damage={stats.DamageMultiplier:F2}, " +
                $"CooldownReduction={stats.CooldownReduction:F2}",
                this);

            return true;
        }

        private int GetOwnedLevel(string id)
        {
            return ownedLevels.TryGetValue(
                id, out int level)
                    ? level
                    : 0;
        }

        private void HideAllCards()
        {
            foreach (UpgradeOptionCard card in validCards)
            {
                if (card != null)
                    card.Hide();
            }
        }
    }
}
