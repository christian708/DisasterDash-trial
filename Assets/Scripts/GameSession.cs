public static class GameSession
{
    // Holds the save data that was just loaded (or freshly created)
    // so the next scene's GameManager can read it in Start().
    public static SaveData LoadedData;
}