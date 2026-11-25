using UnityEngine;

public static class StarManager
{
    public const int StarsPerLevel = 3;
    private const int MaxLevels = 12;

    private static string Key(int level, int starIndex)
    {
        return "Level" + level + "_Star" + starIndex;
    }

    public static bool IsCollected(int level, int starIndex)
    {
        return PlayerPrefs.GetInt(Key(level, starIndex), 0) == 1;
    }

    public static void Collect(int level, int starIndex)
    {
        PlayerPrefs.SetInt(Key(level, starIndex), 1);
        PlayerPrefs.Save();
    }

    public static int GetLevelStars(int level)
    {
        int count = 0;
        for (int i = 1; i <= StarsPerLevel; i++)
        {
            if (IsCollected(level, i)) count++;
        }
        return count;
    }

    public static void ResetAll()
    {
        for (int level = 1; level <= MaxLevels; level++)
        {
            for (int i = 1; i <= StarsPerLevel; i++)
            {
                PlayerPrefs.DeleteKey(Key(level, i));
            }
        }
        PlayerPrefs.Save();
    }
}