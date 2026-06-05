public static class GameData
{
    public static int playerHealth = 100;
    public static int playerDamage = 1;

    public static void ResetRun()
    {
        playerHealth = 100;
        playerDamage = 1;
    }

    public static void AddDamage(int amount)
    {
        if (amount <= 0)
            return;

        playerDamage += amount;
    }
}
