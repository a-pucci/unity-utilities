using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Patterns {
	public class Pools<T> : MonoBehaviour where T : MonoBehaviour {
		public struct PoolStruct<T1> where T1 : MonoBehaviour {
			public int size;
			public T1 item;
		}

		public static Pools<T> Instance { get; private set; }

		private PoolStruct<T>[] items = default;

		private Dictionary<string, Pool<T>> pools;
		private Transform parent;

		private void Awake() {
			parent = new GameObject("Pools").transform;
			parent.SetParent(transform);
			Instance = this;
			pools = new Dictionary<string, Pool<T>>();
			foreach (PoolStruct<T> poolStruct in items) {
				pools.Add(poolStruct.item.gameObject.name, new Pool<T>(poolStruct.item, poolStruct.size, parent));
			}
		}

		public void AddToPool(T go, int size) {
			pools.Add(go.name, new Pool<T>(go, size, parent));
			if (pools.ContainsKey(go.name)) {
				pools[go.name].AddToPool(size, parent);
			}
			else {
				pools.Add(go.name, new Pool<T>(go, size, parent));
			}
		}

		public T Get(T originalGameObject) {
			return pools[originalGameObject.name].Get();
		}

		public T Get(string prefabName) {
			if (!pools.ContainsKey(prefabName)) {
				Debug.Log($"Missing '{prefabName}' in pool.");
			}
			return pools[prefabName].Get();
		}

		public void ReturnToPool(T obj) {
			obj.transform.SetParent(parent);
			var pool = pools[obj.name];
			pool.ReturnToPool(obj);
		}
	}

}