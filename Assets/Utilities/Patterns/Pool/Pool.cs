using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities.Patterns {
	public class Pool<T> where T : MonoBehaviour {
		
		public Stack<T> Stack { get; } = new Stack<T>();
		private T prefab;

		public Pool(T prefab, int poolSize, Transform parent = null) {
			this.prefab = prefab;
			AddToPool(poolSize, parent);
		}

		public Pool(T[] prefabs, int poolSize, Transform parent = null) {
			prefab = prefabs[Random.Range(0, prefabs.Length)];
			AddToPool(prefabs, poolSize, parent);
		}

		public T Get() {
			if (Stack.Count == 0) {
				AddToPool(1);
			}

			return Stack.Pop();
		}

		public void AddToPool(int count, Transform parent = null) {
			for (int i = 0; i < count; i++) {
				T obj = Object.Instantiate(prefab, parent);
				obj.gameObject.SetActive(false);
				Stack.Push(obj);
			}
		}

		public void AddToPool(T[] prefabs, int count, Transform parent = null) {
			for (int i = 0; i < count; i++) {
				T obj = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], parent);
				obj.gameObject.SetActive(false);
				Stack.Push(obj);
			}
		}

		public void Release(T obj) {
			obj.gameObject.SetActive(false);
			Stack.Push(obj);
		}

		public void Clear() {
			var temp = new List<T>(Stack);
			temp.ForEach(o => Object.Destroy(o.gameObject));
			Stack.Clear();
		}
	}
}