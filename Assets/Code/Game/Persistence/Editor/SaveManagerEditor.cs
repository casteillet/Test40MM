using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveLoadSystem))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var saveLoadSystem = (SaveLoadSystem) target;
        var gameName = saveLoadSystem.gameData.Name;
        
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
            saveLoadSystem.LoadGame(gameName);
        }

        if (GUILayout.Button("Bind Datas"))
        {
            saveLoadSystem.BindDatas();
        }

        if (GUILayout.Button("Delete Game"))
        {
            saveLoadSystem.DeleteGame(gameName);
        }
    }
}