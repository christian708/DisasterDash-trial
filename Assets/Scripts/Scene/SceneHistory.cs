using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneHistory
{
    private static readonly List<string> history = new List<string>();

    // Scenes whose names start with these are never "gone back to"
    private static readonly string[] skipPrefixes = { "story", "level" };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        history.Clear();
        history.Add(SceneManager.GetActiveScene().name);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (mode == LoadSceneMode.Single) history.Add(scene.name);
        };
    }

    private static bool ShouldSkip(string name, string current)
    {
        if (name == current) return true;
        foreach (var p in skipPrefixes)
            if (name.StartsWith(p, System.StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    public static string GetPrevious(string fallback)
    {
        string current = SceneManager.GetActiveScene().name;
        for (int i = history.Count - 2; i >= 0; i--)
        {
            if (!ShouldSkip(history[i], current))
            {
                history.RemoveRange(i + 1, history.Count - (i + 1));
                return history[i];
            }
        }
        return fallback;
    }
}