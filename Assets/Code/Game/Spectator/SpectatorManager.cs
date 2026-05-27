using System.Collections.Generic;
using System.Linq;
using BennyKok.RuntimeDebug.Actions;
using BennyKok.RuntimeDebug.Attributes;
using BennyKok.RuntimeDebug.Systems;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpectatorManager : NetworkSingleton<SpectatorManager>
{
    // TODO: Add interface Activate / Deactivate to enable or not this script and in deactivate call pointExitHandler on lastHit if exist
    
    [Header("Cursor")]
    [SerializeField] private Image cursorImage;
    
    [Header("UI")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private Camera uiCamera;

    private readonly Dictionary<ulong, SpectatorPointerState> pointerStates = new();

    private ulong currentSpectatedClient;

    private PointerEventData eventData;
    private EventSystem eventSystem;
    private GameObject lastHit;
    
    private BaseDebugAction[] actions;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        HideCursor();
        
        if (!IsServer) return;
        
        eventSystem = EventSystem.current;
        eventData = new PointerEventData(eventSystem);
        
        actions = RuntimeDebugSystem.RegisterActionsAuto(this);
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        
        if (!IsServer) return;
        
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
        HideCursor();
    }
    
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void UpdatePointerServerRpc(ulong clientId, SpectatorPointerState state)
    {
        if (!IsServer) return;

        pointerStates[clientId] = state;
        
        // Debug.Log($"{clientId}: {state.screenPosition}, {state.isClicking}");
    }
    
    private void Update()
    {
        if (!IsServer) return;

        if (!pointerStates.TryGetValue(currentSpectatedClient, out var state))
        {
            HideCursor();
            return;
        }
        
        ShowCursor();
        
        PointerPosition(state);
        PointerInteraction(state);
    }

    private void PointerPosition(SpectatorPointerState state)
    {
        // var screenPosition = new Vector2(
        //     (viewportPosition.x - .5f) * cursorRectTransform.sizeDelta.x,
        //     (viewportPosition.y - .5f) * cursorRectTransform.sizeDelta.y
        // );
        
        cursorImage.transform.position = state.screenPosition;
        cursorImage.color = state.isClicking ? Color.red : Color.white;
    }

    private void PointerInteraction(SpectatorPointerState state)
    {
        eventData.Reset();
        eventData.position = state.screenPosition;

        var results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);
        
        if (results.Count == 0)
        {
            Clear();
            return;
        }

        var hit = results[0].gameObject;

        if (lastHit && hit != lastHit)
        {
            Clear();
            return;
        }
        
        ExecuteEvents.ExecuteHierarchy(hit, eventData, ExecuteEvents.pointerEnterHandler);

        if (state.isClicking)
        {
            ExecuteEvents.ExecuteHierarchy(hit, eventData, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.ExecuteHierarchy(hit, eventData, ExecuteEvents.pointerClickHandler);
        }
        
        lastHit = hit;
    }

    private void ShowCursor()
    {
        cursorImage.gameObject.SetActive(true);
    }

    private void HideCursor()
    {
        cursorImage.gameObject.SetActive(false);
    }

    private void Clear()
    {
        if (!lastHit) return;
        
        ExecuteEvents.ExecuteHierarchy(lastHit, eventData, ExecuteEvents.pointerExitHandler);
        lastHit = null;
    }
}