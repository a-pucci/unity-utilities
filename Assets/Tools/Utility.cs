using UnityEngine;
using UnityEditor;

namespace Tools {
	public static class Utility {

		public static T[] GetAllInstances<T>() where T : ScriptableObject {
			string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
			var a = new T[guids.Length];
			for (int i = 0; i < guids.Length; i++) {
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
			}
			return a;
		}
	}
}