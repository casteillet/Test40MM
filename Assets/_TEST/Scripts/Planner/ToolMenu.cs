using UnityEngine;

public class ToolMenu : MonoBehaviour
{
    public PlannerController planner;

    public Camera cam;

    public void SelectPathTool()
    {
        planner.SetTool(new PathDrawingTool(cam));
    }

    public void SelectCommandTool()
    {
        planner.SetTool(new CommandPlacementTool(cam));
    }

    public void SelectSelectionTool()
    {
        planner.SetTool(new EntitySelectionTool(cam));
    }
}