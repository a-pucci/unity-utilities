using System;
using System.Collections.Generic;

namespace AP.Utilities.Patterns
{
	public class Event
	{
		public readonly List<Action<EventData>> actions = new();
		private readonly Queue<Action> QueueToRemove = new();
		private bool isInvoking;

		public void Subscribe(Action<EventData> action)
		{
			if (isInvoking)
			{
				QueueToRemove.Enqueue(() => Subscribe(action));
				return;
			}
			
			if(!actions.Contains(action))
				actions.Add(action);
		}
	
		public void Unsubscribe(Action<EventData> action)
		{
			if (isInvoking)
			{
				QueueToRemove.Enqueue(() => Unsubscribe(action));
				return;
			}
			
			actions.Remove(action);
		}

		public void UnsubscribeAll()
		{
			if (isInvoking)
			{
				QueueToRemove.Enqueue(UnsubscribeAll);
				return;
			}
			actions.Clear();
		}

		public void Raise(EventData data = null)
		{
			isInvoking = true;
			
			foreach (Action<EventData> action in actions)
			{
				action(data);
			}
			
			isInvoking = false;

			while (QueueToRemove.Count > 0)
			{
				QueueToRemove.Dequeue().Invoke();
			}
		}
	}
}