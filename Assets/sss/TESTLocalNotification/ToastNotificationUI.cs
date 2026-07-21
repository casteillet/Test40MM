using PrimeTween;
using TMPro;
using UnityEngine;

public class ToastNotificationUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    
    [SerializeField] private float showDuration = .25f;
    [SerializeField] private float hideDuration = .2f;
    
    [SerializeField] private Vector2 hiddenOffset = new(0, -64);
    
    private Vector2 originalPosition;
    private Sequence sequence;
    
    private Vector2 HiddenPosition => originalPosition + hiddenOffset;

    private void Awake()
    {
        originalPosition = rectTransform.anchoredPosition;
        
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        Hide();
    }

    public void Show(LocalNotification notification)
    {
        if (sequence.isAlive)
        {
            sequence.Stop();
        }
        
        Init(notification);
        Hide();

        sequence = Sequence.Create()
            .Chain(Tween.Alpha(canvasGroup, 1f, showDuration, Ease.OutCubic))
            .Group(Tween.UIAnchoredPosition(rectTransform, originalPosition, showDuration, Ease.OutCubic))
            .ChainDelay(notification.Duration)
            .Chain(Tween.Alpha(canvasGroup, 0f, hideDuration, Ease.InCubic))
            .Group(Tween.UIAnchoredPosition(rectTransform, HiddenPosition, hideDuration, Ease.InCubic));
    }

    private void Init(LocalNotification notification)
    {
        titleText.text = notification.Title;
        messageText.text = notification.Message;
    }

    private void Hide()
    {
        canvasGroup.alpha = 0;
        rectTransform.anchoredPosition = HiddenPosition;
    }
}