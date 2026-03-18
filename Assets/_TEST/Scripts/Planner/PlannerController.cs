using UnityEngine;

public class PlannerController : MonoBehaviour
{
    private IPlannerTool activeTool;

    private void Update()
    {
        activeTool?.HandleInput();
    }

    public void SetTool(IPlannerTool tool)
    {
        activeTool = tool;
    }
}