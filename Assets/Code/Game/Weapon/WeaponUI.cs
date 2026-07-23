using TMPro;
using Unity.Netcode;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private PlayerView view;
    [SerializeField] private TextMeshProUGUI ammoToFireText;
    // [SerializeField] private GameObject selectionButtonsRoot;
    
    private void Start()
    {
        var local = NetworkManager.Singleton ? NetworkManager.Singleton.LocalClient?.PlayerObject : null;

        if (!local || !local.TryGetComponent(out Player _)) // Is the host spectator
        {
            view = null;
            // TODO: Find a way to lock these interaction button for the spectator
            // if (selectionButtonsRoot)
            // {
            //     selectionButtonsRoot.SetActive(false);
            // }
        }
    }

    private void OnEnable()
    {
        view.OnAmmoToFireChanged += Refresh;
        Refresh(view.AmmoToFire);
    }

    private void OnDisable()
    {
        if (!view) return;
        
        view.OnAmmoToFireChanged -= Refresh;
    }

    public void SetAmmoToFire(int count)
    {
        view?.RequestAmmoToFireRpc(count);
    }

    private void Refresh(int ammoToFire)
    {
        if (ammoToFireText)
        {
            ammoToFireText.text = ammoToFire.ToString();
        }
    }
}
