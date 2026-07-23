using System;
using UnityEngine;

public class Timer : MonoBehaviour, IPausable
{
    private TimerPhase _phase = TimerPhase.Idle;
    private double _elapsedSeconds;

    public event Action<TimerPhase> PhaseChanged;

    public TimerPhase Phase => _phase;
    public bool IsRunning => _phase == TimerPhase.Running;
    public double ElapsedSeconds => _elapsedSeconds;

    private void Awake()
    {
        PauseRegistry.Register(this);
    }

    private void OnDestroy()
    {
        PauseRegistry.Unregister(this);
    }

    public void Begin()
    {
        _elapsedSeconds = 0d;
        SetPhase(TimerPhase.Running);
    }

    public void Pause()
    {
        if (_phase != TimerPhase.Running) return;

        SetPhase(TimerPhase.Paused);
    }

    public void Resume()
    {
        if (_phase != TimerPhase.Paused) return;

        SetPhase(TimerPhase.Running);
    }

    public void Stop()
    {
        if (_phase == TimerPhase.Idle || _phase == TimerPhase.Stopped) return;

        SetPhase(TimerPhase.Stopped);
    }

    public void ResetToIdle()
    {
        _elapsedSeconds = 0d;
        SetPhase(TimerPhase.Idle);
    }

    private void Update()
    {
        if (_phase != TimerPhase.Running) return;

        _elapsedSeconds += Time.deltaTime;
    }

    private void SetPhase(TimerPhase phase)
    {
        if (_phase == phase) return;

        _phase = phase;
        PhaseChanged?.Invoke(phase);
    }
}