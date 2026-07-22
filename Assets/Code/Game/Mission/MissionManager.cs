public class MissionManager : Singleton<MissionManager>
{
    // TODO : Create a MissionLog holder and a MissionLogUI
    // TODO : Logs can have a type like kill enemy with metadata not showed in log ui but in the end ui ?? or do an external structure for this
    
    public bool IsMissionRunning { get; private set; }

    public void StartMission()
    {
        // TODO: Start countdown (in another script ?), can be paused from here to centralize
        IsMissionRunning = true;
    }

    public void StopMission()
    {
        // TODO: Stop countdown
        IsMissionRunning = false;
    }
}
