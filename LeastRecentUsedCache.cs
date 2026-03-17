using System;
using System.Collections.Generic;

namespace AP.Utilities
{
	public class LeastRecentUsedCache<TKey, TValue>
	{
		private class CacheItem
		{
			public TKey Key { get; }
			public TValue Value { get; }

			public CacheItem(TKey key, TValue value)
			{
				Key = key;
				Value = value;
			}
		}

		private readonly int capacity;
		private readonly Dictionary<TKey, LinkedListNode<CacheItem>> cacheMap = new();
		private readonly LinkedList<CacheItem> lruList = new();
		private readonly Action<TValue> onEvict;

		public LeastRecentUsedCache(int capacity, Action<TValue> onEvict = null)
		{
			this.capacity = capacity;
			this.onEvict = onEvict;
		}

		public TValue Get(TKey key)
		{
			if (cacheMap.TryGetValue(key, out LinkedListNode<CacheItem> node))
			{
				lruList.Remove(node);
				lruList.AddFirst(node);
				return node.Value.Value;
			}

			return default;
		}

		public bool TryGet(TKey key, out TValue value)
		{
			value = default;

			if (cacheMap.TryGetValue(key, out LinkedListNode<CacheItem> node))
			{
				lruList.Remove(node);
				lruList.AddFirst(node);
				value = node.Value.Value;
				return true;
			}

			return false;
		}

		public bool Contains(TKey key) => cacheMap.ContainsKey(key);

		public void Add(TKey key, TValue value)
		{
			if (cacheMap.TryGetValue(key, out LinkedListNode<CacheItem> existingNode))
			{
				lruList.Remove(existingNode);
				onEvict?.Invoke(existingNode.Value.Value);
				existingNode.Value = new CacheItem(key, value);
				lruList.AddFirst(existingNode);
				return;
			}

			if (cacheMap.Count >= capacity)
			{
				LinkedListNode<CacheItem> lastNode = lruList.Last;

				if (lastNode != null)
				{
					cacheMap.Remove(lastNode.Value.Key);
					lruList.RemoveLast();

					if (lastNode.Value.Value != null)
						onEvict?.Invoke(lastNode.Value.Value);
				}
			}

			var newItem = new CacheItem(key, value);
			var newNode = new LinkedListNode<CacheItem>(newItem);

			lruList.AddFirst(newNode);
			cacheMap.TryAdd(key, newNode);
		}

		public void Clear()
		{
			foreach (CacheItem node in lruList)
			{
				if (node.Value != null)
					onEvict?.Invoke(node.Value);
			}

			cacheMap.Clear();
			lruList.Clear();
		}
	}
}