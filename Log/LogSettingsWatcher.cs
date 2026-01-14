#if UNITY_EDITOR
using UnityEditor;

namespace AP.Logging
{
	[InitializeOnLoad]
	internal static class LogSettingsWatcher
	{
		static LogSettingsWatcher() => AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

		private static void OnAfterAssemblyReload()
		{
			string[] guids = AssetDatabase.FindAssets("t:LogSettings");
			foreach (string g in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(g);
				var asset = AssetDatabase.LoadAssetAtPath<LogSettings>(path);
				if (asset != null)
				{
					_ = asset.RefreshAsync();
				}
			}
		}
	}
}
#endif