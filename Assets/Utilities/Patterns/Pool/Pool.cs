using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities.Patterns {
	public class Pool<T> where T : MonoBehaviour {
		public Queue<T> Queue { get; } = new Queue<T>();
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
			if (Queue.Count == 0) {
				AddToPool(1);
			}

			return Queue.Dequeue();
		}

		public void AddToPool(int count, Transform parent = null) {
			for (int i = 0; i < count; i++) {
				T obj = Object.Instantiate(prefab, parent);
				obj.gameObject.SetActive(false);
				Queue.Enqueue(obj);
			}
		}
	
		public void AddToPool(T[] prefabs, int count, Transform parent = null) {
			for (int i = 0; i < count; i++) {
				T obj = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], parent);
				obj.gameObject.SetActive(false);
				Queue.Enqueue(obj);
			}
		}

		public void ReturnToPool(T obj) {
			obj.gameObject.SetActive(false);
			Queue.Enqueue(obj);
		}

		public void Clear() {
			var temp = new List<T>(Queue);
			temp.ForEach(o => Object.Destroy(o.gameObject));
			Queue.Clear();
		}
	}
}