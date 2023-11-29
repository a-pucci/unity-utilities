using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.Reflection;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AP.Utilities.Extensions
{
	public static class Extensions
	{
		#region GameObject

		public static void DestroyChildren(this GameObject obj) => DestroyChildren(obj.transform);

		public static T GetOrAddComponent<T>(this GameObject obj) where T : Component => obj.TryGetComponent(out T oldComp) ? oldComp : obj.AddComponent<T>();

		public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component) where T : Component
		{
			if (obj.TryGetComponent(out component))
				return true;

			foreach (Transform child in obj.transform)
			{
				if (child.gameObject.TryGetComponentInChildren(out component))
					return true;
			}
			return false;
		}
		
		public static bool HasComponent<T>(this GameObject obj) => obj.TryGetComponent(out T _);
		
		#endregion
		
		#region Transform
	
		public static void DestroyChildren(this Transform obj)
		{
			foreach (Transform child in obj)
			{
				Object.Destroy(child.gameObject);
			}
		}
	
		public static void SetChildrenActive(this Transform obj, bool value)
		{
			foreach (Transform child in obj)
			{
				child.gameObject.SetActive(value);
			}
		}
	
		#endregion

		#region Button

		public static void SetNavigation(this Selectable button, Selectable up, Selectable down, Selectable left, Selectable right)
		{
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

		public static void Show(this CanvasGroup cg, bool value)
		{
			cg.alpha = value.ToInt();
			cg.interactable = value;
			cg.blocksRaycasts = value;
		}

		#endregion

		#region MonoBehaviour

		public static Coroutine ExecuteNextFrame(this MonoBehaviour mb, Action action)
		{
			IEnumerator Wait(Action a)
			{
				yield return null;

				a.Invoke();
			}

			return mb.StartCoroutine(Wait(action));
		}

		public static Coroutine ExecuteAfterFrames(this MonoBehaviour mb, Action action, int frames)
		{
			IEnumerator Wait(Action a)
			{
				for (int i = 0; i < frames; i++)
				{
					yield return null;
				}

				a.Invoke();
			}

			return mb.StartCoroutine(Wait(action));
		}

		public static Coroutine WaitFrames(this MonoBehaviour mb, int frames)
		{
			IEnumerator Wait()
			{
				var frame = new WaitForEndOfFrame();

				for (int i = 0; i < frames; i++)
				{
					yield return frame;
				}
			}

			return mb.StartCoroutine(Wait());
		}

		public static Coroutine ExecuteAfterDelay(this MonoBehaviour mb, Action action, float time)
		{
			IEnumerator Wait(Action a)
			{
				yield return new WaitForSeconds(time);

				a.Invoke();
			}

			return mb.StartCoroutine(Wait(action));
		}

		#endregion

		#region ScriptableObject

#if UNITY_EDITOR
		public static void SetDirty(this ScriptableObject so) => EditorUtility.SetDirty(so);
#endif

		#endregion

		#region Collections

		public static T First<T>(this List<T> list) => list[0];

		public static T Last<T>(this List<T> list) => list[list.Count - 1];

		public static List<T> Distinct<T>(this List<T> list)
		{
			var temp = new List<T>();

			foreach (T element in list)
			{
				if (!temp.Contains(element))
					temp.Add(element);
			}

			return temp;
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			for (int i = 0; i < list.Count - 1; i++)
			{
				int index = Random.Range(i, list.Count);
				(list[i], list[index]) = (list[index], list[i]);
			}
		}

		public static void Remove<T>(this IList<T> list, IEnumerable<T> toRemove)
		{
			foreach (T element in toRemove)
			{
				list.Remove(element);
			}
		}
		
		public static void RemoveLast<T>(this List<T> list) => list.RemoveAt(list.Count - 1);

		public static bool FindAndRemove<T>(this List<T> list, Predicate<T> match)
		{
			T element = list.Find(match);

			return element != null && list.Remove(element);
		}

		public static T GetRandom<T>(this IList<T> list) => list[Random.Range(0, list.Count)];

		public static T GetRandom<T>(this IEnumerable<T> list, T toIgnore)
		{
			var l = new List<T>(list);
			l.Remove(toIgnore);

			return l.GetRandom();
		}
		
		public static T GetRandom<T>(this IEnumerable<T> list, IEnumerable<T> toIgnore)
		{
			var l = new List<T>(list);
			l.Remove(toIgnore);
			return l.GetRandom();
		}

		public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
		{
			foreach (T obj in ie)
			{
				action(obj);
			}
		}

		public static List<T> ShiftLeft<T>(this List<T> list, int shiftBy)
		{
			if (list.Count <= shiftBy)
				return list;

			List<T> result = list.GetRange(shiftBy, list.Count - shiftBy);
			result.AddRange(list.GetRange(0, shiftBy));

			return result;
		}

		public static List<T> ShiftRight<T>(this List<T> list, int shiftBy)
		{
			if (list.Count <= shiftBy)
				return list;

			List<T> result = list.GetRange(list.Count - shiftBy, shiftBy);
			result.AddRange(list.GetRange(0, list.Count - shiftBy));

			return result;
		}

		public static int CountNullSlots<T>(this T[] list, int startIndex = 0, int endIndex = 0)
		{
			if (endIndex == 0)
				endIndex = list.Length;

			int count = 0;

			for (int i = startIndex; i < endIndex; i++)
			{
				if (list[i] == null)
					count++;
			}

			return count;
		}
		
		public static T[] GetSubArray<T>(this T[] array, int start, int length)
		{
			var newArr = new T[length];
			for(int i = start; i < start+length; i++){
				newArr[i-start] = array[i];
			}
			return newArr;
		}

		public static bool IsNullOrEmpty<T>(this IList<T> list) => list == null || list.Count == 0;

		public static bool IsInRange<T>(this IList<T> list, int index) => index >= 0 && index < list.Count;
		
		public static void Swap<T>(this IList<T> list, int indexA, int indexB) => (list[indexA], list[indexB]) = (list[indexB], list[indexA]);

		public static bool IsNullOrEmpty<T1, T2>(this Dictionary<T1, T2> dict) => dict == null || dict.Count == 0;

		#endregion

		#region String

		public static string UppercaseFirst(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;

			return char.ToUpper(s[0]) + s.Substring(1);
		}

		public static string SnakeCaseToCapitalizedCase(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;

			string[] sA = s.Split('_');

			for (int i = 0; i < sA.Length; i++)
			{
				sA[i] = sA[i].UppercaseFirst();
			}

			return string.Join(" ", sA);
		}
		
		public static string ToCamelCase(this string s, char separator)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;

			string[] sA = s.Split(separator);

			if (sA.Length > 1)
			{
				for (int i = 1; i < sA.Length; i++)
				{
					sA[i] = sA[i].UppercaseFirst();
				}
			}

			return string.Join("", sA);
		}

		public static string SnakeCaseToUpperCase(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;

			string[] sA = s.Split('_');

			for (int i = 0; i < sA.Length; i++)
			{
				sA[i] = sA[i].ToUpper();
			}

			return string.Join(" ", sA);
		}

		public static string CapitalizedCaseToSnakeCase(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;

			string ss = s.Replace(" ", "_");

			return ss.ToLower();
		}

		public static string CapitalizedCaseToCamelCase(this string s)
		{
			s = char.ToLower(s[0]) + s.Substring(1);

			return s.Replace(" ", "");
		}
		
		public static string ToTitleCase(this string str)
		{
			var stringBuilder = new StringBuilder();
			if (str.IsNullOrEmpty())
				return string.Empty;

			byte lastCharType = 1; // 0 -> lowercase, 1 -> _ (underscore), 2 -> number, 3 -> uppercase
			int index = 0;
			if (str.Length > 1 && str[1] == '_')
				index = 2;

			stringBuilder.Length = 0;
			for (; index < str.Length; index++)
			{
				char ch = str[index];
				if (char.IsUpper(ch))
				{
					if ((lastCharType < 2 || (str.Length > index + 1 && char.IsLower(str[index + 1]))) && stringBuilder.Length > 0)
						stringBuilder.Append(' ');

					stringBuilder.Append(ch);
					lastCharType = 3;
				}
				else if (ch == '_')
					lastCharType = 1;
				else if (char.IsNumber(ch))
				{
					if (lastCharType != 2 && stringBuilder.Length > 0)
						stringBuilder.Append(' ');

					stringBuilder.Append(ch);
					lastCharType = 2;
				}
				else
				{
					if (lastCharType is 1 or 2)
					{
						if (stringBuilder.Length > 0)
							stringBuilder.Append(' ');

						stringBuilder.Append(char.ToUpper(ch));
					}
					else
						stringBuilder.Append(ch);

					lastCharType = 0;
				}
			}

			return stringBuilder.Length == 0 ? str : stringBuilder.ToString();
		}

		public static bool IsNullOrEmpty(this string text) => string.IsNullOrWhiteSpace(text);

		public static string SubstringByWords(this string text, int i, char separator = ' ')
		{
			string[] words = text.Split(separator);

			if (i <= words.Length)
			{
				string result = string.Empty;

				for (int j = 0; j <= i - 1; j++)
				{
					result += words[j] + separator;
				}

				return result.TrimEnd(separator);
			}
			else
				return "";
		}

		public static string GetWord(this string text, char separator, int i)
		{
			string[] words = text.Split(separator);

			return i < words.Length + 1 ? words[i - 1] : "";
		}

		public static string FirstWord(this string text, char separator) => text.Split(separator)[0];

		public static string LastWord(this string text, char separator)
		{
			string[] t = text.Split(separator);

			return t[t.Length - 1];
		}

		public static T ToEnum<T>(this string value)
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException();
			
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static string RTBold(this string text) => $"<b>{text}</b>";

		public static string RTSize(this string text, int size) => $"<size={size}>{text}</size>";

		public static string RTColor(this string text, RTColors color) => $"<color={color.ToString().ToLower()}>{text}</color>";

		#endregion

		#region Vector3

		/// Todo TEST
		public static Vector3 PerpendicularCW(this Vector3 vector, Vector3 dir) => Vector3.Cross(vector, dir);

		/// Todo TEST
		public static Vector3 PerpendicularCCW(this Vector3 vector, Vector3 dir) => -Vector3.Cross(vector, dir);

		public static Vector2 FlipX(this Vector2 vector) => new Vector2(-vector.x, vector.y);

		public static Vector2 FlipY(this Vector2 vector) => new Vector2(vector.x, -vector.y);

		public static Vector3 FlipX(this Vector3 vector) => new Vector3(-vector.x, vector.y, vector.z);

		public static Vector3 FlipY(this Vector3 vector) => new Vector3(vector.x, -vector.y, vector.z);

		public static Vector3 FlipZ(this Vector3 vector) => new Vector3(vector.x, vector.y, -vector.z);
		
		public static Vector2 WithX(this Vector2 vector, float x) => new Vector2(x, vector.y);

		public static Vector2 WithY(this Vector2 vector, float y) => new Vector2(vector.x, y);
	
		public static Vector3 WithX(this Vector3 vector, float x) => new Vector3(x, vector.y, vector.z);

		public static Vector3 WithY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);

		public static Vector3 WithZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, z);

		#endregion

		#region Int

		public static string ToRoundFormat(this int number)
		{
			if (number < 1000)
				return number.ToString();

			if (number < 1000000)
				return (number / (float)1000).ToString("#.00") + "K";

			if (number < 1000000000d)
				return (number / (float)1000000).ToString("#.00") + "M";

			return (number / (float)1000000000).ToString("#.00") + "B";
		}

		public static int Remap(this int number, int fromMin, int fromMax, int toMin, int toMax) => toMin + (number - fromMin) * (toMax - toMin) / (fromMax - fromMin);

		private static string GetOrdinalStringEnd(this int number)
		{
			int x = number % 10;

			return x switch
			{
				1 => "st",
				2 => "nd",
				3 => "rd",
				_ => "th"
			};
		}

		#endregion
		
		#region UInt
		
		public static bool IsInRange(this uint value, uint min, uint max, bool includeLimits = true)
		{
			if (includeLimits) return min <= value && value <= max;
			else return min < value && value < max;
		}
	
		#endregion

		#region Float

		public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax) => toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);

		public static float Remap(this int number, int fromMin, int fromMax, float toMin, float toMax) => toMin + (number - fromMin) * (toMax - toMin) / (fromMax - fromMin);

		public static float Distance(this float a, float b) => Mathf.Abs(a - b);

		#endregion

		#region Bool

		public static int ToInt(this bool value) => value ? 1 : 0;

		#endregion

		#region Enum

		public static List<string> GetEnumNames<T>(this T e)
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("Not an Enum type");

			return Enum.GetNames(typeof(T)).ToList();
		}
		
		public static List<T> GetEnumValues<T>(this T e)
		{
			if (!e.GetType().IsEnum)
				throw new ArgumentException("Not an Enum type");
		
			return Enum.GetValues(typeof(T)).Cast<T>().ToList();
		} 

		#endregion

		#region Type
		
		public static T GetFieldValue<T>(this object obj, string name) {
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			FieldInfo field = obj.GetType().GetField(name, bindingFlags);
			return (T)field?.GetValue(obj);
		}
        
		public static void SetFieldValue(this object obj, string name, object value) {
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			FieldInfo field = obj.GetType().GetField(name, bindingFlags);
			field?.SetValue(obj, value);
		}

