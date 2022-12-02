using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AP.Utilities
{
	public static class Utility
	{
		///Random Weighted elements
		///https://forum.unity.com/threads/random-numbers-with-a-weighted-chance.442190/
		public static int GetRandomWeightedIndex(float[] weights)
		{
			if (weights == null || weights.Length == 0)
				return -1;
			
			float totalWeight = 0;

			for (int i = 0; i < weights.Length; i++)
			{
				if (float.IsPositiveInfinity(weights[i]))
					return i;
				else if (weights[i] >= 0f && !float.IsNaN(weights[i]))
					totalWeight += weights[i];
			}

			float randomPick = Random.value;
			float s = 0f;

			for (int i = 0; i < weights.Length; i++)
			{
				float currentWeight = weights[i];

				if (float.IsNaN(currentWeight) || currentWeight <= 0f)
					continue;
				
				s += currentWeight / totalWeight;

				if (s >= randomPick)
					return i;
			}

			return -1;
		}

		/// Get all element names of th Enum
		public static List<string> GetEnumNames<T>() => !typeof(T).IsEnum ? null : Enum.GetNames(typeof(T)).ToList();

		private static readonly Dictionary<float, WaitForSeconds> WaitsDictionary = new Dictionary<float, WaitForSeconds>();

		public static WaitForSeconds GetWait(float time)
		{
			if (WaitsDictionary.TryGetValue(time, out WaitForSeconds wait))
				return wait;

			WaitsDictionary.Add(time, new WaitForSeconds(time));
			return WaitsDictionary[time];
		}

		private static readonly Dictionary<float, WaitForSecondsRealtime> RealtimeWaitsDictionary = new Dictionary<float, WaitForSecondsRealtime>();

		public static WaitForSecondsRealtime GetWaitRealtime(float time)
		{
			if (RealtimeWaitsDictionary.TryGetValue(time, out WaitForSecondsRealtime wait))
				return wait;

			RealtimeWaitsDictionary.Add(time, new WaitForSecondsRealtime(time));
			return RealtimeWaitsDictionary[time];
		}
	}
}