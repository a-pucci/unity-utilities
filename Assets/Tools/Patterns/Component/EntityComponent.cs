using UnityEngine;

namespace Tools.Patterns {
	public abstract class EntityComponent : MonoBehaviour {
		public virtual void EnableInput() { }
		public virtual void DisableInput() { }
		public virtual void SubscribeEvents() { }
		public virtual void UnsubscribeEvents() { }
	}

	public abstract class EntityComponent<T> : EntityComponent where T : Entity {
		protected T Entity { get; private set; }

		protected virtual void Awake() {
			Entity = GetComponent<T>();
			Debug.Assert(Entity != null, $"No entity found of type {typeof(T)}", this);
		}

	}
}