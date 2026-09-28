using UnityEngine;
using UnityEngine.UI;

public class ScenarioEditController : Singleton<ScenarioEditController>
{
    [SerializeField] private PanelId editorPanel;
    [SerializeField] private PanelId homePanel;
    [SerializeField] private Button homeButton;

    private const string UnsavedTitle = "Modifications non sauvegardées";
    private const string UnsavedMessage = "Voulez-vous sauvegarder les modifications du scénario en cours ?";

    private const string SaveLabel = "Sauvegarder";
    private const string DiscardLabel = "Ignorer";
    private const string CancelLabel = "Annuler";

    private void Start()
    {
        homeButton.onClick.AddListener(RequestClose);
        homeButton.interactable = ScenarioManager.Instance.CurrentMode == ScenarioEditMode.Editing;
    }

    public void OpenEditor(Scenario scenario)
    {
        ScenarioManager.Instance.Select(scenario);
        PanelManager.Instance.ActivateExclusive(editorPanel);
    }

    public void RequestClose()
    {
        if (!ScenarioManager.Instance.IsDirty)
        {
            Close();
            return;
        }

        ConfirmPrompt.Instance.Show(
            UnsavedTitle,
            UnsavedMessage,
            new PromptChoice(SaveLabel, SaveThenClose),
            new PromptChoice(DiscardLabel, DiscardThenClose),
            new PromptChoice(CancelLabel, null));
    }

    private void SaveThenClose()
    {
        SaveLoadSystem.Instance.SaveGame();
        Close();
    }

    private void DiscardThenClose()
    {
        SaveLoadSystem.Instance.DiscardChanges();
        Close();
    }

    private void Close() => PanelManager.Instance.ActivateExclusive(homePanel);
}
