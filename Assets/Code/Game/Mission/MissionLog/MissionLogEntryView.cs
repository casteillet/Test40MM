using TMPro;
using UnityEngine;

public class MissionLogEntryView : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    public void Render(string richText)
    {
        label.text = richText;
    }
}