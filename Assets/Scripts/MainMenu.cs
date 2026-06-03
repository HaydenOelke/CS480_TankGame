using UnityEngine;

/// <summary>
/// Drives the main menu (title) scene. Hook these methods up to your
/// Button OnClick events in the Inspector:
///   Play        -> PlayGame()
///   How to Play -> ShowTutorial()
///   Back        -> HideTutorial()
///   Quit        -> QuitGame()
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Scenes")]
    [Tooltip("First gameplay scene loaded when Play is pressed.")]
    public string firstScene = "SampleScene";

    [Header("Panels")]
    [Tooltip("The How-to-Play / tutorial panel, hidden until the button is pressed.")]
    public GameObject tutorialPanel;

    void Start()
    {
        // If we came back here from a Game Over, the game may still be frozen.
        Time.timeScale = 1f;

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneLoader.LoadScene(firstScene);
    }

    public void ShowTutorial()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);
    }

    public void HideTutorial()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quit pressed");
        Application.Quit();
    }
}
