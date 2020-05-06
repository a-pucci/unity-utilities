using UnityEngine;

namespace Utilities.Patterns {
	public abstract class EntityComponent : MonoBehaviour { }

	public abstract class EntityComponent<T> : EntityComponent where T : Entity {
		protected T Entity { get; private set; }

		protected virtual void Awake() {
			Entity = GetComponent<T>();
			Debug.Assert(Entity != null, $"No entity found of type {typeof(T)}", this);
		}

	}
}