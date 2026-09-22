using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveLoadSystem))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var saveLoadSystem = (SaveLoadSystem) target;

        DrawDefaultInspector();

        if (GUILayout.Button("New Game"))
        {
            saveLoadSystem.NewGame();
        }

        if (GUILayout.Button("Save Game"))
        {
            saveLoadSystem.SaveGame();
        }

        if (GUILayout.Button("Load Game"))
        {
            saveLoadSystem.ReloadGame();
        }

        if (GUILayout.Button("Delete Game"))
        {
            saveLoadSystem.DeleteGame();
        }
    }
}
