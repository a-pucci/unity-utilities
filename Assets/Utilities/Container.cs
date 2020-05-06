using UnityEngine;

namespace Utilities {
	public class Container<T> : ScriptableObject where T : ScriptableObject {
		protected T[] list;
		public T[] List => list;

#if UNITY_EDITOR

	protected virtual void GetAll() {
		list = Utility.GetAllInstances<T>();
	}
	
#endif
	}
}