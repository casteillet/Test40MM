using UnityEngine;

public class ScenarioNotifier : MonoBehaviour
{
    private const string savedTitle = "Sauvegardé";
    private const string loadedTitle = "Chargé";
    private const string deletedTitle = "Supprimé";
    
    private void Start()
    {
        ScenarioManager.Instance.OnScenarioSaved += OnScenarioSaved;
        ScenarioManager.Instance.OnScenarioLoaded += OnScenarioLoaded;
        ScenarioManager.Instance.OnScenarioDeleted += OnScenarioDeleted;
    }

    private void OnScenarioSaved(Scenario scenario)
    {
        var notification = new LocalNotification(savedTitle, $"{scenario.Name} sauvegardé", NotificationType.Success);
        LocalNotificationManager.Instance.Show(notification);
    }
    
    private void OnScenarioLoaded(Scenario scenario)
    {
        var notification = new LocalNotification(loadedTitle, $"{scenario.Name} chargé", NotificationType.Info);
        LocalNotificationManager.Instance.Show(notification);
    }

    private void OnScenarioDeleted(Scenario scenario)
    {
        var notification = new LocalNotification(deletedTitle, $"{scenario.Name} supprimé", NotificationType.Info);
        LocalNotificationManager.Instance.Show(notification);
    }

    private void OnDestroy()
    {
        if (!ScenarioManager.Instance) return;
        
        ScenarioManager.Instance.OnScenarioSaved += OnScenarioSaved;
        ScenarioManager.Instance.OnScenarioLoaded += OnScenarioLoaded;
        ScenarioManager.Instance.OnScenarioDeleted += OnScenarioDeleted;
    }
}
