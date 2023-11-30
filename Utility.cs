using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
		
		public static async Task WaitUntil(Func<bool> predicate)
		{
			int sleep = (int)(Time.deltaTime * 1000);
			while (!predicate())
			{
				await Task.Delay(sleep);
			}
		}
		
		public static async Task WaitWhile(Func<bool> predicate)
		{
			int sleep = (int)(Time.deltaTime * 1000);
			while (predicate())
			{
				await Task.Delay(sleep);
			}
		}

   		public static Bounds CalculateBounds(GameObject go)
    	{
       	 	var b = new Bounds(go.transform.position, Vector3.zero);
        	Renderer[] rList = go.GetComponentsInChildren<Renderer>();
        	foreach (Renderer renderer in rList)
            	b.Encapsulate(renderer.bounds);

        	return b;
    	}

    	public static short EncodeDouble(double value)
    	{
        	// 52.1 = 521 * 10 ^ -1 => 0x1521
        	// 1.25 = 125 * 10 ^ -2 => 0x2125
        	// range from 0.0000000000000001 (0xf001) to 999 (0x0999)

        	int cnt = 0;
        	while (value != Math.Floor(value))
        	{
            	value *= 10.0;
            	cnt++;
        	}
        	return (short)((cnt << 12) + (int)value);
    	}

    	public static double DecodeDouble(short value)
    	{
        	int cnt = value >> 12;
        	double result = value & 0xfff;
        	while (cnt > 0)
        	{
            	result /= 10.0;
            	cnt--;
        	}
        	return result;
    	}

    	/// Returns a sine smoothed value 0-1
    	public static float SmoothProgress(float progress)
    	{
        	progress = Mathf.Clamp01(progress);
        	float halfPI = Mathf.PI / 2;
        	// maps the progress between -PI/2 to PI/2
        	progress = Mathf.Lerp(-halfPI, halfPI, progress);

        	// returns value between -1 and 1
        	progress = Mathf.Sin(progress);

        	// scale the sin value between 0 and 1
        	progress = (progress / 2) + 0.5f;
        	return progress;
    	}
	}
}
