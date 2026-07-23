using System;
using System.Collections.Generic;

public readonly struct MissionLogEntry
{
    public readonly DateTime Timestamp;
    public readonly string MessageFormat;
    public readonly MissionLogType Type;
    public readonly IReadOnlyList<string> PlayerNames;
 
    public MissionLogEntry(DateTime timestamp, string messageFormat, MissionLogType type, IReadOnlyList<string> playerNames)
    {
        Timestamp = timestamp;
        MessageFormat = messageFormat;
        Type = type;
        PlayerNames = playerNames;
    }
 
    public bool HasPlayerNames => PlayerNames != null && PlayerNames.Count > 0;
}