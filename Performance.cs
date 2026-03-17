using System;
using System.Diagnostics;

namespace AP.Utilities
{
	public static class Performance
	{
		public static IDisposable StartMeasure(string methodName = "") => new MethodMeasure(methodName);

		private class MethodMeasure : IDisposable
		{
			private readonly Stopwatch stopwatch = Stopwatch.StartNew();
			private readonly string methodName;

			public MethodMeasure(string methodName = "") => this.methodName = methodName;

			public void Dispose()
			{
				stopwatch.Stop();
				UnityEngine.Debug.Log($"{(methodName == "" ? "Operation" : methodName)} took {stopwatch.ElapsedMilliseconds} ms");
			}
		}
	}
}