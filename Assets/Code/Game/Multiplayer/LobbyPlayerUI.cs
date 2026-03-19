using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerIdText;
    [SerializeField] private TMP_Dropdown dropdown;

    private string playerId;
    private bool isServer;

    private void Awake()
    {
        dropdown.interactable = false;
    }

    public void Initialize(PlayerLobbyState state, bool isServer)
    {
        playerId = state.PlayerId.ToString();
        this.isServer = isServer;

        playerIdText.text = playerId;

        dropdown.options = new List<TMP_Dropdown.OptionData>
        {
            new(nameof(SpawnPosition.None)),
            new(nameof(SpawnPosition.Front)),
            new(nameof(SpawnPosition.Back))
        };

        dropdown.SetValueWithoutNotify((int)state.Spawn);
        dropdown.interactable = isServer;

        if (isServer)
        {
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }
    }

    private void OnDropdownChanged(int value)
    {
        LobbyManager.Instance.AssignSpawn(playerId, (SpawnPosition)value);
    }

    private void OnDestroy()
    {
        if (isServer)
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        }
    }
}