using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Patterns {
	public abstract class Entity : MonoBehaviour {
		public readonly Dictionary<Type, EntityComponent> components = new Dictionary<Type, EntityComponent>();

		public T Get<T>() where T : EntityComponent {
			return (T)components[typeof(T)];
		}
	}
}