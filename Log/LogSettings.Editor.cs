#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;

namespace AP.Logging
{
	public partial class LogSettings
	{
		public void SelectAll()
		{
			for (int i = 0; i < loggers.Count; i++)
			{
				LoggerSetting ls = loggers[i];
				ls.isEnabled = true;
				loggers[i] = ls;
			}
		}

		public void DeselectAll()
		{
			for (int i = 0; i < loggers.Count; i++)
			{
				LoggerSetting ls = loggers[i];
				ls.isEnabled = false;
				loggers[i] = ls;
			}
		}
		
		public async Task RefreshAsync()
		{
			Dictionary<string, bool> existingSettings = loggers.ToDictionary(ls => ls.LoggerName, ls => ls.isEnabled);
			
			// Execute in background
			List<LoggerSetting> newList = await Task.Run(() =>
			{
				List<Type> tempLoggers = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(a =>
					{
						try { return a.GetTypes(); }
						catch { return Type.EmptyTypes; }
					})
					.Where(t => typeof(ILogger).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
					.ToList();

				var result = new List<LoggerSetting>();

				foreach (Type loggerType in tempLoggers)
				{
					string typeName = loggerType.FullName;
					string assemblyName = loggerType.Assembly.GetName().Name;
					var ls = new LoggerSetting { typeName = typeName, assemblyName = assemblyName, isEnabled = true };
					bool isEnabled = existingSettings.GetValueOrDefault(ls.LoggerName, true);
					ls.isEnabled = isEnabled;
					result.Add(ls);
				}

				return result;
			});

			// Apply result in main thread
			EditorApplication.delayCall += ApplyResult;
			return;

			void ApplyResult()
			{
				EditorApplication.delayCall -= ApplyResult;
				loggers = newList;
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
		}
	}
}
#endif