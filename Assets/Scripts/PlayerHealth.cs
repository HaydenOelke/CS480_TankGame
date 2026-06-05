using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    
    [Header("UI")]
    public TextMeshProUGUI healthText;
    public Image healthBarFill;
    public float fullBarWidth = 250f;
    
    [Header("Game Over")]
    public GameObject gameOverPanel;

    void Start()
    {
        Debug.Log("GameData.playerHealth on scene load: " + GameData.playerHealth);
        currentHealth = GameData.playerHealth;
        if (healthBarFill != null)
        {
            RectTransform rt = healthBarFill.GetComponent<RectTransform>();
            Vector2 size = rt.sizeDelta;
            size.x = fullBarWidth;
            rt.sizeDelta = size;
        }
        UpdateUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);
        GameData.playerHealth = currentHealth;
        UpdateUI();
        
        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int healAmount)
    {
        if (healAmount <= 0)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        GameData.playerHealth = currentHealth;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
        
        if (healthBarFill != null)
        {
            float ratio = (float)currentHealth / maxHealth;
            
            RectTransform rt = healthBarFill.GetComponent<RectTransform>();
            Vector2 size = rt.sizeDelta;
            size.x = fullBarWidth * ratio;
            rt.sizeDelta = size;
            
            if (ratio > 0.5f)
                healthBarFill.color = Color.Lerp(Color.yellow, Color.green, (ratio - 0.5f) * 2f);
            else
                healthBarFill.color = Color.Lerp(Color.red, Color.yellow, ratio * 2f);
        }
    }

    void Die()
    {
        GameData.ResetRun();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
