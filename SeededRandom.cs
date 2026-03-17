using System;
using System.Collections.Generic;

namespace AP.Utilities
{
	public static class SeededRandom
	{
		private static Random random;
		private static readonly object Lock = new();
		private static int seed;
		
		public static void Initialize(int newSeed)
		{
			lock (Lock)
			{
				seed = newSeed;
				random = new Random(seed);
			}
		}

		public static int Range(int minInclusive, int maxExclusive)
		{
			lock (Lock)
			{
				return random.Next(minInclusive, maxExclusive);
			}
		}
       
		public static float Range(float minInclusive, float maxExclusive)
		{
			lock (Lock)
			{
				return (float)random.NextDouble() * (maxExclusive - minInclusive) + minInclusive;
			}
		}
		
		public static int WeightedIndex(float[] weights)
		{
			lock (Lock)
			{
				if (weights == null || weights.Length == 0)
					return -1;

				float totalWeights = 0f;
        
				for (int i = 0; i < weights.Length; i++)
				{
					float weight = weights[i];
            
					if (float.IsPositiveInfinity(weight))
						return i;
            
					if (weight > 0f && !float.IsNaN(weight))
						totalWeights += weight;
				}
        
				if (totalWeights <= 0f)
					return -1;
        
				float rnd = (float)random.NextDouble() * totalWeights;
				float sum = 0f;
        
				for (int i = 0; i < weights.Length; i++)
				{
					float weight = weights[i];
            
					if (float.IsNaN(weight) || weight <= 0f)
						continue;
            
					sum += weight;
            
					if (rnd < sum)
						return i;
				}
        
				for (int i = weights.Length - 1; i >= 0; i--)
				{
					if (weights[i] > 0f && !float.IsNaN(weights[i]))
						return i;
				}
        
				return -1;
			}
		}
		
		#region Utility
		
		public static void Shuffle<T>(IList<T> list)
		{
			lock (Lock)
			{
				int n = list.Count;
				while (n > 1)
				{
					n--;
					int k = random.Next(n + 1);
					(list[k], list[n]) = (list[n], list[k]);
				}
			}
		}
		
		public static IList<T> ShuffledCopy<T>(IEnumerable<T> list)
		{
			var shuffledList = new List<T>(list);
			Shuffle(shuffledList);
			return shuffledList;
		}
		
		public static T GetRandom<T>(IReadOnlyList<T> list)
		{
			return list.Count == 0 ? default : list[Range(0, list.Count)];
		}
	
		public static IList<T> GetRandom<T>(IReadOnlyList<T> list, int count)
		{
			if (list == null) throw new ArgumentNullException(nameof(list));
			if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must be > 0");
			if (count > list.Count) throw new ArgumentException("Not enough unique elements in source to satisfy count", nameof(count));

			var copy = new List<T>(list);
			Shuffle(copy);
			return copy.GetRange(0, count);
		}

		public static IList<T> GetRandom<T>(IEnumerable<T> list, int count, IEnumerable<T> exclude)
		{
			if (list == null) throw new ArgumentNullException(nameof(list));
			var l = new List<T>(list);
			l.Remove(exclude);
			return GetRandom(l, count);
		}
		
		public static void Remove<T>(this IList<T> list, IEnumerable<T> toRemove)
		{
			foreach (T element in toRemove)
			{
				list.Remove(element);
			}
		}
		
		#endregion
	}
}