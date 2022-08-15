using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Random = UnityEngine.Random;

namespace AP.Utilities {
	public static class Utility {
		///Random Weighted elements
		///https://forum.unity.com/threads/random-numbers-with-a-weighted-chance.442190/
		public static int GetRandomWeightedIndex(float[] weights) {
			if (weights == null || weights.Length == 0) {
				return -1;
			}
			float totalWeight = 0;

			for (int i = 0; i < weights.Length; i++) {
				if (float.IsPositiveInfinity(weights[i])) 
					return i;
				else if (weights[i] >= 0f && !float.IsNaN(weights[i])) 
					totalWeight += weights[i];
			}

			float randomPick = Random.value;
			float s = 0f;

			for (int i = 0; i < weights.Length; i++) {
				float currentWeight = weights[i];
				if (float.IsNaN(currentWeight) || currentWeight <= 0f) {
					continue;
				}
				s += currentWeight / totalWeight;
				if (s >= randomPick) {
					return i;
				}
			}

			return -1;
		}
		
		/// Get all element names of th Enum
		public static List<string> GetEnumNames<T>() {
			return !typeof(T).IsEnum ? null : Enum.GetNames(typeof(T)).ToList();
		}

	}
}