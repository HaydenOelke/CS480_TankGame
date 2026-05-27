using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;
    
    [Header("UI")]
    public TextMeshProUGUI healthText;
    public Image healthBarFill;
    public float fullBarWidth = 250f;
    
    [Header("Game Over")]
    public GameObject gameOverPanel;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void TakeDamage(int damageAmount)
{
    Debug.Log("TakeDamage called: " + damageAmount + " | " + System.Environment.StackTrace);
    currentHealth -= damageAmount;
    currentHealth = Mathf.Max(0, currentHealth);
    UpdateUI();
    
    if (currentHealth <= 0)
        Die();
}

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
        
        if (healthBarFill != null)
        {
            float ratio = (float)currentHealth / maxHealth;
            
            // Shrink width — left edge stays, right edge pulls in
            RectTransform rt = healthBarFill.GetComponent<RectTransform>();
            Vector2 size = rt.sizeDelta;
            size.x = fullBarWidth * ratio;
            rt.sizeDelta = size;
            
            // Color shift: green → yellow → red
            if (ratio > 0.5f)
                healthBarFill.color = Color.Lerp(Color.yellow, Color.green, (ratio - 0.5f) * 2f);
            else
                healthBarFill.color = Color.Lerp(Color.red, Color.yellow, ratio * 2f);
        }
    }

    void Die()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        Time.timeScale = 0f;
    }
}