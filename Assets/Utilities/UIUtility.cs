using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Utilities {
	public static class UIUtility {
		#region Elements Pool
		
		public static GameObject AddElement(GameObject original, Transform parent, Action<GameObject> set = null, Action onClick = null) {
			GameObject newObject = null;
			foreach (Transform child in parent) {
				if (!child.gameObject.activeSelf) {
					newObject = child.gameObject;
					newObject.SetActive(true);
					break;
				}
			}
			if (newObject == null) {
				newObject = Object.Instantiate(original, parent);
			}
			set?.Invoke(newObject);

			if (onClick != null) {
				var button = newObject.GetComponent<Button>();
				if (button != null) {
					button.onClick.RemoveAllListeners();
					button.onClick.AddListener(new UnityEngine.Events.UnityAction(onClick));
				}
			}
			return newObject;
		}

		public static T AddElement<T>(T original, Transform parent, Action<T> set = null, Action onClick = null) where T : MonoBehaviour {
			T newObject = null;
			foreach (Transform child in parent) {
				if (!child.gameObject.activeSelf) {
					newObject = child.GetComponent<T>();
					if (newObject) {
						newObject.gameObject.SetActive(true);
					}
					break;
				}
			}
			if (!newObject) {
				newObject = Object.Instantiate(original, parent);
			}
			set?.Invoke(newObject);

			if (onClick != null) {
				var button = newObject.GetComponent<Button>();
				if (button) {
					button.onClick.RemoveAllListeners();
					button.onClick.AddListener(new UnityEngine.Events.UnityAction(onClick));
				}
			}
			return newObject;
		}
		
		public static void ClearElements(Transform parent) {
			if (parent != null) {
				foreach (Transform child in parent) {
					var button = child.GetComponent<Button>();
					if (button) {
						button.onClick.RemoveAllListeners();
					}
					child.gameObject.SetActive(false);
				}
			}
		}
		
		#endregion

		#region Navigation

		public static void SetVerticalNavigation<T>(IReadOnlyList<T> list, bool loop) where T : MonoBehaviour {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			for (int i = 0; i < buttons.Count(); i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnUp = i == 0 ? (loop ? buttons[buttons.Count - 1] : null) : buttons[i - 1],
					selectOnDown = i == buttons.Count - 1 ? (loop ? buttons[0] : null) : buttons[i + 1]
				};
				buttons[i].navigation = nav;
			}
		}

		public static void SetHorizontalNavigation<T>(IReadOnlyList<T> list, bool loop) where T : MonoBehaviour {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			for (int i = 0; i < buttons.Count(); i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnLeft = i == 0 ? (loop ? buttons[buttons.Count - 1] : null) : buttons[i - 1],
					selectOnRight = i == buttons.Count - 1 ? (loop ? buttons[0] : null) : buttons[i + 1]
				};
				buttons[i].navigation = nav;
			}
		}

		public static void SetGridNavigations<T>(IEnumerable<T> list, int gridColumns) where T : MonoBehaviour {
			List<Button> buttons = list.Select(b => b.GetComponent<Button>()).ToList();

			for (int i = 0; i < buttons.Count; i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit
				};

				if (i < buttons.Count - gridColumns && i + gridColumns < buttons.Count) {
					nav.selectOnDown = buttons[i + gridColumns];
				}
				if (i >= gridColumns) {
					nav.selectOnUp = buttons[i - gridColumns];
				}
				if (i % gridColumns != gridColumns - 1 && i + 1 < buttons.Count) {
					nav.selectOnRight = buttons[i + 1];
				}
				if (i % gridColumns != 0) {
					nav.selectOnLeft = buttons[i - 1];
				}
				buttons[i].navigation = nav;
			}
		}

		public static void SetScrollbarsValue(ref ScrollRect scrollRect, Vector2 firstPosition, Vector2 lastPosition, Vector2 currentPosition) {
			if (scrollRect.horizontal) {
				scrollRect.horizontalScrollbar.value = 1 - Mathf.InverseLerp(firstPosition.x, lastPosition.x, currentPosition.x);
			}
			if (scrollRect.vertical) {
				scrollRect.verticalScrollbar.value = 1 - Mathf.InverseLerp(firstPosition.y, lastPosition.y, currentPosition.y);
			}
		}

		#endregion
	}
}