using System;
using UnityEngine;

[Serializable]
public class MissionLogPalette
{
    [SerializeField] private Color timestampColor = new Color(0.55f, 0.58f, 0.62f);
    [SerializeField] private Color playerNameColor = new Color(0.30f, 0.62f, 1f);
    [SerializeField] private Color neutralColor = new Color(0.90f, 0.92f, 0.94f);
    [SerializeField] private Color successColor = new Color(0.28f, 0.82f, 0.42f);
    [SerializeField] private Color warningColor = new Color(1f, 0.62f, 0.16f);
 
    public Color TimestampColor => timestampColor;
    public Color PlayerNameColor => playerNameColor;
 
    public Color GetLogColor(MissionLogType type) => type switch
    {
        MissionLogType.Success => successColor,
        MissionLogType.Warning => warningColor,
        _ => neutralColor
    };
}