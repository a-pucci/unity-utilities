namespace AP.Utilities.Patterns
{
	public class EventData
	{
		private readonly object Data;

		public EventData(object data) => Data = data;
		
		public T GetData<T>() => (Data is T tData) ? tData : default;
		
		public bool TryGetData<T>(out T outData)
		{
			outData = default;
			
			if (Data is not T tData)
				return false;
			
			outData = tData;
			return true;
		}
	}
}