using Code.Scripts.StateMachine;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AgentStateMachine))]
public class AgentStateMachineEditor : Editor
{
    private AgentStateMachine stateMachine;
    
    private void OnEnable()
    {
        stateMachine = (AgentStateMachine)target;
    }

    public override void OnInspectorGUI()
    {
        if (!Application.isPlaying)
        {
            base.OnInspectorGUI();
            return;
        }
        
        GUILayout.Space(10f);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("Command");
        
        EditorGUILayout.BeginVertical();
        
        var commandInvoker = stateMachine.CommandInvoker;
        if (commandInvoker.CurrentCommand != null)
        {
            EditorGUILayout.LabelField(commandInvoker.CurrentCommand.ToString());
            if (commandInvoker.CurrentCommand is MovePathCommand command && command.InternalInvoker.CurrentCommand != null)
            {
                EditorGUILayout.LabelField(command.InternalInvoker.CurrentCommand.ToString());
            }
        }
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);

        base.OnInspectorGUI();
    }

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawHandles(AgentStateMachine stateMachine, GizmoType gizmoType)
    {
        if (!Application.isPlaying) return;
        
        var text = "";
        if (stateMachine.CommandInvoker.CurrentCommand != null)
        {
            text = $"{stateMachine.CommandInvoker.CurrentCommand}\n";
            if (stateMachine.CommandInvoker.CurrentCommand is MovePathCommand command && command.InternalInvoker.CurrentCommand != null)
            {
                text += $"{command.InternalInvoker.CurrentCommand}\n";
            }
        }

        var statusStyle = GUI.skin.label;
        statusStyle.normal.textColor = Color.white;
        statusStyle.alignment = TextAnchor.MiddleCenter;
        statusStyle.fontSize = 14;
        
        Handles.Label(stateMachine.transform.position, text, statusStyle);
    }
}