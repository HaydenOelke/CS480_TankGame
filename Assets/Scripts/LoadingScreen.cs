using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Lives in the Loading scene. Async-loads whatever scene SceneLoader was
/// asked for, updating a progress bar / percent label, then activates it.
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    [Header("Progress UI (all optional - wire up what you use)")]
    [Tooltip("A UI Slider whose value (0-1) tracks load progress.")]
    public Slider progressBar;
    [Tooltip("A filled Image whose fillAmount (0-1) tracks load progress.")]
    public Image progressFill;
    [Tooltip("Text showing the percentage, e.g. \"57%\".")]
    public TextMeshProUGUI percentText;

    [Header("Settings")]
    [Tooltip("Scene to load if this scene is opened directly and no target was set.")]
    public string fallbackScene = "MainMenu";
    [Tooltip("Minimum seconds to keep the loading screen visible.")]
    public float minDisplayTime = 0.5f;

    void Start()
    {
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        string target = string.IsNullOrEmpty(SceneLoader.TargetScene)
            ? fallbackScene
            : SceneLoader.TargetScene;

        // Use unscaled time so a frozen timeScale can't stall the bar.
        float startTime = Time.unscaledTime;

        AsyncOperation op = SceneManager.LoadSceneAsync(target);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            // Unity reports 0 -> 0.9 during load, then holds at 0.9 until
            // we allow activation. Remap that to a clean 0 -> 1 bar.
            float loadProgress = Mathf.Clamp01(op.progress / 0.9f);
            SetProgress(loadProgress);

            bool ready = op.progress >= 0.9f;
            bool minTimeElapsed = Time.unscaledTime - startTime >= minDisplayTime;

            if (ready && minTimeElapsed)
            {
                SetProgress(1f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void SetProgress(float value)
    {
        if (progressBar != null) progressBar.value = value;
        if (progressFill != null) progressFill.fillAmount = value;
        if (percentText != null) percentText.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
