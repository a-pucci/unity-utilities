using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities {
	public static class ArrayUtility {
		public static void Add<T>(ref T[] array, T item) {
			Array.Resize(ref array, array.Length + 1);
			array[array.Length - 1] = item;
		}

		public static void AddFirst<T>(ref T[] array, T item) {
			List<T> reverse = array.Reverse().ToList();
			reverse.Add(item);
			reverse.Reverse();
			array = reverse.ToArray();
		}

		public static void Remove<T>(ref T[] array, T item) {
			var list = new List<T>(array);
			list.Remove(item);
			array = list.ToArray();
		}

		public static bool FindAndRemove<T>(ref T[] array, Predicate<T> match) {
			var list = new List<T>(array);
			T element = list.Find(match);
			if (element != null && list.Remove(element)) {
				array = list.ToArray();
				return true;
			}
			return false;
		}

		public static bool FindAndRemove<T>(ref T[] array, T element) {
			var list = new List<T>(array);
			if (list.Remove(element)) {
				array = list.ToArray();
				return true;
			}
			return false;
		}
	}
}