#if UNITY_EDITOR

		public static IEnumerable<FieldInfo> GetAllFieldsWithAttribute(this Type objectType, Type attributeType) => objectType.GetFields().Where(f => f.GetCustomAttributes(attributeType, false).Any());

#endif

		#endregion

		#region Sprite

		public static Texture2D GetTexture(this Sprite sprite)
		{
			if (sprite == null)
				return null;

			if (sprite.rect.width != sprite.texture.width)
			{
				var texture2D = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
				Color[] colors = sprite.texture.GetPixels(Mathf.CeilToInt(sprite.rect.x),
					Mathf.CeilToInt(sprite.rect.y),
					Mathf.CeilToInt(sprite.rect.width),
					Mathf.CeilToInt(sprite.rect.height));

				texture2D.SetPixels(colors);
				texture2D.Apply();

				return texture2D;
			}
			else
			{
				return sprite.texture;
			}
		}

		public static Color32[] GetPixels32(this Sprite sprite)
		{
			if (sprite == null)
				return null;

			if (sprite.rect.width != sprite.texture.width)
			{
				var texture2D = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
				Color[] colors = sprite.texture.GetPixels(Mathf.CeilToInt(sprite.rect.x),
					Mathf.CeilToInt(sprite.rect.y),
					Mathf.CeilToInt(sprite.rect.width),
					Mathf.CeilToInt(sprite.rect.height));

				texture2D.SetPixels(colors);

				return texture2D.GetPixels32();
			}
			else
				return sprite.texture.GetPixels32();
		}

		#endregion

		#region Color32

		public static bool IsEqualTo(this Color32 color1, Color32 color2) => color1.r == color2.r && color1.g == color2.g && color1.b == color2.b && color1.a == color2.a;

		#endregion

		#region Color

		public static string ToHtml(this Color color) => ColorUtility.ToHtmlStringRGB(color);

		public static Color Opaque(this Color color) => new Color(color.r, color.g, color.b);

		public static Color Invert(this Color color) => new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);

		public static Color WithAlpha(this Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

		#endregion

		#region Rect

		public static Vector2 TopLeft(this Rect rect) => new Vector2(rect.xMin, rect.yMin);

		public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale;
			result.xMax *= scale;
			result.yMin *= scale;
			result.yMax *= scale;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;

			return result;
		}

		#endregion

		#region RectTransform

		public static RectTransform Copy(this RectTransform target)
		{
			var copy = new RectTransform
			{
				localScale = target.localScale,
				anchorMin = target.anchorMin,
				anchorMax = target.anchorMax,
				pivot = target.pivot,
				sizeDelta = target.sizeDelta,
				anchoredPosition3D = target.anchoredPosition3D,
				rotation = target.rotation
			};

			return copy;
		}

		public static void ResetScaleAndPosition(this RectTransform rect)
		{
			rect.anchoredPosition = Vector2.zero;
			rect.localScale = Vector3.one;
		}

		#endregion
		
		#region Camera
	
		public static bool ScreenPointToPlane(this Camera camera, Vector3 point, Plane plane, out Vector3 hitPoint)
		{
			Ray ray = camera.ScreenPointToRay(point);
			return RayToPlane(plane, out hitPoint, ray);
		}
	
		public static bool CameraCenterToPlane(this Camera camera, Plane plane, out Vector3 hitPoint)
		{
			Transform cameraTransf = camera.gameObject.transform;
			var ray = new Ray(cameraTransf.position, cameraTransf.forward);
			return RayToPlane(plane, out hitPoint, ray);
		}

		private static bool RayToPlane(Plane plane, out Vector3 hitPoint, Ray ray)
		{
			if (plane.Raycast(ray, out float hitDist))
			{
				// check for the intersection point between ray and plane
				hitPoint = ray.GetPoint(hitDist);
				return true;
			}
			if (hitDist < -1.0f)
			{
				// when point is "behind" plane (hitdist != zero, fe for far away orthographic camera)
				// simply switch sign https://docs.unity3d.com/ScriptReference/Plane.Raycast.html
				hitPoint = ray.GetPoint(-hitDist);
				return true;
			}
			// both are parallel or plane is behind camera so write a log and return zero vector
			hitPoint = Vector3.zero;
			return false;
		}

		#endregion
		
		#region AnimationCurve
	
		public static float GetValueRatio(this AnimationCurve c, float value)
		{
			Keyframe minFrame = c.keys[^1];
			Keyframe maxFrame = c.keys[0];
		
			if (value <= minFrame.value)
				return minFrame.time;
			else if (value >= maxFrame.value)
				return maxFrame.time;
		
			float min = 0;
			float max = 1;
			const float epsilon = 0.01f;
		
			while (min <= max)
			{
				float mid = (min + max) / 2;
				float lowMid = c.Evaluate(mid - epsilon);
				float highMid = c.Evaluate(mid + epsilon);
			
				if ((value <= lowMid && value >= highMid))
					return mid;
			
				float midVal = c.Evaluate(mid);
				if (value > midVal)
					max = mid - epsilon;
				else
					min = mid + epsilon;
			}
			return 0;
		}
	
		#endregion
	}
}