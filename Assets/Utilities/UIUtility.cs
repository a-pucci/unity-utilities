using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Utilities {
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
			if (newObject == null) {
				newObject = Object.Instantiate(original, parent);
			}

			return newObject;
		}

		public static T AddElement<T>(T original, Transform parent) where T : MonoBehaviour {
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

			return newObject;
		}

		public static void ClearElements(Transform parent) {
			if (parent != null) {
				foreach (Transform child in parent) {
					child.gameObject.SetActive(false);
				}
			}
		}

		#endregion

		#region Navigation

		#region Verical

		public static void SetVerticalNavigation(List<Button> buttons, bool loop = true) {
			for (int i = 0; i < buttons.Count; i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnUp = i == 0 ? loop ? buttons[buttons.Count - 1] : null : buttons[i - 1],
					selectOnDown = i == buttons.Count - 1 ? loop ? buttons[0] : null : buttons[i + 1]
				};
				buttons[i].navigation = nav;
			}
		}

		public static void SetVerticalNavigation(IEnumerable<GameObject> list, bool loop = true) {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			for (int i = 0; i < buttons.Count; i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnUp = i == 0 ? loop ? buttons[buttons.Count - 1] : null : buttons[i - 1],
					selectOnDown = i == buttons.Count - 1 ? loop ? buttons[0] : null : buttons[i + 1]
				};
				buttons[i].navigation = nav;
			}
		}

		public static void SetVerticalNavigation<T>(IEnumerable<T> list, bool loop = true) where T : MonoBehaviour {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			for (int i = 0; i < buttons.Count; i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnUp = i == 0 ? loop ? buttons[buttons.Count - 1] : null : buttons[i - 1],
					selectOnDown = i == buttons.Count - 1 ? loop ? buttons[0] : null : buttons[i + 1]
				};
				buttons[i].navigation = nav;
			}
		}

		#endregion

		#region Horizontal

		public static void SetHorizontalNavigation(List<Button> buttons, bool loop = true) {
			for (int i = 0; i < buttons.Count; i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnLeft = i == 0 ? loop ? buttons[buttons.Count - 1] : null : buttons[i - 1],
					selectOnRight = i == buttons.Count - 1 ? loop ? buttons[0] : null : buttons[i + 1]
				};
				buttons[i].navigation = nav;
			}
		}

		public static void SetHorizontalNavigation(IEnumerable<GameObject> list, bool loop = true) {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			for (int i = 0; i < buttons.Count; i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnLeft = i == 0 ? loop ? buttons[buttons.Count - 1] : null : buttons[i - 1],
					selectOnRight = i == buttons.Count - 1 ? loop ? buttons[0] : null : buttons[i + 1]
				};
				buttons[i].navigation = nav;
			}
		}

		public static void SetHorizontalNavigation<T>(IEnumerable<T> list, bool loop = true) where T : MonoBehaviour {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			for (int i = 0; i < buttons.Count; i++) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnLeft = i == 0 ? loop ? buttons[buttons.Count - 1] : null : buttons[i - 1],
					selectOnRight = i == buttons.Count - 1 ? loop ? buttons[0] : null : buttons[i + 1]
				};
				buttons[i].navigation = nav;
			}
		}

		#endregion

		#region Grid

		public static void SetGridNavigations(List<Button> buttons, int gridColumns) {
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

		public static void SetGridNavigations(IEnumerable<GameObject> list, int gridColumns) {
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
			foreach (Button b in buttons) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.None
				};
				b.navigation = nav;
			}
		}

		public static void RemoveNavigations<T>(IEnumerable<T> list) where T : MonoBehaviour {
			List<Button> buttons = list.Select(x => x.GetComponent<Button>()).ToList();
			foreach (Button b in buttons) {
				var nav = new Navigation
				{
					mode = Navigation.Mode.None
				};
				b.navigation = nav;
			}
		}

		#endregion

		#endregion
	}
}