using System.Collections;
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;

[System.Serializable] 
public class Wave
{
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnRate; 
}

public class WaveManager : MonoBehaviour
{
    public Wave[] waves; 
    public Transform[] spawnNodes; 
    public float timeBetweenWaves = 5f;
    public string nextSceneName;
    public int startingWaveNumber = 1;


    public TextMeshProUGUI gameUIText;
    
    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;

    void Start()
    {
        StartCoroutine(SpawnWavesSequence());
    }

    private IEnumerator SpawnWavesSequence()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            currentWaveIndex = startingWaveNumber + i;
            Wave currentWave = waves[i];
            
            enemiesAlive = currentWave.enemyCount;
            UpdateUI();

            for (int j = 0; j < currentWave.enemyCount; j++)
            {
                SpawnEnemy(currentWave.enemyPrefab);
                yield return new WaitForSeconds(currentWave.spawnRate);
            }

            while (enemiesAlive > 0)
            {
                yield return null; 
            }
            Debug.Log("wave finished. i = " + i + " waves.Length = " + waves.Length);
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                Debug.Log("Last wave cleared. Loading scene: " + nextSceneName);
                SceneManager.LoadScene(nextSceneName);
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
            
        }


        gameUIText.text = "All Waves Cleared! YOU WIN!";
    }

    private void SpawnEnemy(GameObject prefab)
    {
        Transform randomNode = spawnNodes[Random.Range(0, spawnNodes.Length)];
        Instantiate(prefab, randomNode.position, randomNode.rotation);
    }

    public void EnemyDefeated()
    {
        enemiesAlive--;
        Debug.Log("enemy defeated. enemies alive: " + enemiesAlive);
        UpdateUI();
    }

private void UpdateUI()
    {

        if (gameUIText != null)
        {
            gameUIText.text = "Wave: " + currentWaveIndex + "   |   Enemies Left: " + enemiesAlive;
        }
        else
        {

            Debug.LogWarning("The UI text was destroyed, but the game is still running!");
        }
    }
}