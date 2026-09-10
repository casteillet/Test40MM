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
    
    [Child, SerializeField] private Canvas canvas;
    [Child, SerializeField] private GraphicRaycaster raycaster;
    [Child, SerializeField] private CanvasGroup canvasGroup;

    private Action onPrimary;
    private Action onSecondary;
    
    public bool IsActive { get; private set; }
    public void Activate() => SetVisibility(true);
    public void Deactivate() => SetVisibility(false);
    
    private void Start()
    {
        primaryButton.onClick.AddListener(HandlePrimary);
        secondaryButton.onClick.AddListener(HandleSecondary);
        
        Deactivate();
    }

    public void Show(string title, string message, PromptChoice primary, PromptChoice secondary)
    {
        titleText.text = title;
        messageText.text = message;
        
        primaryLabel.text = primary.Label;
        secondaryLabel.text = secondary.Label;
        
        onPrimary = primary.OnSelected;
        onSecondary = secondary.OnSelected;
        
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

    private void Dismiss()
    {
        onPrimary = null;
        onSecondary = null;
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