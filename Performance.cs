using System;
using System.Diagnostics;

namespace AP.Utilities
{
	public static class Performance
	{
		public static IDisposable MeasureMethod() => new Method();

		private class Method : IDisposable
		{
			private readonly Stopwatch stopwatch = Stopwatch.StartNew();

			public void Dispose()
			{
				stopwatch.Stop();
				UnityEngine.Debug.Log($"Operation took {stopwatch.ElapsedMilliseconds} ms");
			}
		}
	}
}