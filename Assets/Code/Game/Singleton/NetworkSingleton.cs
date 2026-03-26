using UnityEngine;
using Unity.Netcode;

public class NetworkSingleton<T> : NetworkBehaviour where T : Component {
    protected static T instance;

    public static bool HasInstance => instance != null;
    public static T TryGetInstance() => HasInstance ? instance : null;

    public static T Instance {
        get {
            if (instance == null) {
                instance = FindAnyObjectByType<T>();
            }

            return instance;
        }
    }

    /// <summary>
    /// Make sure to call base.Awake() in override if you need awake.
    /// </summary>
    protected virtual void Awake() {
        InitializeSingleton();
    }

    protected virtual void InitializeSingleton() {
        if (!Application.isPlaying) return;

        instance = this as T;
    }
}
