using System;
using AP.Utilities.Editor;
using UnityEngine;

namespace AP.Utilities
{
	public class Container<T> : ScriptableObject where T : ScriptableObject
	{
		protected T[] list;
		public T[] List => list;

		public int GetItemIndex(T item) => Array.IndexOf(list, item);

		public int GetItemIndex(string itemName) => Array.FindIndex(list, c => c.name == itemName);

		public T GetItem(string itemName) => Array.Find(list, c => c.name == itemName);

#if UNITY_EDITOR

		protected virtual void GetAll() => list = EditorUtilities.GetAllInstances<T>();

#endif
	}
}