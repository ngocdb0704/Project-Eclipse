using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Core
{
    public sealed class BootLoader : MonoBehaviour
    {
        private const string MainMenuSceneName = "MainMenu";

        private void Start()
        {
            SceneManager.LoadSceneAsync(
                MainMenuSceneName,
                LoadSceneMode.Single);
        }
    }
}