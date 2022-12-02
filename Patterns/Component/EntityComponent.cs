using UnityEngine;

namespace AP.Utilities.Patterns
{
	public abstract class EntityComponent : MonoBehaviour { }

	public abstract class EntityComponent<T> : EntityComponent where T : Entity
	{
		protected T Entity { get; private set; }

		protected virtual void Awake() => Entity = GetComponentInParent<T>();
	}
}