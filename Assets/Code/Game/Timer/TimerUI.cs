using System.Text;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Timer _timer;
    [SerializeField] private TMP_Text _timeLabel;
    [SerializeField] private GameObject _pausedIndicator;

    private readonly StringBuilder _labelBuilder = new StringBuilder(8);
    private int _displayedSeconds = InvalidSecond;

    private const int InvalidSecond = -1;

    private void OnEnable()
    {
        _displayedSeconds = InvalidSecond;
        _timer.PhaseChanged += HandlePhaseChanged;
        HandlePhaseChanged(_timer.Phase);
    }

    private void OnDisable()
    {
        _timer.PhaseChanged -= HandlePhaseChanged;
    }

    private void Update()
    {
        int seconds = Mathf.FloorToInt((float)_timer.ElapsedSeconds);

        if (seconds == _displayedSeconds) return;

        _displayedSeconds = seconds;
        WriteLabel(seconds);
    }

    private void WriteLabel(int totalSeconds)
    {
        _labelBuilder.Clear();
        AppendPadded(_labelBuilder, totalSeconds / 60);
        _labelBuilder.Append(':');
        AppendPadded(_labelBuilder, totalSeconds % 60);
        _timeLabel.SetText(_labelBuilder);
    }

    private static void AppendPadded(StringBuilder builder, int value)
    {
        if (value < 10)
        {
            builder.Append('0');
        }
        
        builder.Append(value);
    }

    private void HandlePhaseChanged(TimerPhase phase)
    {
        _pausedIndicator?.SetActive(phase == TimerPhase.Paused);
    }
}