using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

public enum PathEditMode { None, DrawingNew, RedrawingFromExisting }

public class EntityCommandPlanner : ValidatedMonoBehaviour
{
    [SerializeField] private LayerMask agentLayer;
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField, Range(.3f, 1f)] private float waypointDistance = .3f;
    [SerializeField] private float waypointDistanceTolerance = .4f;
    [SerializeField] private float waypointSnapDistance = .4f;

    [HideInInspector, SerializeField, Scene] private Camera mainCamera;

    private bool inputDisabled;

    private PathEditMode editMode = PathEditMode.None;
    private Vector3 lastWaypointPosition;
    private bool hasFirstPoint;
    private bool wasSafeNavMeshPosition;
    
    private EntityCommandPath currentPathForRotation;
    private int currentWaypointIndexForRotation;
    private Vector3 rotationCommandStartPos;
    private bool isAwaitingRotationEnd;
    
    private Entity targetEntity;

    private void Update()
    {
        if (inputDisabled) return;
        
        HandlePathDrawingInput();
        HandleRotationCommandInput();
        HandleRemoveCommandInput();
    }

    private void HandlePathDrawingInput()
    {
        HandlePathDrawingStart();
        HandlePathDrawingUpdate();
        HandlePathDrawingEnd();
    }

    private void HandlePathDrawingStart()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetAgent(out var agent))
            {
                BeginPathDrawing(agent);
            }
        }
    }

    private void HandlePathDrawingUpdate()
    {
        if (Input.GetMouseButton(0) && editMode != PathEditMode.None)
        {
            // TODO: Later find a way to fetch sample position even outside the map
            if (RaycastLayer(walkableLayer + interactableLayer, out var hit))
            {
                HandleWaypointPlacement(hit);
            }
            else
            {
                wasSafeNavMeshPosition = false;
            }
        }
    }

    private void HandlePathDrawingEnd()
    {
        if (Input.GetMouseButtonUp(0) && editMode != PathEditMode.None)
        {
            EndPathDrawing();
        }
    }

    private void HandleRotationCommandInput() 
    {
        HandleRotationCommandInputStart();
        HandleRotationCommandInputEnd();
    }

    private void HandleRotationCommandInputStart()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!TryGetClosestWaypointIndex(out var path, out var index)) return;

            currentPathForRotation = path;
            currentWaypointIndexForRotation = index;
            rotationCommandStartPos = path.Waypoints[index].Position;
            isAwaitingRotationEnd = true;
        }
    }

    private void HandleRotationCommandInputEnd()
    {
        if (Input.GetMouseButtonUp(1) && isAwaitingRotationEnd)
        {
            isAwaitingRotationEnd = false;

            if (!RaycastLayer(walkableLayer, out var hit)) return;

            var direction = (hit.point - rotationCommandStartPos).normalized;

            ICommand rotationCommand;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                rotationCommand = new LookAtCommand(hit.point);
                Debug.DrawRay(rotationCommandStartPos, direction * 2f, Color.red, 2f);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                rotationCommand = new LockAimCommand(direction);
                Debug.DrawRay(rotationCommandStartPos, direction * 2f, Color.blue, 2f);
            }
            else
            {
                rotationCommand = new QuickLookCommand(direction, 6);
                Debug.DrawRay(rotationCommandStartPos, direction * 2f, Color.yellow, 2f);
            }

            TryAddCommandAt(currentPathForRotation, currentWaypointIndexForRotation, rotationCommand);
        }
    }

    private void HandleRemoveCommandInput()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (!TryGetClosestWaypointIndex(out var path, out var index)) return;
            if (ExistCommandAt(path, index))
            {
                RemoveCommandAt(path, index);
            }
        }
    }
    
    private void BeginPathDrawing(Entity entity)
    {
        hasFirstPoint = false;
        editMode = PathEditMode.DrawingNew;
        
        targetEntity = entity;
        targetEntity.EntityCommandPath.ClearWaypoints();
        targetEntity.EntityCommandPath.EditMode = PathEditMode.DrawingNew;
    }

    private void EndPathDrawing()
    {
        editMode = PathEditMode.None;
        hasFirstPoint = false;
        targetEntity.EntityCommandPath.EditMode = PathEditMode.None;
    }

    private bool TryGetAgent(out Entity entity)
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            entity = hit.collider.GetComponent<Entity>();
            return entity;
        }

        entity = null;
        return false;
    }

    private bool TryGetClosestWaypointIndex(out EntityCommandPath entityCommandPath, out int index)
    {
        entityCommandPath = null;
        index = -1;

        if (!RaycastLayer(walkableLayer, out var hit)) return false;

        var closestSqrDist = float.MaxValue;
        EntityCommandPath closest = null;
        var closestIndex = -1;

        // foreach (var path in squadManager.GetAgents().Select(agent => agent.AgentCommandPath))
        // {
        //     var waypoints = path.Waypoints;
        //     for (int i = 0; i < waypoints.Count; i++)
        //     {
        //         var sqrDist = (waypoints[i].Position - hit.point).sqrMagnitude;
        //         if (sqrDist <= waypointSnapDistance * waypointSnapDistance && sqrDist < closestSqrDist)
        //         {
        //             closestSqrDist = sqrDist;
        //             closest = path;
        //             closestIndex = i;
        //         }
        //     }
        // }

        if (closest)
        {
            entityCommandPath = closest;
            index = closestIndex;
            return true;
        }

        return false;
    }
    
    private void HandleWaypointPlacement(RaycastHit hit)
    {
        bool IsReachable(Vector3 from, Vector3 to, out NavMeshHit navMeshHit)
        {
            var reachable = TryGetSafeNavMeshPosition(from, to, out navMeshHit);
            if (!reachable)
            {
                wasSafeNavMeshPosition = false;
            }
            return reachable;
        }
        
        bool ShouldAddWaypoint(Vector3 lastPos, Vector3 newPos)
        {
            var distance = Vector3.Distance(lastPos, newPos);
            return wasSafeNavMeshPosition
                ? distance >= waypointDistance
                : distance >= waypointDistance && distance <= waypointDistance + waypointDistanceTolerance;
        }

        void AddWaypoint(Vector3 position)
        {
            lastWaypointPosition = position;
            targetEntity.EntityCommandPath.AppendWaypoint(position);
            wasSafeNavMeshPosition = true;
        }

        if (IsReachable(targetEntity.transform.position, hit.point, out var navMeshHit) &&
            HasLineOfSightCapsule(targetEntity.transform.position, navMeshHit.position))
        {
            if (!hasFirstPoint)
            {
                AddWaypoint(targetEntity.transform.position);
                hasFirstPoint = true;
            }

            if (ShouldAddWaypoint(lastWaypointPosition, navMeshHit.position))
            {
                var direction = (navMeshHit.position - lastWaypointPosition).normalized;
                var waypointPosition = lastWaypointPosition + direction * waypointDistance;
                AddWaypoint(waypointPosition);
            }
        }
    }
    
    private bool RaycastLayer(LayerMask layerMask, out RaycastHit hit)
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, 1000f, layerMask);
    }

    private bool HasLineOfSightCapsule(Vector3 from, Vector3 to)
    {
        var direction = to - from;
        var distance = direction.magnitude;
        direction.Normalize();
        
        var agentRadius = targetEntity.NavMeshAgent.radius;
        var agentHeight = targetEntity.NavMeshAgent.height;
        
        var capsuleBottom = from.Add(y:.2f);
        var capsuleTop = capsuleBottom.Add(y: agentHeight - .2f);

        var obstacleLayers = ~agentLayer;
        
        if (Physics.CapsuleCast(capsuleBottom, capsuleTop, agentRadius, direction, out var hit, distance, obstacleLayers,  QueryTriggerInteraction.Ignore))
        {
            if (hit.distance < distance) return false;
        }

        return true;
    }

    private bool TryGetSafeNavMeshPosition(Vector3 agentPosition, Vector3 targetPosition, out NavMeshHit hit)
    {
        var queryFilter = new NavMeshQueryFilter
        {
            agentTypeID = targetEntity.NavMeshAgent.agentTypeID,
            areaMask = targetEntity.NavMeshAgent.areaMask
        };
        
        if (!NavMesh.SamplePosition(targetPosition, out hit, 2f, queryFilter))
        {
            return false;
        }

        var path = new NavMeshPath();
        if (!NavMesh.CalculatePath(agentPosition, hit.position, queryFilter, path))
        {
            return false;
        }
        
        return path.status == NavMeshPathStatus.PathComplete;
    }
    
    private bool ExistCommandAt(EntityCommandPath path, int index) => path.ExistCommandAt(index);

    private void TryAddCommandAt(EntityCommandPath path, int index, ICommand command)
    {
        if (!path.ExistCommandAt(index))
        {
            path.AddCommandAt(index, command);
        }
    }

    private void RemoveCommandAt(EntityCommandPath path, int index) => path.RemoveCommandAt(index);

    public void EnableInput() => inputDisabled = false;
    public void DisableInput() => inputDisabled = true;
}