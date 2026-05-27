using UnityEngine;
using UnityEngine.InputSystem;

public class TestPause : MonoBehaviour, IPausable
{
    private void Awake()
    {
        PauseRegistry.Register(this);
    }

    private void OnDestroy()
    {
        PauseRegistry.Unregister(this);
    }
    
    public void Pause()
    {
        enabled = false;
    }

    public void Resume()
    {
        enabled = true;
    }

    private void Update()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
        
        Debug.Log("Test pause");
    }
}