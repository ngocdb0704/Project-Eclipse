using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        private const string GameplaySceneName = "Gameplay";

        public void Play()
        {
            SceneManager.LoadSceneAsync(
                GameplaySceneName,
                LoadSceneMode.Single);
        }
    }
}