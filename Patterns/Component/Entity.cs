using System;
using System.Collections.Generic;
using UnityEngine;

namespace AP.Utilities.Patterns
{
	public abstract class Entity : MonoBehaviour
	{
		private Dictionary<Type, EntityComponent> components = new Dictionary<Type, EntityComponent>();
		private bool initialized;
		
		public T Get<T>() where T : EntityComponent
		{
			if (!initialized) GetComponents();
			return components.TryGetValue(typeof(T), out EntityComponent value) ? value as T : null;
		}

		protected virtual void Awake() => GetComponents();

		private void GetComponents()
		{
			if (initialized)
				return;
			
			components = new Dictionary<Type, EntityComponent>();
			EntityComponent[] parts = GetComponentsInChildren<EntityComponent>();
			foreach (EntityComponent part in parts)
			{
				components.Add(part.GetType(), part);
			}
			initialized = true;
		}
	}
}