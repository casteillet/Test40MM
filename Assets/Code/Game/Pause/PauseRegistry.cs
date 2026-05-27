using System.Collections.Generic;

public static class PauseRegistry
{
    private static readonly HashSet<IPausable> pausables = new();

    public static void Register(IPausable pausable)
    {
        pausables.Add(pausable);
    }

    public static void Unregister(IPausable pausable)
    {
        pausables.Remove(pausable);
    }
    
    public static void PauseGame()
    {
        foreach (var pausable in pausables)
        {
            pausable.Pause();
        }
    }

    public static void ResumeGame()
    {
        foreach (var pausable in pausables)
        {
            pausable.Resume();
        }
    }
}