using UnityEngine;
using UnityEditor;

namespace Utilities {
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
		
		public static int GetRandomWeightedIndex(float[] weights) {
			if (weights == null || weights.Length == 0) return -1;

			float w;
			float total = 0;
			int i;
			for (i = 0; i < weights.Length; i++) {
				w = weights[i];
				if (float.IsPositiveInfinity(w)) return i;
				else if (w >= 0f && !float.IsNaN(w)) total += weights[i];
			}

			float r = Random.value;
			float s = 0f;

			for (i = 0; i < weights.Length; i++) {
				w = weights[i];
				if (float.IsNaN(w) || w <= 0f) continue;

				s += w / total;
				if (s >= r) return i;
			}

			return -1;
		}
		
	}
}