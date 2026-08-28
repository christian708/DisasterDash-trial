[System.Serializable]
public class SaveData
{
    public string currentScene = "Level0_Fire_Interior";
    public int hearts = 3;
    public float playerX = 0f;
    public float playerY = 0f;
    public string selectedCharacter = "Boy";

    // Add more fields here as your save needs grow:
    // e.g. public bool[] completedLevels;
    // e.g. public int coins;
}