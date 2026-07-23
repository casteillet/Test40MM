using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionLogUI : MonoBehaviour
{
    private const float ScrolledToLatestThreshold = .01f;

    [SerializeField] private MissionLogEntryView entryViewPrefab;
    [SerializeField] private RectTransform entriesRoot;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private MissionLogPalette palette = new();
    [SerializeField] private int maxVisibleEntries = 100;

    private readonly List<MissionLogEntryView> entryViews = new();
    private MissionLogEntryFormatter formatter;
    private MissionLog missionLog;

    private void Start()
    {
        formatter = new MissionLogEntryFormatter(palette);
        missionLog = MissionManager.Instance.Log;

        RebuildFromLog();

        missionLog.EntryAppended += HandleEntryAppended;
        missionLog.Cleared += HandleCleared;
    }

    private void OnDestroy()
    {
        if (missionLog == null)
        {
            return;
        }

        missionLog.EntryAppended -= HandleEntryAppended;
        missionLog.Cleared -= HandleCleared;
    }

    private void HandleEntryAppended(MissionLogEntry entry)
    {
        var wasScrolledToLatest = IsScrolledToLatest();
        AppendEntryView(entry);

        if (wasScrolledToLatest)
        {
            ScrollToLatest();
        }
    }

    private void HandleCleared()
    {
        DestroyEntryViews();
    }

    private void RebuildFromLog()
    {
        DestroyEntryViews();

        var entries = missionLog.Entries;
        var firstVisibleIndex = Mathf.Max(0, entries.Count - maxVisibleEntries);
        for (var i = firstVisibleIndex; i < entries.Count; i++)
        {
            AppendEntryView(entries[i]);
        }

        ScrollToLatest();
    }

    private void AppendEntryView(MissionLogEntry entry)
    {
        MissionLogEntryView entryView;
        if (entryViews.Count < maxVisibleEntries)
        {
            entryView = Instantiate(entryViewPrefab, entriesRoot);
        }
        else
        {
            entryView = entryViews[0];
            entryViews.RemoveAt(0);
        }

        entryView.transform.SetAsLastSibling();
        entryView.Render(formatter.Format(entry));
        entryViews.Add(entryView);
    }

    private void DestroyEntryViews()
    {
        entryViews.ForEach(entryView => Destroy(entryView.gameObject));
        entryViews.Clear();
    }

    private bool IsScrolledToLatest()
    {
        return !scrollRect || scrollRect.verticalNormalizedPosition <= ScrolledToLatestThreshold;
    }

    private void ScrollToLatest()
    {
        if (!scrollRect) return;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}