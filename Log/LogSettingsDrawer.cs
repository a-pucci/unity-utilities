#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AP.Logging
{
    [CustomEditor(typeof(LogSettings), true)]
    public partial class LogSettingsDrawer : Editor
    {
        private SerializedProperty enableLogsProp;
        private SerializedProperty logLevelProp;
        private SerializedProperty loggersProp;

        private static float HalfLine => EditorGUIUtility.singleLineHeight / 2;

        private LogSettings logSettings;
        private LogSettings LogSettings => logSettings ??= (LogSettings)target;
        private bool isRefreshing;

        private bool useDark;
        private Color darkGray = new Color(0.25f, 0.25f, 0.25f, 1f);
        private Color lightGray = new Color(0.3f, 0.3f, 0.3f, 1f);

        private void OnEnable()
        {
            useDark = true;
            enableLogsProp = serializedObject.FindProperty("enableLogs");
            logLevelProp = serializedObject.FindProperty("logLevel");
            loggersProp = serializedObject.FindProperty("loggers");

            InitCache();
        }

        private void DrawLineColor()
        {
            useDark = !useDark;
            Color lineColor = useDark ? darkGray : lightGray;
            Rect rect = EditorGUILayout.GetControlRect(false, 0);
            rect.y += 1;
            rect.height = EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.DrawRect(rect, lineColor);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            useDark = false;

            if (LogSettings == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.BeginHorizontal();

            // if (GUILayout.Button("Refresh") && !isRefreshing)
            //     RefreshAndRebuildCache();

            if (GUILayout.Button("Refresh") && !isRefreshing)
                BuildCache(LogSettings);

            EditorGUILayout.EndHorizontal();

            if (isRefreshing)
            {
                EditorGUILayout.HelpBox("Refreshing logger types... Please wait.", MessageType.Info);
                return;
            }
            
            GUILayout.Space(HalfLine);

            EditorGUILayout.PropertyField(enableLogsProp);
            EditorGUILayout.PropertyField(logLevelProp);

            GUILayout.Space(HalfLine);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Select All", GUILayout.Width(80)))
            {
                LogSettings.SelectAll();
                EditorUtility.SetDirty(LogSettings);
                serializedObject.Update();
            }

            if (GUILayout.Button("Deselect All", GUILayout.Width(100)))
            {
                LogSettings.DeselectAll();
                EditorUtility.SetDirty(LogSettings);
                serializedObject.Update();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(HalfLine);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;

            if (!cache.HasData)
            {
                if (GUILayout.Button("Build Cache Now"))
                    BuildCache(LogSettings);

                return;
            }

            foreach (TypeNode node in cache.assemblies.Values)
            {
                DrawNodeRecursive(node, LogSettings);
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawNodeRecursive(TypeNode node, LogSettings settings)
        {
            // Node with children
            if (node.children.Count > 0)
            {
                DrawLineColor();
                EditorGUILayout.BeginHorizontal();

                node.foldout = EditorGUILayout.Foldout(node.foldout, node.name, true);

                if (node.loggerIndex >= 0 && node.loggerIndex < loggersProp.arraySize)
                {
                    SerializedProperty element = loggersProp.GetArrayElementAtIndex(node.loggerIndex);
                    SerializedProperty enabledProp = element.FindPropertyRelative("isEnabled");

                    bool newVal = GUILayout.Toggle(enabledProp.boolValue, GUIContent.none, GUILayout.Width(18));

                    if (newVal != enabledProp.boolValue)
                    {
                        enabledProp.boolValue = newVal;
                        EditorUtility.SetDirty(settings);
                    }
                }

                EditorGUILayout.EndHorizontal();

                if (node.foldout)
                {
                    EditorGUI.indentLevel++;

                    // Recursively draw children
                    foreach (KeyValuePair<string, TypeNode> childKvp in node.children)
                    {
                        DrawNodeRecursive(childKvp.Value, settings);
                    }

                    EditorGUI.indentLevel--;
                }

                return;
            }

            // Leaf node
            if (node.loggerIndex >= 0 && node.loggerIndex < loggersProp.arraySize)
            {
                EditorGUI.indentLevel++;
                DrawLeaf(settings, node.loggerIndex, node.name);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawLeaf(LogSettings settings, int idx, string label)
        {
            SerializedProperty element = loggersProp.GetArrayElementAtIndex(idx);
            SerializedProperty enabledProp = element.FindPropertyRelative("isEnabled");

            DrawLineColor();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.ExpandWidth(true));
            bool newVal = GUILayout.Toggle(enabledProp.boolValue, GUIContent.none, GUILayout.Width(18));

            if (newVal != enabledProp.boolValue)
            {
                enabledProp.boolValue = newVal;
                EditorUtility.SetDirty(settings);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif