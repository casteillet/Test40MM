using UnityEngine;
using VInspector;

public class TestPrompt : MonoBehaviour
{
    public string title;
    public string message;

    [Button]
    public void Test()
    {
        ConfirmPrompt.Instance.Show(
            title, 
            message,
            new PromptChoice("1", () => Debug.Log("1")),
            new PromptChoice("2", () => Debug.Log("2")));    
    }
}
