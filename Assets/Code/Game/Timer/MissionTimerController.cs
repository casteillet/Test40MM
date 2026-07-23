using UnityEngine;

public class MissionTimerController : MonoBehaviour
{
    [SerializeField] private Timer _timer;

    private MissionManager _missionManager;

    private void Start()
    {
        _missionManager = MissionManager.Instance;
        _missionManager.MissionStarted += HandleMissionStarted;
        _missionManager.MissionStopped += HandleMissionStopped;

        if (_missionManager.IsMissionRunning) _timer.Begin();
    }

    private void OnDestroy()
    {
        if (_missionManager == null) return;

        _missionManager.MissionStarted -= HandleMissionStarted;
        _missionManager.MissionStopped -= HandleMissionStopped;
    }

    private void HandleMissionStarted()
    {
        _timer.Begin();
    }

    private void HandleMissionStopped()
    {
        _timer.Stop();
    }
}