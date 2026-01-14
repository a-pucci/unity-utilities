#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AP.Logging
{
	public partial class LogSettingsDrawer
	{
        private static LogCache cache;

        private void InitCache()
        {
            cache = new LogCache();
            BuildCache(LogSettings);
        }
        
        private async void RefreshAndRebuildCache()
        {
            await LogSettings.RefreshAsync();
            BuildCache(LogSettings);
            serializedObject.ApplyModifiedProperties();
        }
        
        private void BuildCache(LogSettings settings)
        {
            cache.Clear();
            if (settings.Loggers == null)
                return;

            for (int i = 0; i < settings.Loggers.Count; i++)
            {
                LoggerSetting logger = settings.Loggers[i];
                string assembly = string.IsNullOrEmpty(logger.assemblyName) ? "<NoAssembly>" : logger.assemblyName;

                if (!cache.assemblies.TryGetValue(assembly, out TypeNode root))
                {
                    root = new TypeNode(assembly);
                    cache.assemblies.Add(assembly, root);
                }

                string[] parts = string.IsNullOrEmpty(logger.typeName) ? new[] { "<Unknown>" } : logger.typeName.Split('+');
                TypeNode current = root;

                foreach (string part in parts)
                {
                    if (!current.children.TryGetValue(part, out TypeNode child))
                    {
                        child = new TypeNode(part);
                        current.children.Add(part, child);
                    }

                    current = child;
                }

                current.loggerIndex = i;
            }
            // LogCacheTree();
            serializedObject.Update();
        }

        private void LogCacheTree()
        {
            if (cache is not { HasData: true })
            {
                Debug.Log("LogCache: empty");
                return;
            }

            var sb = new StringBuilder();

            void PrintNode(TypeNode node, int indent)
            {
                sb.Append(new string(' ', indent * 2));
                sb.Append("- ");
                sb.Append(node.name);
                if (node.loggerIndex >= 0)
                    sb.Append($" [{node.loggerIndex}]");

                sb.AppendLine();
                foreach (TypeNode child in node.children.Values)
                    PrintNode(child, indent + 1);
            }

            foreach (KeyValuePair<string, TypeNode> kvp in cache.assemblies)
            {
                sb.AppendLine($"Assembly: {kvp.Key}");
                PrintNode(kvp.Value, 1);
            }

            Debug.Log(sb.ToString());
        }

        private class LogCache
        {
            public readonly Dictionary<string, TypeNode> assemblies = new();
            public bool HasData => assemblies.Count > 0;

            public void Clear() => assemblies.Clear();
        }

        private class TypeNode
        {
            public readonly string name;
            public bool foldout;
            public readonly Dictionary<string, TypeNode> children = new();
            public int loggerIndex = -1;

            public TypeNode(string name)
            {
                this.name = name;
                foldout = true;
            }
        }
	}
}
#endif