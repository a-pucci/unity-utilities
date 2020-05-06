using System.Collections.Generic;
using UnityEngine;

namespace Tools.Patterns {
	public class GameObjectPool {
		private Queue<GameObject> Queue { get; } = new Queue<GameObject>();
		private readonly GameObject prefab;

		public GameObjectPool(GameObject prefab, int poolSize, Transform parent = null) {
			this.prefab = prefab;
			AddToPool(prefab.name, poolSize, parent);
		}

		public GameObject Get() {
			if (Queue.Count == 0) {
				AddToPool(prefab.name, 1);
			}
			return Queue.Dequeue();
		}
	
		private void AddToPool(string prefabName, int count, Transform parent = null) {
			for (int i = 0; i < count; i++) {
				GameObject obj = Object.Instantiate(prefab, parent);
				obj.name = prefabName;
				obj.gameObject.SetActive(false);
				Queue.Enqueue(obj);
			}
		}

		public void ReturnToPool(GameObject obj) {
			obj.gameObject.SetActive(false);
			Queue.Enqueue(obj);
		}

		public void Clear() {
			var temp = new List<GameObject>(Queue);
			temp.ForEach(o => Object.Destroy(o.gameObject));
			Queue.Clear();
		}
	}
}