using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // "true" = pretty print, easier to read/debug
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveSystem] Saved to: {SavePath}");
    }

    public static SaveData Load()
    {
        if (!SaveExists())
        {
            Debug.LogWarning("[SaveSystem] No save file found. Returning new default data.");
            return new SaveData();
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void DeleteSave()
    {
        if (SaveExists())
        {
            File.Delete(SavePath);
            Debug.Log("[SaveSystem] Save file deleted.");
        }
    }
}