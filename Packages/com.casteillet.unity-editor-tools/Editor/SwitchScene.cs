using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class SceneSwitcherToolbar
{
    private const string ElementPath = "Scenes/Switcher";

    private const int DockIndex = -1;

    static SceneSwitcherToolbar()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
    }

    private static void OnActiveSceneChanged(Scene previous, Scene current)
    {
        MainToolbar.Refresh(ElementPath);
    }
    
    [MainToolbarElement(ElementPath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = DockIndex)]
    private static MainToolbarElement CreateSceneSwitcher()
    {
        var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
        MainToolbarContent content = new(SceneManager.GetActiveScene().name, icon, "");
        return new MainToolbarDropdown(content, ShowSwitcherPopup);
    }

    private static void ShowSwitcherPopup(Rect activatorRect)
    {
        if (EditorBuildSettings.scenes.Length == 0)
        {
            Debug.LogWarning("No scene found in Build Settings");
            return;
        }

        if (!HasValidBuildScene())
        {
            Debug.LogWarning("No valid scene found in Build Settings, please check if the scenes exist");
            return;
        }

        PopupWindow.Show(activatorRect, new PopupSwitchScene());
    }

    private static bool HasValidBuildScene()
    {
        foreach (EditorBuildSettingsScene editorScene in EditorBuildSettings.scenes)
        {
            if (SwitchScene.SceneIsValid(editorScene))
                return true;
        }

        return false;
    }
}

[InitializeOnLoad]
public static class SwitchScene
{
    #region Variables
    private static string editorStartupPathScene;

    public static string EditorStartupPathScene
    {
        get => editorStartupPathScene;
        set
        {
            editorStartupPathScene = value;
            EditorPrefs.SetString("EditorStartupPathScene", value);
        }
    }
    #endregion

    static SwitchScene()
    {
        InitializePlayModeStartScene();
    }

    private static void InitializePlayModeStartScene()
    {
        editorStartupPathScene = EditorPrefs.GetString("EditorStartupPathScene");
        
        if (!string.IsNullOrEmpty(editorStartupPathScene))
        {
            SetPlayModeStartScene(editorStartupPathScene);
        }
    }

    public static void SetPlayModeStartScene(string scenePath)
    {
        EditorSceneManager.playModeStartScene = string.IsNullOrEmpty(scenePath)
            ? null
            : AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
    }

    public static void OpenScene(string path)
    {
        if (Application.isPlaying)
        {
            var sceneName = Path.GetFileNameWithoutExtension(path);
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError($"Scene '{sceneName}' is not in the Build Settings.");
            }
        }
        else
        {
            if (File.Exists(path))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(path);
                }
            }
            else
            {
                Debug.LogError($"Scene at path '{path}' does not exist.");
            }
        }
    }

    public static bool SceneIsValid(EditorBuildSettingsScene editorScene)
    {
        if (string.IsNullOrEmpty(editorScene.path))
            return false;

        return AssetDatabase.LoadAssetAtPath<SceneAsset>(editorScene.path);
    }
}

public class PopupSwitchScene : PopupWindowContent
{
    #region Variables
    private Dictionary<string, List<EditorBuildSettingsScene>> editorScenes = new();

    private Vector2 scrollPos;
    private readonly float popupWidth = 250f;
    private float popupHeight = 132f;
    private readonly float scrollViewHeight = 130f;
    private readonly float headerHeight = 20f;
    private readonly float buttonHeight = 20f;
    private float scrollViewContentHeight;
    private float marginRight;

    private static readonly GUIStyle headerLabelStyle;
    private static readonly GUIStyle headerButtonStyle;
    private static readonly GUIStyle buttonStyle;
    #endregion

    static PopupSwitchScene()
    {
        headerLabelStyle = new(GUI.skin.label)
        {
            fontStyle = FontStyle.Bold,
            margin = new RectOffset()
        };

        headerButtonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = { background = MakeTex(30, 20, new Color(0, 0, 0, 0)) },
            margin = new RectOffset(0, 0, 0, 0)
        };

