using System;
using System.Collections.Generic;
using UnityEngine;

namespace AP.Utilities.Patterns {
	public class Pools : MonoBehaviour {
		[Serializable]
		private struct PoolData {
			public int size;
			public MonoBehaviour item;
		}

		public static Pools Instance { get; private set; }

		[SerializeField] private PoolData[] items;

		private Dictionary<string, Pool<MonoBehaviour>> pools;
		private Transform parent;

		private void Awake() {
			parent = new GameObject("Pools").transform;
			parent.SetParent(transform);
			Instance = this;
			pools = new Dictionary<string, Pool<MonoBehaviour>>();
			foreach (PoolData data in items) {
				pools.Add(data.item.gameObject.name, new Pool<MonoBehaviour>(data.item, data.size, parent));
			}
		}

		public void AddToPool(MonoBehaviour go, int size) {
			pools.Add(go.name, new Pool<MonoBehaviour>(go, size, parent));
			if (pools.ContainsKey(go.name)) {
				pools[go.name].AddToPool(size, parent);
			}
			else {
				pools.Add(go.name, new Pool<MonoBehaviour>(go, size, parent));
			}
		}

		public MonoBehaviour Get(MonoBehaviour originalGameObject) {
			if (!pools.ContainsKey(originalGameObject.name))
				return null;
			
			return pools[originalGameObject.name].Get();
		}

		public MonoBehaviour Get(string prefabName) {
			if (!pools.ContainsKey(prefabName))
				return null;
			
			return pools[prefabName].Get();
		}

		public void Release(MonoBehaviour obj) {
			obj.transform.SetParent(parent);
			var pool = pools[obj.name];
			pool.Release(obj);
		}
	}
}