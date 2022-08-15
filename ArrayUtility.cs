using System;
using System.Collections.Generic;
using System.Linq;

namespace AP.Utilities {
	public static class ArrayUtility {
		
		public static void Add<T>(ref T[] array, T item) {
			Array.Resize(ref array, array.Length + 1);
			array[array.Length - 1] = item;
		}

		public static void AddFirst<T>(ref T[] array, T item) {
			var list = new List<T> { item };
			list.AddRange(array);
			array = list.ToArray();
		}

		public static void Remove<T>(ref T[] array, IEnumerable<T> toRemove) {
			var list = new List<T>(array);

			foreach (T element in toRemove) {
				list.Remove(element);
			}

			array = list.ToArray();
		}

		public static void Remove<T>(ref T[] array, T item) {
			var list = new List<T>(array);
			list.Remove(item);
			array = list.ToArray();
		}

		public static void RemoveAt<T>(ref T[] array, int index) {
			var list = new List<T>(array);
			list.RemoveAt(index);
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

		public static void SwapElements<T>(ref T[] array, int from, int to) {
			var list = new List<T>(array);
			(list[from], list[to]) = (list[to], list[from]);
			array = list.ToArray();
		}

		public static void Insert<T>(ref T[] array, int index, T element) {
			var list = new List<T>(array);
			list.Insert(index, element);
			array = list.ToArray();
		}
	}
}