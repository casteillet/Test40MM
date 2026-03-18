using TMPro;
using UnityEngine;

public class EntityInfoUI : MonoBehaviour
{
    public TextMeshProUGUI factionText;

    private Entity observed;

    public void Observe(Entity entity)
    {
        observed = entity;
    }

    private void Update()
    {
        if (!observed) return;

        factionText.text = observed.Threat.ToString();
    }
}