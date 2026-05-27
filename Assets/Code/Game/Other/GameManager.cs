using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : NetworkSingleton<GameManager>
{
    private GameState gameState = GameState.Playing;

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (!IsServer) return;

        gameState = gameState == GameState.Playing 
            ? GameState.Paused 
            : GameState.Playing;

        var isPaused = gameState == GameState.Paused;
        
        SetPause(isPaused);
        SetPauseClientRpc(isPaused);
    }

    private void SetPause(bool paused)
    {
        if (paused)
        {
            PauseRegistry.PauseGame();
        }
        else
        {
            PauseRegistry.ResumeGame();
        }
        
        Debug.Log($"SetPause: {paused}, IsServer: {IsServer}");
    }
    
    [ClientRpc]
    private void SetPauseClientRpc(bool paused)
    {
        SetPause(paused);
    }
}