using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using System.Reflection;
#endif
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Utilities.Extensions {
	public static class Extensions {
		#region GameObject

		public static void DestroyChildren(this GameObject obj) {
			foreach (Transform child in obj.transform) {
				Object.Destroy(child.gameObject);
			}
		}

		public static T GetOrAddComponent<T>(this GameObject obj) where T : Component {
			var newComp = obj.GetComponent<T>();
			if (newComp == null) {
				newComp = obj.AddComponent<T>();
			}
			return newComp;
		}

		#endregion

		#region Button

		public static void SetNavigation(this Selectable button, Selectable up, Selectable down, Selectable left, Selectable right) {
			var nav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnUp = up,
				selectOnDown = down,
				selectOnLeft = left,
				selectOnRight = right
			};
			button.navigation = nav;
		}

		#endregion

		#region Canvas Group

		public static void Show(this CanvasGroup cg, bool value) {
			cg.alpha = value.ToInt();
			cg.interactable = value;
			cg.blocksRaycasts = value;
		}

		#endregion

		#region Monobehaviour

		public static Coroutine ExecuteNextFrame(this MonoBehaviour mb, Action action) {
			IEnumerator Wait(Action a) {
				yield return null;
				a.Invoke();
			}

			return mb.StartCoroutine(Wait(action));
		}

		public static Coroutine ExecuteAfterFrames(this MonoBehaviour mb, Action action, int frames) {
			IEnumerator Wait(Action a) {
				var frame = new WaitForEndOfFrame();
				for (int i = 0; i < frames; i++) {
					yield return frame;
				}
				a.Invoke();
			}

			return mb.StartCoroutine(Wait(action));
		}

		public static Coroutine WaitFrames(this MonoBehaviour mb, int frames) {
			IEnumerator Wait() {
				var frame = new WaitForEndOfFrame();
				for (int i = 0; i < frames; i++) {
					yield return frame;
				}
			}

			return mb.StartCoroutine(Wait());
		}

		public static Coroutine ExecuteAfterDelay(this MonoBehaviour mb, Action action, float time) {
			IEnumerator Wait(Action a) {
				yield return new WaitForSeconds(time);
				a.Invoke();
			}

			return mb.StartCoroutine(Wait(action));
		}

		#endregion

		#region List

		public static T First<T>(this List<T> list) {
			return list[0];
		}

		public static T Last<T>(this List<T> list) {
			return list[list.Count - 1];
		}

		public static List<T> Distinct<T>(this List<T> list) {
			var temp = new List<T>();
			foreach (T element in list) {
				if (!temp.Contains(element)) {
					temp.Add(element);
				}
			}
			return temp;
		}

		public static void Shuffle<T>(this IList<T> list) {
			for (int i = 0; i < list.Count - 1; i++) {
				int index = Random.Range(i, list.Count);
				T temp = list[i];
				list[i] = list[index];
				list[index] = temp;
			}
		}

		public static bool FindAndRemove<T>(this List<T> list, Predicate<T> match) {
			T element = list.Find(match);
			return element != null && list.Remove(element);
		}

		public static T GetRandom<T>(this IList<T> list) {
			return list[Random.Range(0, list.Count)];
		}

		public static T GetRandom<T>(this IList<T> list, T toIgnore) {
			var l = new List<T>(list);
			l.Remove(toIgnore);
			return l.GetRandom();
		}

		#endregion

		#region String

		public static string UppercaseFirst(this string s) {
			if (string.IsNullOrEmpty(s)) {
				return string.Empty;
			}
			return char.ToUpper(s[0]) + s.Substring(1);
		}

		public static string SnakeCaseToCapitalizedCase(this string s) {
			if (string.IsNullOrEmpty(s)) {
				return string.Empty;
			}
			string[] sA = s.Split('_');
			for (int i = 0; i < sA.Length; i++) {
				sA[i] = sA[i].UppercaseFirst();
			}
			return string.Join(" ", sA);
		}

		public static string SnakeCaseToUpperCase(this string s) {
			if (string.IsNullOrEmpty(s)) {
				return string.Empty;
			}
			string[] sA = s.Split('_');
			for (int i = 0; i < sA.Length; i++) {
				sA[i] = sA[i].ToUpper();
			}
			return string.Join(" ", sA);
		}

		#endregion

		#region Vector3

		public static Vector3 OppositeDirection(this Vector3 vector) {
			return new Vector3(-vector.x, vector.y, -vector.z).normalized;
		}

		public static Vector3 PerpendicularClockwise(this Vector3 vector) {
			return new Vector3(vector.z, 0, -vector.x);
		}

		public static Vector3 PerpendicularCounterClockwise(this Vector3 vector) {
			return new Vector3(-vector.z, 0, vector.x);
		}

		#endregion

		#region Int

		public static string ToRoundFormat(this int num) {
			if (num < 1000) {
				return num.ToString();
			}
			if (num < 1000000) {
				return (num / (float)1000).ToString("#.00") + "K";
			}
			if (num < 1000000000d) {
				return (num / (float)1000000).ToString("#.00") + "M";
			}
			return (num / (float)1000000000).ToString("#.00") + "B";
		}

		#endregion

		#region Float

		public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax) {
			return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
		}

		public static float Remap(this int value, int fromMin, int fromMax, float toMin, float toMax) {
			return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
		}

		public static int Remap(this int value, int fromMin, int fromMax, int toMin, int toMax) {
			return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
		}

		#endregion

		#region Bool

		public static int ToInt(this bool value) {
			return value ? 1 : 0;
		}

		#endregion

		#region Type

#if UNITY_EDITOR

		public static IEnumerable<FieldInfo> GetAllFieldsWithAttribute(this Type objectType, Type attributeType) {
			return objectType.GetFields().Where(
				f => f.GetCustomAttributes(attributeType, false).Any());
		}

#endif

		#endregion
	}
}