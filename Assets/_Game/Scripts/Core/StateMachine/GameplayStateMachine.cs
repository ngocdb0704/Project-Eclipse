
using System;
using UnityEngine;

namespace Eclipse.Core.StateMachine
{
    public enum GameplayState
    {
        Playing,
        LevelUp
    }

    [DisallowMultipleComponent]
    public sealed class GameplayStateMachine : MonoBehaviour
    {
        public GameplayState CurrentState { get; private set; }
            = GameplayState.Playing;

        public event Action<GameplayState> StateChanged;

        private float timeScaleBeforePause = 1f;

        public bool PauseForLevelUp()
        {
            if (CurrentState != GameplayState.Playing)
                return false;

            timeScaleBeforePause = Time.timeScale;

            CurrentState = GameplayState.LevelUp;
            Time.timeScale = 0f;

            StateChanged?.Invoke(CurrentState);
            return true;
        }

        public bool ResumePlaying()
        {
            if (CurrentState != GameplayState.LevelUp)
                return false;

            CurrentState = GameplayState.Playing;
            Time.timeScale = timeScaleBeforePause;

            StateChanged?.Invoke(CurrentState);
            return true;
        }

        private void OnDisable()
        {
            // Prevent a scene transition or stopping Play Mode
            // from leaving the game time frozen.
            if (CurrentState == GameplayState.LevelUp)
            {
                Time.timeScale = timeScaleBeforePause;
                CurrentState = GameplayState.Playing;
            }
        }
    }
}
