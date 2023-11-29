using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AP.Utilities
{
	public static class UIUtility
	{
		#region Elements Pool

		public static GameObject AddElement(GameObject original, Transform parent)
		{
			GameObject newObject;

			foreach (Transform child in parent)
			{
				if (!child.gameObject.activeSelf)
				{
					newObject = child.gameObject;
					newObject.SetActive(true);

					return newObject;
				}
			}

			newObject = Object.Instantiate(original, parent);

			return newObject;
		}

		public static T AddElement<T>(T original, Transform parent) where T : MonoBehaviour
		{
			T newObject;

			foreach (Transform child in parent)
			{
				if (!child.gameObject.activeSelf)
				{
					if (child.TryGetComponent(out newObject))
						newObject.gameObject.SetActive(true);

					return newObject;
				}
			}

			newObject = Object.Instantiate(original, parent);

			return newObject;
		}

		public static void ClearElements(Transform parent)
		{
			if (parent == null)
				return;

			foreach (Transform child in parent)
			{
				child.gameObject.SetActive(false);
			}
		}

		#endregion

		#region Navigation

		public static void SetNavigation(NavigationType type, IEnumerable<GameObject> list, bool loop = true)
		{
			List<Selectable> selectables = list.Select(x => x.GetComponent<Selectable>()).ToList();
			SetNavigation(type, selectables, loop);
		}

		public static void SetNavigation<T>(NavigationType type, IEnumerable<T> list, bool loop = true) where T : MonoBehaviour
		{
			List<Selectable> selectables = list.Select(x => x.GetComponent<Selectable>()).ToList();
			SetNavigation(type, selectables, loop);
		}
		
		public static void SetNavigation(NavigationType type, List<Selectable> selectables, bool loop = true)
		{
			switch (type)
			{
				case NavigationType.Horizontal:
					SetHorizontalNavigation(selectables, loop);
					break;

				case NavigationType.Vertical:
					SetVerticalNavigation(selectables, loop);
					break;
			}
		}

		#region Vertical

		public static void SetVerticalNavigation(List<Selectable> selectables, bool loop = true)
		{
			if (selectables.Count <= 1)
				return;
			
			bool IsFirst(int index) => index == 0;
			bool IsLast(int index) => index == selectables.Count - 1;

			Selectable GetUp(int i) => IsFirst(i) ? loop ? selectables[selectables.Count - 1] : null : selectables[i - 1];
			Selectable GetDown(int i) => IsLast(i) ? loop ? selectables[0] : null : selectables[i + 1];
			
			for (int i = 0; i < selectables.Count; i++)
			{
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnUp = GetUp(i),
					selectOnDown = GetDown(i)
				};

				selectables[i].navigation = nav;
			}
		}

		#endregion

		#region Horizontal

		public static void SetHorizontalNavigation(List<Selectable> selectables, bool loop = true)
		{
			if (selectables.Count <= 1)
				return;
			
			bool IsFirst(int index) => index == 0;
			bool IsLast(int index) => index == selectables.Count - 1;

			Selectable GetLeft(int i) => IsFirst(i) ? loop ? selectables[selectables.Count - 1] : null : selectables[i - 1];
			Selectable GetRight(int i) => IsLast(i) ? loop ? selectables[0] : null : selectables[i + 1];

			for (int i = 0; i < selectables.Count; i++)
			{
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnLeft = GetLeft(i),
					selectOnRight = GetRight(i)
				};

				selectables[i].navigation = nav;
			}
		}

		#endregion

		#region Grid

		public static void SetGridNavigation(List<Selectable> selectables, int columns, bool loop = false)
		{
			if (columns == 0 || selectables.Count == 0)
				return;
			
			int count = selectables.Count;
			int rows = count / columns;
			int lastRowIndex = (rows - 1) * columns;

			bool IsFirstRow(int index) => index < columns;
			bool IsLastRow(int index) => index >= count - columns;
			bool IsFirstColumn(int index) => index % columns == 0;
			bool IsLastColumn(int index) => index % columns == columns - 1;
			
			bool IsInRange(int index) => index >= 0 && index < count;

			Selectable GetUp(int i) => IsFirstRow(i) ? loop ? selectables[lastRowIndex + i] : null : IsInRange(i - columns) ? selectables[i - columns] : null;
			Selectable GetDown(int i) => IsLastRow(i) ? loop ? selectables[i % columns] : null : IsInRange(i + columns) ? selectables[i + columns] : null;
			Selectable GetLeft(int i) => IsFirstColumn(i) ? loop ? selectables[i + columns - 1] : null : IsInRange(i - 1) ? selectables[i - 1] : null;
			Selectable GetRight(int i) => IsLastColumn(i) ? loop ? selectables[i - columns + 1] : null : IsInRange(i + 1) ? selectables[i + 1] : null;

			for (int i = 0; i < selectables.Count; i++)
			{
				selectables[i].navigation = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnUp = GetUp(i),
					selectOnDown = GetDown(i),
					selectOnLeft = GetLeft(i),
					selectOnRight = GetRight(i)
				};
			}
		}

		public static void SetGridNavigation(IEnumerable<GameObject> list, int gridColumns, bool loop = false)
		{
			List<Selectable> selectables = list.Select(b => b.GetComponent<Selectable>()).ToList();
			SetGridNavigation(selectables, gridColumns, loop);
		}

		public static void SetGridNavigation<T>(IEnumerable<T> list, int gridColumns, bool loop = false) where T : MonoBehaviour
		{
			List<Selectable> selectables = list.Select(b => b.GetComponent<Selectable>()).ToList();
			SetGridNavigation(selectables, gridColumns, loop);
		}

		#endregion

		#region Remove

		public static void RemoveNavigations(IEnumerable<Selectable> selectables)
		{
			foreach (Selectable b in selectables)
			{
				var nav = new Navigation
				{
					mode = Navigation.Mode.None
				};

				b.navigation = nav;
			}
		}

		public static void RemoveNavigations(IEnumerable<GameObject> list)
		{
			List<Selectable> selectables = list.Select(x => x.GetComponent<Selectable>()).ToList();
			RemoveNavigations(selectables);
		}

		public static void RemoveNavigations<T>(IEnumerable<T> list) where T : MonoBehaviour
		{
			List<Selectable> selectables = list.Select(x => x.GetComponent<Selectable>()).ToList();
			RemoveNavigations(selectables);
		}

		#endregion

		#endregion
	}
}