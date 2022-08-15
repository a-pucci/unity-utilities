using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AP.Utilities {
	public static class UIUtility {
		#region Elements Pool

		public static GameObject AddElement(GameObject original, Transform parent) {
			GameObject newObject = null;
			foreach (Transform child in parent) {
				if (!child.gameObject.activeSelf) {
					newObject = child.gameObject;
					newObject.SetActive(true);
					break;
				}
			}
			if (newObject == null) 
				newObject = Object.Instantiate(original, parent);
			
			return newObject;
		}

		public static T AddElement<T>(T original, Transform parent) where T : MonoBehaviour {
			T newObject = null;
			foreach (Transform child in parent) {
				if (!child.gameObject.activeSelf) {
					newObject = child.GetComponent<T>();
					if (newObject) 
						newObject.gameObject.SetActive(true);
					
					break;
				}
			}
			if (!newObject) 
				newObject = Object.Instantiate(original, parent);

			return newObject;
		}

		public static void ClearElements(Transform parent) {
			if (parent == null)
				return;
			
			foreach (Transform child in parent) { 
				child.gameObject.SetActive(false);
			}
		}

		#endregion

		#region Navigation

		public static void SetNavigation(NavigationType type, IEnumerable<GameObject> list, bool loop = true) {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			switch(type) {
				case NavigationType.Horizontal: 
					SetHorizontalNavigation(buttons, loop);
					break;
				case NavigationType.Vertical:
					SetVerticalNavigation(buttons, loop);
					break;
			}
		}

		public static void SetNavigation<T>(NavigationType type, IEnumerable<T> list, bool loop = true) where T : MonoBehaviour {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			switch(type) {
				case NavigationType.Horizontal: 
					SetHorizontalNavigation(buttons, loop);
					break;
				case NavigationType.Vertical:
					SetVerticalNavigation(buttons, loop);
					break;
			}
		}
		
		#region Vertical

		public static void SetVerticalNavigation(List<Button> buttons, bool loop = true) {
			
			bool IsFirst(int index) => index == 0;
			bool IsLast(int index) => index == buttons.Count - 1;

			Button GetUp(int i) => IsFirst(i) ? loop ? buttons[buttons.Count - 1] : null : buttons[i - 1];
			Button GetDown(int i) => IsLast(i) ? loop ? buttons[0] : null : buttons[i + 1];

			for (int i = 0; i < buttons.Count; i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnUp = GetUp(i),
					selectOnDown = GetDown(i)
				};
				buttons[i].navigation = nav;
			}
		}

		#endregion

		#region Horizontal

		public static void SetHorizontalNavigation(List<Button> buttons, bool loop = true) {
			
			bool IsFirst(int index) => index == 0;
			bool IsLast(int index) => index == buttons.Count - 1;

			Button GetLeft(int i) => IsFirst(i) ? loop ? buttons[buttons.Count - 1] : null : buttons[i - 1];
			Button GetRight(int i) => IsLast(i) ? loop ? buttons[0] : null : buttons[i + 1];

			for (int i = 0; i < buttons.Count; i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnLeft = GetLeft(i),
					selectOnRight = GetRight(i)
				};
				buttons[i].navigation = nav;
			}
		}

		#endregion

		#region Grid

		public static void SetGridNavigation(List<Button> buttons, int columns, bool loop = false) {
			int count = buttons.Count;
			int rows = count / columns;
			int lastRowIndex = (rows - 1) * columns;
		
			bool IsFirstRow(int index) => index < columns;
			bool IsLastRow(int index) => index >= count - columns;
			bool IsFirstColumn(int index) => index % columns == 0;
			bool IsLastColumn(int index) => index % columns == columns - 1;

			Button GetUp(int i) => IsFirstRow(i) ? loop ? buttons[lastRowIndex + i] : null : buttons[i - columns];
			Button GetDown(int i) => IsLastRow(i) ? loop ? buttons[i % columns] : null : buttons[i + columns];
			Button GetLeft(int i) => IsFirstColumn(i) ? loop ? buttons[i + columns - 1] : null : buttons[i - 1]; 
			Button GetRight(int i) => IsLastColumn(i) ? loop ? buttons[i - columns + 1] : null : buttons[i + 1];

			for (int i = 0; i < buttons.Count; i++) {
				buttons[i].navigation = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnUp = GetUp(i),
					selectOnDown = GetDown(i),
					selectOnLeft = GetLeft(i),
					selectOnRight = GetRight(i)
				};
			}
		}

		public static void SetGridNavigation(IEnumerable<GameObject> list, int gridColumns, bool loop = false) {
			List<Button> buttons = list.Select(b => b.GetComponent<Button>()).ToList();
			SetGridNavigation(buttons, gridColumns, loop);
		}

		public static void SetGridNavigation<T>(IEnumerable<T> list, int gridColumns, bool loop = false) where T : MonoBehaviour {
			List<Button> buttons = list.Select(b => b.GetComponent<Button>()).ToList();
			SetGridNavigation(buttons, gridColumns, loop);
		}

		#endregion

		#region Remove

		public static void RemoveNavigations(IEnumerable<Button> buttons) {
			foreach (Button b in buttons) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.None
				};
				b.navigation = nav;
			}
		}

		public static void RemoveNavigations(IEnumerable<GameObject> list) {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			RemoveNavigations(buttons);
		}

		public static void RemoveNavigations<T>(IEnumerable<T> list) where T : MonoBehaviour {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			RemoveNavigations(buttons);
		}

		#endregion

		#endregion
	}
}