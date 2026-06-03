using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central entry point for changing scenes. Instead of calling
/// SceneManager.LoadScene directly, call SceneLoader.LoadScene(target):
/// it remembers the destination, then loads the Loading scene, which
/// async-loads the destination behind a progress bar.
/// </summary>
public static class SceneLoader
{
    /// <summary>Name of the loading scene (must be added to Build Settings).</summary>
    public const string LoadingSceneName = "Loading";

    /// <summary>The scene the loading screen should load next.</summary>
    public static string TargetScene { get; private set; }

    public static void LoadScene(string sceneName)
    {
        TargetScene = sceneName;

        // Make sure we're not still frozen from a Game Over / pause.
        Time.timeScale = 1f;

        SceneManager.LoadScene(LoadingSceneName);
    }
}
