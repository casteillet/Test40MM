using System.Collections.Generic;
using System.Linq;
using BennyKok.RuntimeDebug.Actions;
using BennyKok.RuntimeDebug.Attributes;
using BennyKok.RuntimeDebug.Systems;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PrimeTween;

public class SpectatorManager : NetworkSingleton<SpectatorManager>
{
    // TODO: Add interface Activate / Deactivate to enable or not this script and in deactivate call pointExitHandler on lastHit if exist
    
    [Header("Cursor")]
    [SerializeField] private RectTransform cursorRect;
    [SerializeField] private Graphic cursorClickGraphic;
    
    [Header("UI")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private Camera uiCamera;

    private static readonly int PROGRESS_ID = Shader.PropertyToID("_Progression");
    
    private readonly Dictionary<ulong, Queue<SpectatorPointerState>> pointerStatesQueues = new();

    private ClientPointerSender currentSender;
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
        if (currentSpectatedClient == clientId) return;

        StopCurrentSpectatedClient();

        currentSpectatedClient = clientId;

        StartSpectatedClient(clientId);
    }
    
    public void StopSpectating()
    {
        StopCurrentSpectatedClient();

        currentSpectatedClient = 0;

        Clear();

        HideCursor();
    }
    
    private void StartSpectatedClient(ulong clientId)
    {
        var player = NetworkManager.ConnectedClients[clientId].PlayerObject;
        var sender = player.GetComponent<ClientPointerSender>();
        sender.SetSpectatorListeningStateClientRpc(true);
    }
    
    private void StopCurrentSpectatedClient()
    {
        if (currentSpectatedClient == 0) return;

        var player = NetworkManager.ConnectedClients[currentSpectatedClient].PlayerObject;
        var sender = player.GetComponent<ClientPointerSender>();
        sender.SetSpectatorListeningStateClientRpc(false);
    }
    
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void UpdatePointerServerRpc(SpectatorPointerState state, RpcParams rpcParams = default)
    {
        var senderId = rpcParams.Receive.SenderClientId;
        
        if (!pointerStatesQueues.TryGetValue(senderId, out var queue))
        {
            queue = new Queue<SpectatorPointerState>();
            pointerStatesQueues.Add(senderId, queue);
        }

        queue.Enqueue(state);
    }
    
    private void Update()
    {
        if (!IsServer) return;

        if (!pointerStatesQueues.TryGetValue(currentSpectatedClient, out var queue)) return;
        
        while (queue.Count > 0)
        {
            var state = queue.Dequeue();

            ProcessPointerEvent(state);
        }
    }
    
    private void ProcessPointerEvent(SpectatorPointerState state)
    {
        PointerPosition(state);

        switch (state.eventType)
        {
            case PointerEventType.Move:
                PointerHover(state);
                break;

            case PointerEventType.Down:
                PointerDown(state);
                break;

            case PointerEventType.Up:
                PointerUp(state);
                break;
        }
    }

    private void PointerPosition(SpectatorPointerState state)
    {
        cursorRect.position = GetViewportToScreenPosition(state.viewportPosition);
        
        if (state.eventType == PointerEventType.Down)
        {
            TriggerCursorClickEffect();
        }
    }

    private void TriggerCursorClickEffect()
    {
        ShowCursor();

        Tween.MaterialProperty(cursorClickGraphic.material, PROGRESS_ID, 0, 1, .5f)
            .OnComplete(HideCursor);
    }
    
    private void PointerHover(SpectatorPointerState state)
    {
        var hit = GetCurrentHit(state.viewportPosition);

        if (hit == lastHit) return;

        if (lastHit)
        {
            ExecuteEvents.ExecuteHierarchy(lastHit, eventData, ExecuteEvents.pointerExitHandler);
        }

        if (hit)
        {
            ExecuteEvents.ExecuteHierarchy(hit, eventData, ExecuteEvents.pointerEnterHandler);
        }

        lastHit = hit;
    }
    
    private void PointerDown(SpectatorPointerState state)
    {
        var hit = GetCurrentHit(state.viewportPosition);

        if (!hit)
        {
            Clear();
            return;
        }
        
        ExecuteEvents.ExecuteHierarchy(hit, eventData, ExecuteEvents.pointerDownHandler);
    }
    
    private void PointerUp(SpectatorPointerState state)
    {
        var hit = GetCurrentHit(state.viewportPosition);

        if (!hit)
        {
            Clear();
            return;
        }

        ExecuteEvents.ExecuteHierarchy(hit, eventData, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.ExecuteHierarchy(hit, eventData, ExecuteEvents.pointerClickHandler);
    }
    
    private GameObject GetCurrentHit(Vector2 viewportPosition)
    {
        eventData.Reset();
        
        eventData.position = GetViewportToScreenPosition(viewportPosition);

        var results = new List<RaycastResult>();

        raycaster.Raycast(eventData, results);

        return results.Count > 0 ? results[0].gameObject : null;
    }
    
    private Vector2 GetViewportToScreenPosition(Vector2 viewport)
    {
        return new Vector2(
            viewport.x * Screen.width,
            viewport.y * Screen.height
        );
    }

    private void ShowCursor()
    {
        cursorClickGraphic.enabled = true;
    }

    private void HideCursor()
    {
        cursorClickGraphic.enabled = false;
    }

    private void Clear()
    {
        // ExecuteEvents.ExecuteHierarchy(lastSelectedHit, eventData, ExecuteEvents.pointerExitHandler);
        // EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(null, eventData);
    }
}