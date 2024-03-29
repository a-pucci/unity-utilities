namespace AP.Utilities.Patterns
{
	using System;
	using System.Collections.Generic;

	public static class EventsManager
	{
		public static readonly Dictionary<Event, List<Action<EventData>>> EventActionsMap = new();
		private static readonly Queue<Action> QueueToRemove = new();
		private static bool isInvoking;

		public static void AddAction(Event evt, Action<EventData> action)
		{
			if (isInvoking)
			{
				QueueToRemove.Enqueue(() => AddAction(evt, action));
				return;
			}

			if (!EventActionsMap.TryGetValue(evt, out List<Action<EventData>> actions))
			{
				actions = new List<Action<EventData>> { action };
				EventActionsMap.Add(evt, actions);
			}
			else if(!actions.Contains(action))
				actions.Add(action);
		}
	
		public static void RemoveAction(Event evt, Action<EventData> action)
		{
			if (isInvoking)
			{
				QueueToRemove.Enqueue(() => RemoveAction(evt, action));
				return;
			}
			
			if (EventActionsMap.TryGetValue(evt, out List<Action<EventData>> actions))
				actions.Remove(action);
		}

		public static void RemoveAction(Action<EventData> action)
		{
			if (isInvoking)
			{
				QueueToRemove.Enqueue(() => RemoveAction(action));
				return;
			}
			
			foreach (List<Action<EventData>> actions in EventActionsMap.Values)
				actions.Remove(action);
		}

		public static void RemoveAllActions(Event evt)
		{
			if (isInvoking)
			{
				QueueToRemove.Enqueue(() => RemoveAllActions(evt));
				return;
			}
			if (EventActionsMap.TryGetValue(evt, out List<Action<EventData>> actions))
				actions.Clear();
		}

		public static void Invoke(Event evt, EventData data = null)
		{
			if (!EventActionsMap.TryGetValue(evt, out List<Action<EventData>> actions)) 
				return;

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