using System.Reflection;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class ClearConsole
{
	private const string ElementPath = "Console/ClearConsole";
	
	private const int DockIndex = 1;
	
	static bool m_enabled;

	static bool Enabled
	{
		get { return m_enabled; }
		set
		{
			m_enabled = value;
			EditorPrefs.SetBool("ClearConsoleOnSceneChanged", value);
		}
	}

	static ClearConsole()
	{
		m_enabled = EditorPrefs.GetBool("ClearConsoleOnSceneChanged", false);
		EditorApplication.playModeStateChanged += OnPlayModeChanged;
	}
	
	[MainToolbarElement(ElementPath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = DockIndex)]
	public static MainToolbarElement CreateClearConsole()
	{
		var icon = EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow").image as Texture2D;
		var content = new MainToolbarContent("Clear Console", icon, "");
		return new MainToolbarToggle(content, Enabled, ToggleClearConsole);
	}

	static void OnPlayModeChanged(PlayModeStateChange playModeState)
	{
		if (playModeState == PlayModeStateChange.EnteredPlayMode)
		{
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
		}
		else if (playModeState == PlayModeStateChange.ExitingPlayMode)
		{
			SceneManager.activeSceneChanged -= OnActiveSceneChanged;
		}
	}

	private static void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
	{
		ToggleClearConsole(Enabled);
	}

	private static void ToggleClearConsole(bool value)
	{
		Enabled = value;
		
		if (Enabled)
		{
			ClearLog();
		}
	}

	static void ClearLog()
	{
		var assembly = Assembly.GetAssembly(typeof(Editor));
		var type = assembly.GetType("UnityEditor.LogEntries");
		var method = type.GetMethod("Clear");
		method?.Invoke(new object(), null);
	}
}