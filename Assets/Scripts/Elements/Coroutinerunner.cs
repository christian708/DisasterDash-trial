using UnityEngine;
using System.Collections;

/// <summary>
/// Runs coroutines that need to keep going even when the object that started
/// them gets deactivated (e.g. a fire's relight timer, since the fire's own
/// GameObject turning off would normally kill any coroutine running on it).
/// Auto-creates itself the first time it's used, persists across scenes.
/// </summary>
public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("CoroutineRunner");
                instance = obj.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
}