        buttonStyle = new(GUI.skin.button);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        Initialize();
    }

    private void Initialize()
    {
        editorScenes.Clear();
        foreach (EditorBuildSettingsScene editorScene in EditorBuildSettings.scenes)
        {
            if (!SwitchScene.SceneIsValid(editorScene)) continue;

            string directoryPath = Path.GetDirectoryName(editorScene.path);

            if (!editorScenes.ContainsKey(directoryPath))
            {
                editorScenes[directoryPath] = new List<EditorBuildSettingsScene> { editorScene };
                scrollViewContentHeight += (headerHeight + 2f) + (buttonHeight + 2f);
            }
            else
            {
                if (!editorScenes[directoryPath].Contains(editorScene))
                {
                    editorScenes[directoryPath].Add(editorScene);
                    scrollViewContentHeight += buttonHeight + 2f;
                }
            }
        }
    }

    #region Rendering
    public override Vector2 GetWindowSize()
    {
        if (EditorBuildSettings.scenes.Length > 0)
        {
            if (scrollViewContentHeight + 15f > scrollViewHeight)
            {
                marginRight = -11f;
            }
            else
            {
                marginRight = 2f;
                popupHeight = scrollViewContentHeight + 20f;
            }
        }

        return new Vector2(popupWidth, popupHeight);
    }

    private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color32[] pix = new Color32[width * height];
        for (int i = 0; i < pix.Length; ++i)
            pix[i] = col;

        Texture2D result = new(width, height);
        result.SetPixels32(pix);
        result.Apply();
        return result;
    }
    #endregion

    #region GUI
    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginVertical();

        if (EditorBuildSettings.scenes.Length > 0)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(scrollViewHeight));

            foreach (KeyValuePair<string, List<EditorBuildSettingsScene>> editorScene in editorScenes)
            {
                HeaderGUI(editorScene);
                SceneListGUI(editorScene);
            }

            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No scene found in Build Settings");
        }

        GUILayout.EndVertical();
    }

    private void HeaderGUI(KeyValuePair<string, List<EditorBuildSettingsScene>> editorScene)
    {
        GUILayout.BeginHorizontal(GUI.skin.box);

        if (!string.IsNullOrEmpty(editorScene.Key))
        {
            EditorGUILayout.LabelField(editorScene.Key, headerLabelStyle, GUILayout.Width(popupWidth - 45 + marginRight));

            if (GUILayout.Button(EditorGUIUtility.IconContent("Search On Icon"), headerButtonStyle, GUILayout.Width(26), GUILayout.Height(headerHeight - 2)))
            {
                if (editorScene.Value[0] != null)
                {
                    string scenePath = editorScene.Value[0].path;
                    string directoryPath = Path.GetDirectoryName(scenePath);
                    FocusDirectory(directoryPath);
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("Assets", headerLabelStyle, GUILayout.Width(popupWidth - 16 + marginRight));
        }

        GUILayout.EndHorizontal();
    }

    private void SceneListGUI(KeyValuePair<string, List<EditorBuildSettingsScene>> editorScene)
    {
        foreach (EditorBuildSettingsScene scene in editorScene.Value)
        {
            bool sceneIsStartup = SwitchScene.EditorStartupPathScene == scene.path;
            string sceneName = Path.GetFileNameWithoutExtension(scene.path);

            GUILayout.BeginHorizontal();

            bool toogleState = GUILayout.Toggle(sceneIsStartup, EditorGUIUtility.IconContent("PlayButton"), GUI.skin.button, GUILayout.Width(20), GUILayout.Height(buttonHeight));
            if (toogleState)
            {
                SwitchScene.EditorStartupPathScene = scene.path;
                SwitchScene.SetPlayModeStartScene(scene.path);
            }
            else
            {
                if (SwitchScene.EditorStartupPathScene == scene.path)
                {
                    SwitchScene.EditorStartupPathScene = null;
                    SwitchScene.SetPlayModeStartScene(null);
                }
            }

            if (scene.enabled)
            {
                if (GUILayout.Button(sceneName, GUILayout.Width(popupWidth - 32 + marginRight), GUILayout.Height(buttonHeight)))
                    SwitchScene.OpenScene(scene.path);
            }
            else
            {
                buttonStyle.normal.textColor = new Color(1f, 1f, 1f, .5f);
                buttonStyle.hover.textColor = new Color(1f, 1f, 1f, .5f);

                if (GUILayout.Button(sceneName, buttonStyle, GUILayout.Width(popupWidth - 32 + marginRight), GUILayout.Height(buttonHeight)))
                    SwitchScene.OpenScene(scene.path);
            }

            GUILayout.EndHorizontal();
        }
    }
    #endregion

    private void FocusDirectory(string path)
    {
        EditorUtility.FocusProjectWindow();
        Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);
    }
}