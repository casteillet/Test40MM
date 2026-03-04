using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

public class EntitySpawner : ValidatedMonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform uiContainer;

    private AgentFactory agentFactory;
    
    public event Action OnAgentSpawned;

    private void Start()
    {
        agentFactory = new AgentFactory();
        
        OnAgentSpawned?.Invoke();
    }
}