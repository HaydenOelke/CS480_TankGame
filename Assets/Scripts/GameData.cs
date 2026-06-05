using UnityEngine;

public static class GameData
{
    public static int playerHealth = 100;
    public static int playerDamage = 1;
    public static float playerSpeedMultiplier = 1f;

    public static void ResetRun()
    {
        playerHealth = 100;
        playerDamage = 1;
        playerSpeedMultiplier = 1f;
    }

    public static void AddDamage(int amount)
    {
        if (amount <= 0)
            return;

        playerDamage += amount;
    }

    public static void AddSpeedMultiplier(float amount)
    {
        if (amount <= 0f)
            return;

        playerSpeedMultiplier += amount;
    }

    public static void AddHealth(int amount, int maxHealth = 100)
    {
        if (amount <= 0)
            return;

        playerHealth = Mathf.Min(maxHealth, playerHealth + amount);
    }
}
