using System.IO;
using UnityEditor;
using UnityEngine;

public static class ARImageScore
{
#if UNITY_EDITOR_OSX
	private const string AugmentedImageCliBinaryName = "augmented_image_cli_osx";
#elif UNITY_EDITOR_WIN
	private const string AugmentedImageCliBinaryName = "augmented_image_cli_win";
#elif UNITY_EDITOR_LINUX
	private const string AugmentedImageCliBinaryName = "augmented_image_cli_linux";
#endif
	
	private static bool FindCliBinaryPath(out string path)
	{
		string[] cliBinaryGuid = AssetDatabase.FindAssets(AugmentedImageCliBinaryName);
		if (cliBinaryGuid.Length == 0)
		{
			Debug.LogErrorFormat(
				"Could not find required tool for building AugmentedImageDatabase: {0}. " +
				"Was it removed from the ARCore SDK?", AugmentedImageCliBinaryName);
			path = string.Empty;
			return false;
		}

		// Remove the '/Assets' from the project path since it will be added in the path below.
		string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
		path = Path.Combine(projectPath, AssetDatabase.GUIDToAssetPath(cliBinaryGuid[0]));
		return !string.IsNullOrEmpty(path);
	}
	
	public static string CalculateScore(Texture2D image)
	{
		if (image == null || !FindCliBinaryPath(out string path))
		{
			Debug.LogError("Image null or not found Cli Binary");
			return "ERROR";
		}

		string imagePath = AssetDatabase.GetAssetPath(image);
		ShellHelper.RunCommand(
			path,
			$"eval-img --input_image_path \"{imagePath}\"",
			out string output,
			out string error);
		
		if (!string.IsNullOrEmpty(error))
		{
			Debug.LogError(error);
			output = "ERROR";
		}

		return output;
	}
}
