using UnityEngine;
using UnityEngine.SceneManagement; 

public class PersistentAudio : MonoBehaviour
{
    private static PersistentAudio instance;

    [Header("Boss Level Settings")]
    [Tooltip("Type the exact name of your final map here (e.g., Map3)")]
    public string bossSceneName;
    public AudioClip bossMusic;

    private AudioSource audioSource;
    private bool isPlayingBossMusic = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            

            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == bossSceneName)
        {

            if (!isPlayingBossMusic && bossMusic != null && audioSource != null)
            {
                audioSource.clip = bossMusic; 
                audioSource.Play();          
                isPlayingBossMusic = true;    
            }
        }
    }
}