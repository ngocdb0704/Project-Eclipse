
using System;
using System.Collections.Generic;
using Eclipse.Core.StateMachine;
using Eclipse.Loot;
using Eclipse.Weapons;
using UnityEngine;

namespace Eclipse.Progression
{
    public sealed class LevelUpPauseController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ExpGemPool gemPool;
        [SerializeField] private ExperienceComponent experience;
        [SerializeField] private GameplayStateMachine gameState;

        [Header("UI")]
        [SerializeField] private GameObject levelUpPanel;

        private readonly Queue<int> pendingLevels = new Queue<int>();
        private readonly List<WeaponBase> pausedWeapons =
            new List<WeaponBase>();

        private bool subscribed;

        public int CurrentUpgradeLevel { get; private set; }

        public int PendingUpgradeCount =>
            pendingLevels.Count + (CurrentUpgradeLevel > 0 ? 1 : 0);

        public event Action<int> UpgradeRequested;

        private void OnEnable()
        {
            if (gemPool == null ||
                experience == null ||
                gameState == null)
            {
                Debug.LogError(
                    "LevelUpPauseController is missing references.",
                    this);

                enabled = false;
                return;
            }

            if (levelUpPanel != null)
                levelUpPanel.SetActive(false);

            gemPool.ExperienceCollected += experience.AddExperience;
            experience.LevelUp += HandleLevelUp;

            subscribed = true;
        }

        private void OnDisable()
        {
            if (subscribed)
            {
                if (gemPool != null && experience != null)
                    gemPool.ExperienceCollected -= experience.AddExperience;

                if (experience != null)
                    experience.LevelUp -= HandleLevelUp;

                subscribed = false;
            }

            pendingLevels.Clear();
            CurrentUpgradeLevel = 0;

            if (levelUpPanel != null)
                levelUpPanel.SetActive(false);

            RestoreWeapons();

            if (gameState != null)
                gameState.ResumePlaying();
        }

        private void HandleLevelUp(int newLevel)
        {
            pendingLevels.Enqueue(newLevel);

            if (CurrentUpgradeLevel == 0)
                ShowNextUpgrade();
        }

        private void ShowNextUpgrade()
        {
            if (pendingLevels.Count == 0)
                return;

            if (gameState.CurrentState == GameplayState.Playing)
            {
                if (!gameState.PauseForLevelUp())
                    return;

                SuspendWeapons();
            }

            CurrentUpgradeLevel = pendingLevels.Dequeue();

            if (levelUpPanel != null)
                levelUpPanel.SetActive(true);

            Debug.Log(
                $"Upgrade selection requested: Level {CurrentUpgradeLevel}",
                this);

            UpgradeRequested?.Invoke(CurrentUpgradeLevel);
        }

        public void CompleteUpgradeSelection()
        {
            if (CurrentUpgradeLevel == 0 ||
                gameState.CurrentState != GameplayState.LevelUp)
            {
                return;
            }

            CurrentUpgradeLevel = 0;

            // One choice for each level gained.
            if (pendingLevels.Count > 0)
            {
                ShowNextUpgrade();
                return;
            }

            if (levelUpPanel != null)
                levelUpPanel.SetActive(false);

            RestoreWeapons();
            gameState.ResumePlaying();
        }

        private void SuspendWeapons()
        {
            WeaponBase[] weapons =
                UnityEngine.Object.FindObjectsByType<WeaponBase>(
                    FindObjectsSortMode.None);

            foreach (WeaponBase weapon in weapons)
            {
                if (weapon == null || !weapon.enabled)
                    continue;

                weapon.enabled = false;
                pausedWeapons.Add(weapon);
            }
        }

        private void RestoreWeapons()
        {
            foreach (WeaponBase weapon in pausedWeapons)
            {
                if (weapon != null)
                    weapon.enabled = true;
            }

            pausedWeapons.Clear();
        }

#if UNITY_EDITOR
        [ContextMenu("Debug/Add 53 EXP")]
        private void DebugAddExperience()
        {
            if (Application.isPlaying && experience != null)
                experience.AddExperience(53);
        }

        [ContextMenu("Debug/Complete Upgrade")]
        private void DebugCompleteUpgrade()
        {
            if (Application.isPlaying)
                CompleteUpgradeSelection();
        }
#endif
    }
}
