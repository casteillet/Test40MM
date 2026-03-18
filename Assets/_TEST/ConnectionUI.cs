using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using BennyKok.RuntimeDebug.Attributes;
using BennyKok.RuntimeDebug.Components;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionUI : RuntimeDebugBehaviour
{
    public Button startHostButton;
    public Button startClientButton;
    public Button stopButton;

    private void Start()
    {
        ActivateButtons();
        
        startHostButton.onClick.AddListener(StartHost);
        startClientButton.onClick.AddListener(StartClient);
        
        stopButton.onClick.AddListener(Stop);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        startHostButton.onClick.RemoveListener(StartHost);
        startClientButton.onClick.RemoveListener(StartClient);
        
        stopButton.onClick.RemoveListener(Stop);
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        DeactivateButtons();
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        DeactivateButtons();
    }
    
    private void Stop()
    {
        NetworkManager.Singleton.Shutdown();
        ActivateButtons(); 
    }
    
    private void ActivateButtons()
    {
        startHostButton.interactable = true;
        startClientButton.interactable = true;
        
        stopButton.interactable = false;
    }

    private void DeactivateButtons()
    {
        startHostButton.interactable = false;
        startClientButton.interactable = false;
        
        stopButton.interactable = true;
    }
    
    [DebugAction]
    public void LogLocalEthernetIPv4()
    {
        Debug.Log($"Ethernet: {NetworkHelper.GetLocalIPv4(NetworkInterfaceType.Ethernet)}");
    }
    
    [DebugAction]
    public void LogLocalWirelessIPv4()
    {
        Debug.Log($"Wireless: {NetworkHelper.GetLocalIPv4(NetworkInterfaceType.Wireless80211)}");
    }
}