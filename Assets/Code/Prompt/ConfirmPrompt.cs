using System;
using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasGroup))]
public sealed class ConfirmPrompt : Singleton<ConfirmPrompt>, IActivatable
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Button primaryButton;
    [SerializeField] private TextMeshProUGUI primaryLabel;

    [SerializeField] private Button secondaryButton;
    [SerializeField] private TextMeshProUGUI secondaryLabel;

    [SerializeField] private Button tertiaryButton;
    [SerializeField] private TextMeshProUGUI tertiaryLabel;

    [Child, SerializeField] private Canvas canvas;
    [Child, SerializeField] private GraphicRaycaster raycaster;
    [Child, SerializeField] private CanvasGroup canvasGroup;

    private Action onPrimary;
    private Action onSecondary;
    private Action onTertiary;

    public bool IsActive { get; private set; }
    public void Activate() => SetVisibility(true);
    public void Deactivate() => SetVisibility(false);

    private void Start()
    {
        primaryButton.onClick.AddListener(HandlePrimary);
        secondaryButton.onClick.AddListener(HandleSecondary);
        tertiaryButton.onClick.AddListener(HandleTertiary);

        Deactivate();
    }

    public void Show(string title, string message, PromptChoice primary, PromptChoice secondary)
        => Show(title, message, primary, secondary, null);

    public void Show(string title, string message, PromptChoice primary, PromptChoice secondary, PromptChoice? tertiary)
    {
        titleText.text = title;
        messageText.text = message;

        primaryLabel.text = primary.Label;
        onPrimary = primary.OnSelected;

        secondaryLabel.text = secondary.Label;
        onSecondary = secondary.OnSelected;

        var hasTertiary = tertiary.HasValue;
        tertiaryButton.gameObject.SetActive(hasTertiary);

        if (hasTertiary)
        {
            tertiaryLabel.text = tertiary.Value.Label;
            onTertiary = tertiary.Value.OnSelected;
        }
        else
        {
            onTertiary = null;
        }

        Activate();
    }

    private void HandlePrimary()
    {
        var callback = onPrimary;
        Dismiss();
        callback?.Invoke();
    }

    private void HandleSecondary()
    {
        var callback = onSecondary;
        Dismiss();
        callback?.Invoke();
    }

    private void HandleTertiary()
    {
        var callback = onTertiary;
        Dismiss();
        callback?.Invoke();
    }

    private void Dismiss()
    {
        onPrimary = null;
        onSecondary = null;
        onTertiary = null;
        Deactivate();
    }

    private void SetVisibility(bool visible)
    {
        IsActive = visible;

        canvas.enabled = visible;
        raycaster.enabled = visible;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}
