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

    public void UpdatePathRenderer(EntityPath entityPath)
    {
        if (entityPath == null || entityPath.Waypoints.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = entityPath.Waypoints.Count;

        lineRenderer.SetPositions(
            entityPath.Waypoints.Select(w => w.Position + Vector3.up * .05f).ToArray()
        );
    }
}