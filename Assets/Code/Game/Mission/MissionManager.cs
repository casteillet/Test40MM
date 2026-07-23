using System;
using VInspector;

public class MissionManager : Singleton<MissionManager>
{
    public bool IsMissionRunning { get; private set; }
    public MissionLog Log { get; } = new();

    public event Action MissionStarted;
    public event Action MissionStopped;

    [Button]
    public void StartMission()
    {
        // TODO: Start countdown (in another script ?), can be paused from here to centralize
        
        if (IsMissionRunning) return;

        Log.Clear();
        IsMissionRunning = true;
        MissionStarted?.Invoke();
    }

    [Button]
    public void StopMission()
    {
        // TODO: Stop countdown
        if (!IsMissionRunning)  return;

        IsMissionRunning = false;
        MissionStopped?.Invoke();
    }
    
    #if UNITY_EDITOR
    [Button]
    public void TestLog()
    {
        Log.Append("{0} destroyed a target", MissionLogType.Success, "playerName1");
        Log.Append("Target reassigned from {0} to {1}", MissionLogType.Warning, "playerName1", "playerName2");
        Log.Append("Mission started");
    }
    #endif
}