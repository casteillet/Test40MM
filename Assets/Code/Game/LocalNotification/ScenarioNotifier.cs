using UnityEngine;

public class ScenarioNotifier : MonoBehaviour
{
    private const string SavedTitle = "Sauvegardé";
    private const string LoadedTitle = "Chargé";
    private const string DeletedTitle = "Supprimé";

    private void OnEnable()
    {
        var saveLoadSystem = SaveLoadSystem.Instance;
        if (saveLoadSystem)
        {
            saveLoadSystem.OnGameSaved += OnGameSaved;
            saveLoadSystem.OnGameLoaded += OnGameLoaded;
        }

        var scenarioManager = ScenarioManager.Instance;
        if (scenarioManager)
        {
            scenarioManager.OnScenarioDeleted += OnScenarioDeleted;
        }
    }

    private void OnDisable()
    {
        if (SaveLoadSystem.HasInstance)
        {
            var saveLoadSystem = SaveLoadSystem.Instance;
            saveLoadSystem.OnGameSaved -= OnGameSaved;
            saveLoadSystem.OnGameLoaded -= OnGameLoaded;
        }

        if (ScenarioManager.HasInstance)
        {
            ScenarioManager.Instance.OnScenarioDeleted -= OnScenarioDeleted;
        }
    }

    private void OnGameSaved() => Show(SavedTitle, "Scénarios sauvegardés", NotificationType.Success);
    private void OnGameLoaded() => Show(LoadedTitle, "Scénarios chargés", NotificationType.Info);
    private void OnScenarioDeleted(Scenario scenario) => Show(DeletedTitle, $"{scenario.Name} supprimé", NotificationType.Info);

    private void Show(string title, string message, NotificationType type)
    {
        var notification = new LocalNotification(title, message, type);
        LocalNotificationManager.Instance.Show(notification);
    }
}
