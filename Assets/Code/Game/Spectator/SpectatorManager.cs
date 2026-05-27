using System.Collections.Generic;
using System.Linq;
using BennyKok.RuntimeDebug.Actions;
using BennyKok.RuntimeDebug.Attributes;
using BennyKok.RuntimeDebug.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpectatorManager : NetworkSingleton<SpectatorManager>
{
    [SerializeField] private RectTransform cursorVisual;
    [SerializeField] private Canvas replayCanvas;
    [SerializeField] private Camera uiCamera;

    private readonly Dictionary<ulong, SpectatorPointerState> pointerStates = new();

    private ulong currentSpectatedClient;

    private PointerEventData eventData;
    private EventSystem eventSystem;
    private GraphicRaycaster raycaster;
    
    private BaseDebugAction[] actions;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        eventSystem = EventSystem.current;
        raycaster = replayCanvas.GetComponent<GraphicRaycaster>();
        eventData = new PointerEventData(eventSystem);
            
        actions = RuntimeDebugSystem.RegisterActionsAuto(this);
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        
        RuntimeDebugSystem.UnregisterActions(actions);
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [DebugAction]
    public void TestSetSpectatedClient()
    {
        if (!IsServer) return;

        var clientId = NetworkManager.ConnectedClientsIds.First();
        SetSpectatedClient(clientId);
    }

    [DebugAction]
    public void TestStopSpectating()
    {
        if (!IsServer) return;

        StopSpectating();
    }
#endif
    
    public void SetSpectatedClient(ulong clientId)
    {
        currentSpectatedClient = clientId;
    }

    public void StopSpectating()
    {
        currentSpectatedClient = 0;
    }
    
    public void UpdatePointer(ulong clientId, SpectatorPointerState state)
    {
        if (!IsServer) return;

        pointerStates[clientId] = state;
    }
    
    private void Update()
    {
        if (!IsServer) return;
        
        if (!pointerStates.TryGetValue(currentSpectatedClient, out var state)) return;

        RenderRemotePointer(state);
        ReplayUIInteraction(state);
    }

    private void RenderRemotePointer(SpectatorPointerState state)
    {
        cursorVisual.anchoredPosition = state.ScreenPosition;
        cursorVisual.gameObject.SetActive(true);
    }

    private void ReplayUIInteraction(SpectatorPointerState state)
    {
        eventData.Reset();
        eventData.position = state.ScreenPosition;

        var results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);
        if (results.Count == 0) return;

        var hit = results[0].gameObject;

        ExecuteEvents.ExecuteHierarchy(hit, eventData, ExecuteEvents.pointerEnterHandler);

        if (state.IsClicking)
        {
            ExecuteEvents.ExecuteHierarchy(hit, eventData, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.ExecuteHierarchy(hit, eventData, ExecuteEvents.pointerClickHandler);
        }
    }
}