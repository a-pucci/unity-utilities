using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utilities.Patterns {
	public class GameObjectPools : MonoBehaviour {
		[Serializable]
		public struct PoolStruct {
			public int size;
			public GameObject gameObject;
		}

		public static GameObjectPools Instance { get; private set; }
		
#pragma warning disable 0649
		[SerializeField] private PoolStruct[] objects;
#pragma warning restore 0649

		private Dictionary<string, GameObjectPool> pools;
		private Transform parent;

		private void Awake() {
			parent = new GameObject("Pool").transform;

			Instance = this;
			pools = new Dictionary<string, GameObjectPool>();
			foreach (var currentGameObject in objects) {
				pools.Add(currentGameObject.gameObject.name, new GameObjectPool(currentGameObject.gameObject, currentGameObject.size, parent));
			}
		}

		public void AddToPool(GameObject go, int size) {
			pools.Add(go.name, new GameObjectPool(go, size, parent));
		}

		public GameObject Get(GameObject originalGameObject) {
			return pools[originalGameObject.name].Get();
		}

		public GameObject Get(string prefabName) {
			if (!pools.ContainsKey(prefabName)) {
				Debug.Log($"Missing '{prefabName}' in pool.");
			}
			return pools[prefabName].Get();
		}

		public void ReturnToPool(GameObject obj) {
			obj.transform.SetParent(parent);
			var pool = pools[obj.name];
			pool.ReturnToPool(obj);
		}
	}
}