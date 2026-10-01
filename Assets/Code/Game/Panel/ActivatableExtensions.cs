using System.Collections.Generic;

public static class ActivatableExtensions
{
    public static void ActivateExclusive(this IEnumerable<IActivatable> activatables, IActivatable target)
    {
        foreach (var activatable in activatables)
        {
            if (activatable != target)
            {
                activatable.Deactivate();
            }
        }

        target.Activate();
    }

    public static void DeactivateAll(this IEnumerable<IActivatable> activatables)
    {
        foreach (var activatable in activatables)
        {
            activatable.Deactivate();
        }
    }
}
