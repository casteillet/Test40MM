using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public class MissionLogEntryFormatter
{
    private const string TimestampFormat = "HH:mm:ss";

    private readonly string timestampHex;
    private readonly string playerNameHex;
    private readonly string neutralHex;
    private readonly string successHex;
    private readonly string warningHex;
    
    private readonly StringBuilder builder = new(256);

    public MissionLogEntryFormatter(MissionLogPalette palette)
    {
        timestampHex = ColorToHex(palette.TimestampColor);
        playerNameHex = ColorToHex(palette.PlayerNameColor);
        neutralHex = ColorToHex(palette.GetLogColor(MissionLogType.Neutral));
        successHex = ColorToHex(palette.GetLogColor(MissionLogType.Success));
        warningHex = ColorToHex(palette.GetLogColor(MissionLogType.Warning));
    }

    public string Format(MissionLogEntry entry)
    {
        builder.Clear();
        builder.Append("<color=").Append(timestampHex).Append('>');
        builder.Append('[').Append(entry.Timestamp.ToString(TimestampFormat, CultureInfo.InvariantCulture)).Append("] ");
        builder.Append("</color>");
        builder.Append("<color=").Append(GetLogHex(entry.Type)).Append('>');
        builder.Append(ResolveMessage(entry));
        builder.Append("</color>");
        return builder.ToString();
    }

    private string ResolveMessage(MissionLogEntry entry)
    {
        if (!entry.HasPlayerNames) return entry.MessageFormat;

        try
        {
            return string.Format(CultureInfo.InvariantCulture, entry.MessageFormat, WrapPlayerNames(entry.PlayerNames));
        }
        catch (FormatException)
        {
            return entry.MessageFormat;
        }
    }

    private object[] WrapPlayerNames(IReadOnlyList<string> playerNames)
    {
        var wrappedNames = new object[playerNames.Count];
        for (var index = 0; index < playerNames.Count; index++)
        {
            wrappedNames[index] = $"<color={playerNameHex}><noparse>{playerNames[index]}</noparse></color>";
        }

        return wrappedNames;
    }

    private string GetLogHex(MissionLogType severity) => severity switch
    {
        MissionLogType.Success => successHex,
        MissionLogType.Warning => warningHex,
        _ => neutralHex
    };

    private static string ColorToHex(Color color) => "#" + ColorUtility.ToHtmlStringRGB(color);
}