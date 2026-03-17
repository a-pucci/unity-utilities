using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using GRS = AP.Utilities.UI.GameResolutionSwitcher;

namespace AP.Utilities.UI.Editor
{
	public class GameResolutionSwitcherWindow : EditorWindow
	{
		[MenuItem("Tools/UI/Game Resolution Switcher")]
		public static void Open()
		{
			var instance = GetWindow<GameResolutionSwitcherWindow>("Game Resolution Switcher");
			instance.grs = CreatePrefab("Game Resolution Switcher");
		}

		private static GRS CreatePrefab(string prefabName)
		{
			var existingInstance = FindObjectOfType<GRS>();

			if (existingInstance)
			{
				Selection.activeObject = existingInstance.gameObject;
				return existingInstance;
			}

			string[] guids = AssetDatabase.FindAssets(prefabName + " t:Prefab");

			if (guids.Length == 0)
			{
				new GameObject(prefabName).AddComponent<GRS>();
				Debug.LogError($"Prefab '{prefabName}' not found. Creating new object.");
				return null;
			}

			string path = AssetDatabase.GUIDToAssetPath(guids[0]);
			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

			if (prefab == null)
			{
				Debug.LogError("Impossibile caricare il prefab.");
				return null;
			}

			Object instance = PrefabUtility.InstantiatePrefab(prefab);

			Undo.RegisterCreatedObjectUndo(instance, "Create " + prefab.name);

			Selection.activeObject = instance;
			return ((GameObject)instance).GetComponent<GRS>();
		}

		private const float ColorDesaturation = 0.5f;

		public GRS grs;

		private bool showHelpBox = false;

		private void OnGUI()
		{
			GUILayout.Space(8);
			showHelpBox = EditorGUILayout.Foldout(showHelpBox, "Help Information");

			if (showHelpBox)
			{
				GUILayout.Label("Tool per aiutare a cambiare velocemente risoluzione della GameView.\n" +
				                "Premendo uno dei bottoni verrà cambiata risoluzione, attivata la reference collegata " +
				                "e disabilitate tutte le altre reference.");
			}

			GUILayout.Space(8);

			if (!grs)
			{
				EditorGUILayout.LabelField("Missing Prefab", GUILayout.ExpandWidth(true));
				if (GUILayout.Button("Find or Create Prefab"))
					grs = CreatePrefab("Game Resolution Switcher");

				return;
			}

			DrawData("Portrait", grs.Portrait, new Color(ColorDesaturation, 1, ColorDesaturation));
			GUILayout.Space(6);
			DrawHorizontalLine(4);
			GUILayout.Space(6);
			DrawData("Landscape", grs.Landscape, new Color(1, ColorDesaturation, ColorDesaturation));
		}

		private void DrawData(string dataTitle, GRS.GameResData[] data, Color color)
		{
			var prevColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			using var _ = new EditorGUILayout.VerticalScope(GUI.skin.box);
			DrawTitle(dataTitle);
			DrawButtons(data);
			GUI.backgroundColor = prevColor;
		}

		private void DrawButtons(GRS.GameResData[] data)
		{
			DrawHorizontalLine(2);
			if (data == null || data.Length == 0)
				return;

			foreach (GRS.GameResData gameResData in data)
			{
				Vector2Int resSize = gameResData.ResolutionSize;
				string resName = gameResData.ResolutionName;

				if (!GUILayout.Button($"{gameResData.ResolutionName} ({resSize.x}, {resSize.y})"))
					continue;

				Array.ForEach(grs.Portrait, d =>
				{
					if (d.Reference)
						d.Reference.SetActive(false);
				});

				Array.ForEach(grs.Landscape, d =>
				{
					if (d.Reference)
						d.Reference.SetActive(false);
				});

				if (gameResData.Reference)
					gameResData.Reference.SetActive(true);

				GameViewUtils.SetSize(GameViewUtils.AddCustomSizeIfMissing(resSize.x, resSize.y, resName));
			}
		}

		private void DrawHorizontalLine(int height = 1)
		{
			GUILayout.Box(GUIContent.none, GUILayout.Height(height), GUILayout.ExpandWidth(true));
		}

		private void DrawTitle(string text)
		{
			EditorGUILayout.LabelField(text, GUILayout.ExpandWidth(true));
		}
	}
}