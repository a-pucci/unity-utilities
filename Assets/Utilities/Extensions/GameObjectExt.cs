using UnityEngine;

namespace Utilities.Extensions {
	public static class GameObjectExt {
		public static T AddComponentToParent<T>(this Component obj) where T : Component {
			return obj.transform.parent.gameObject.AddComponent<T>();
		}
		public static T AddComponentToParent<T>(this GameObject obj) where T : Component {
			return obj.transform.parent.gameObject.AddComponent<T>();
		}

		public static GameObject DestroyChildren(this GameObject obj) {
			foreach (Transform child in obj.transform) {
				Object.Destroy(child.gameObject);
			}
			return obj;
		}
	}
}