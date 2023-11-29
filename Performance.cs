using System;
using System.Diagnostics;

namespace AP.Utilities
{
	public static class Performance
	{
		private class MeasureOperation : IDisposable
		{
			private readonly Stopwatch stopwatch = Stopwatch.StartNew();

			public void Dispose()
			{
				stopwatch.Stop();
				UnityEngine.Debug.Log($"Operation Take {stopwatch.ElapsedMilliseconds} ms");
			}
		}

		public static void MeasureMethod(Action action)
		{
			using var mo = new MeasureOperation();
			action.Invoke();
		}
	}
}