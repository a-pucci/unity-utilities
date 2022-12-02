using System;
using System.Collections.Generic;
using UnityEngine;

namespace AP.Utilities.Patterns
{
	public abstract class Entity : MonoBehaviour
	{
		private readonly Dictionary<Type, EntityComponent> components = new Dictionary<Type, EntityComponent>();

		public T Get<T>() where T : EntityComponent => components.TryGetValue(typeof(T), out EntityComponent value) ? value as T : null;

		protected virtual void Awake() => GetComponents();

		private void GetComponents()
		{
			EntityComponent[] comps = GetComponentsInChildren<EntityComponent>();

			foreach (EntityComponent component in comps)
			{
				components.Add(component.GetType(), component);
			}
		}
	}
}