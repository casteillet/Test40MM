using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Render(Path path)
    {
        if (path == null || path.Waypoints.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = path.Waypoints.Count;

        lineRenderer.SetPositions(
            path.Waypoints.Select(w => w.Position + Vector3.up * .05f).ToArray()
        );
    }
}