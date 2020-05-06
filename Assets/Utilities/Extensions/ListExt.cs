using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Extensions {
	public static class ListExt {	public static T GetRandomElement<T>(this List<T> list) {
			return list[Random.Range(0, list.Count)];
		}

		public static T First<T>(this List<T> list) {
			return list[0];
		}

		public static T Last<T>(this List<T> list) {
			return list[list.Count - 1];
		} }
}