using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

[System.Serializable] 
public class Wave
{
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnRate; 
}

public class WaveManager : MonoBehaviour
{
    private const int HealthUpgradeAmount = 25;
    private const int DamageUpgradeAmount = 1;
    private const float SpeedUpgradeAmount = 0.15f;

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
                yield return ShowUpgradeChoice();
                Debug.Log("Last wave cleared. Loading scene: " + nextSceneName);
                SceneLoader.LoadScene(nextSceneName);
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
        Transform randomNode = spawnNodes[UnityEngine.Random.Range(0, spawnNodes.Length)];
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

    private IEnumerator ShowUpgradeChoice()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
            yield break;

        bool upgradeChosen = false;
        bool hadGameText = gameUIText != null;
        if (hadGameText)
            gameUIText.gameObject.SetActive(false);

        GameObject panel = BuildUpgradePanel(
            canvas.transform,
            () =>
            {
                ApplyHealthUpgrade();
                upgradeChosen = true;
            },
            () =>
            {
                ApplyDamageUpgrade();
                upgradeChosen = true;
            },
            () =>
            {
                ApplySpeedUpgrade();
                upgradeChosen = true;
            });

        Time.timeScale = 0f;

        while (!upgradeChosen)
            yield return null;

        Time.timeScale = 1f;
        if (hadGameText)
            gameUIText.gameObject.SetActive(true);
        Destroy(panel);
        yield return new WaitForSecondsRealtime(0.15f);
    }

    private void ApplyHealthUpgrade()
    {
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(HealthUpgradeAmount);
            return;
        }

        GameData.AddHealth(HealthUpgradeAmount);
    }

    private void ApplyDamageUpgrade()
    {
        PlayerController playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.IncreaseDamage(DamageUpgradeAmount);
            return;
        }

        GameData.AddDamage(DamageUpgradeAmount);
    }

    private void ApplySpeedUpgrade()
    {
        PlayerController playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.IncreaseSpeed(SpeedUpgradeAmount);
            return;
        }

        GameData.AddSpeedMultiplier(SpeedUpgradeAmount);
    }

    private GameObject BuildUpgradePanel(Transform parent, Action onHealthSelected, Action onDamageSelected, Action onSpeedSelected)
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject overlay = CreateUIObject("UpgradeOverlay", parent);
        Image overlayImage = overlay.AddComponent<Image>();
        overlayImage.color = new Color(0.03f, 0.05f, 0.09f, 0.85f);

        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        GameObject card = CreateUIObject("UpgradeCard", overlay.transform);
        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.12f, 0.16f, 0.23f, 0.98f);

        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(820f, 420f);
        cardRect.anchoredPosition = new Vector2(0f, -20f);

        CreateText("UpgradeTitle", card.transform, font, 34, FontStyle.Bold,
            "Choose Your Upgrade", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(700f, 55f), new Vector2(0f, -56f), TextAnchor.MiddleCenter);

        CreateText("UpgradeSubtitle", card.transform, font, 18, FontStyle.Normal,
            "Lock in one boost before the next map.", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(660f, 40f), new Vector2(0f, -108f), TextAnchor.MiddleCenter, new Color(0.82f, 0.87f, 0.94f, 1f));

        CreateButton(card.transform, font, "RepairButton", "Patch Up\n+25 Health", new Vector2(-220f, -40f), new Color(0.76f, 0.24f, 0.32f, 1f), onHealthSelected);
        CreateButton(card.transform, font, "DamageButton", "High-Cal Shells\n+1 Damage", new Vector2(0f, -40f), new Color(0.87f, 0.62f, 0.18f, 1f), onDamageSelected);
        CreateButton(card.transform, font, "SpeedButton", "Overdrive Treads\n+15% Speed", new Vector2(220f, -40f), new Color(0.2f, 0.62f, 0.78f, 1f), onSpeedSelected);

        return overlay;
    }

    private GameObject CreateButton(Transform parent, Font font, string objectName, string label, Vector2 anchoredPosition, Color buttonColor, Action onClick)
    {
        GameObject buttonObject = CreateUIObject(objectName, parent);
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = buttonColor;

        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = Color.Lerp(buttonColor, Color.white, 0.18f);
        colors.pressedColor = Color.Lerp(buttonColor, Color.black, 0.18f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(() => onClick?.Invoke());

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(190f, 160f);
        buttonRect.anchoredPosition = anchoredPosition;

        CreateText("Label", buttonObject.transform, font, 21, FontStyle.Bold, label,
            Vector2.zero, Vector2.one, new Vector2(-24f, -24f), Vector2.zero, TextAnchor.MiddleCenter);

        return buttonObject;
    }

    private GameObject CreateText(string objectName, Transform parent, Font font, int fontSize, FontStyle fontStyle, string content,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPosition, TextAnchor alignment, Color? color = null)
    {
        GameObject textObject = CreateUIObject(objectName, parent);
        Text text = textObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = alignment;
        text.color = color ?? Color.white;
        text.text = content;
        text.supportRichText = true;

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = anchorMin;
        textRect.anchorMax = anchorMax;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = sizeDelta;
        textRect.anchoredPosition = anchoredPosition;

        return textObject;
    }

    private GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject gameObject = new GameObject(objectName, typeof(RectTransform));
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }
}
