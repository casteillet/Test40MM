using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerIdText;
    [SerializeField] private TMP_Dropdown dropdown;

    private Player player;
    private string playerId;
    private bool isServer;

    public void Initialize(Player player, string playerId, bool isServer)
    {
        this.player = player;
        this.playerId = playerId;
        this.isServer = isServer;

        playerIdText.text = playerId;
        
        dropdown.options = new List<TMP_Dropdown.OptionData>
        {
            new (nameof(SpawnPosition.None)),
            new (nameof(SpawnPosition.Front)),
            new (nameof(SpawnPosition.Back))
        };

        dropdown.SetValueWithoutNotify((int)player.spawn.Value);
        dropdown.interactable = isServer;

        if (isServer)
        {
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }

        player.spawn.OnValueChanged += OnSpawnChanged;
    }

    private void OnDropdownChanged(int value)
    {
        LobbyManager.Instance.AssignSpawn(playerId, (SpawnPosition)value);
    }

    private void OnSpawnChanged(SpawnPosition oldValue, SpawnPosition newValue)
    {
        dropdown.SetValueWithoutNotify((int)newValue);
    }

    private void OnDestroy()
    {
        if (player)
        {
            player.spawn.OnValueChanged -= OnSpawnChanged;
        }

        if (isServer)
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        }
    }
}