using System;

public readonly struct PromptChoice
{
    public readonly string Label;
    public readonly Action OnSelected;

    public PromptChoice(string label, Action onSelected)
    {
        Label = label;
        OnSelected = onSelected;
    }
}