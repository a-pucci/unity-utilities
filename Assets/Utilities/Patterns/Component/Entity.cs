using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Patterns {
	public abstract class Entity : MonoBehaviour {
		public readonly Dictionary<Type, EntityComponent> components = new Dictionary<Type, EntityComponent>();

		public T Get<T>() where T : EntityComponent => (T)components[typeof(T)];

		protected void Awake() => GetComponents();

		private void GetComponents() {
			var comps = GetComponents<EntityComponent>();
			foreach (EntityComponent component in comps) {
				components.Add(component.GetType(), component);
			}
		}
	}
}