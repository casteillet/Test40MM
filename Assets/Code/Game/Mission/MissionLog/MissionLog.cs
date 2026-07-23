using System;
using System.Collections.Generic;

public class MissionLog
{
    private readonly List<MissionLogEntry> entries = new();
    private readonly Func<DateTime> timestampProvider;

    public IReadOnlyList<MissionLogEntry> Entries => entries;

    public event Action<MissionLogEntry> EntryAppended;
    public event Action Cleared;

    public MissionLog(Func<DateTime> timestampProvider = null)
    {
        this.timestampProvider = timestampProvider ?? (() => DateTime.Now);
    }

    public void Append(string messageFormat, MissionLogType type = MissionLogType.Neutral, params string[] playerNames)
    {
        var entry = new MissionLogEntry(timestampProvider(), messageFormat, type, playerNames);
        entries.Add(entry);
        EntryAppended?.Invoke(entry);
    }

    public void Clear()
    {
        if (entries.Count == 0) return;

        entries.Clear();
        Cleared?.Invoke();
    }
}
