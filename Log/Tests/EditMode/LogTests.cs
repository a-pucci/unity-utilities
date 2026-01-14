using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AP.Logging
{
	public class LogTests
	{
		private TestSettings settings;
		TestLogger1 loggerEnabled;
		TestLogger2 loggerDisabled;

		[OneTimeSetUp]
		public void Setup()
		{
			settings = ScriptableObject.CreateInstance<TestSettings>();
			loggerEnabled = new TestLogger1();
			loggerDisabled = new TestLogger2();
			var loggerSettings = new List<LoggerSetting>
			{
				new() { typeName = typeof(TestLogger1).FullName, assemblyName = typeof(TestLogger1).Assembly.GetName().Name, isEnabled = true },
				new() { typeName = typeof(TestLogger2).FullName, assemblyName = typeof(TestLogger2).Assembly.GetName().Name, isEnabled = false }
			};

			settings.SetLoggers(loggerSettings);
			settings.SetLevel(Log.Level.Debug);
			Log.Initialize(settings);
		}

		[OneTimeTearDown]
		public void TearDown() => Object.DestroyImmediate(settings);

		[Test]
		public void CheckTypeCanLog() => Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && !Log.CanTypeLog(loggerDisabled));

		[Test]
		public void CheckLevelDebug()
		{
			LogAssert.ignoreFailingMessages = true;
			settings.SetLevel(Log.Level.Debug);
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Debug), "Debug");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Info), "Info");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Warning), "Warning");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Error), "Error");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Critical), "Critical");
			LogAssert.ignoreFailingMessages = false;
		}

		[Test]
		public void CheckLevelInfo()
		{
			LogAssert.ignoreFailingMessages = true;
			settings.SetLevel(Log.Level.Info);
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Debug), "Debug");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Info), "Info");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Warning), "Warning");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Error), "Error");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Critical), "Critical");
			LogAssert.ignoreFailingMessages = false;
		}

		[Test]
		public void CheckLevelWarning()
		{
			LogAssert.ignoreFailingMessages = true;
			settings.SetLevel(Log.Level.Warning);
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Debug), "Debug");
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Info), "Info");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Warning), "Warning");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Error), "Error");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Critical), "Critical");
			LogAssert.ignoreFailingMessages = false;
		}

		[Test]
		public void CheckLevelError()
		{
			LogAssert.ignoreFailingMessages = true;
			settings.SetLevel(Log.Level.Error);
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Debug), "Debug");
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Info), "Info");
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Warning), "Warning");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Error), "Error");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Critical), "Critical");
			LogAssert.ignoreFailingMessages = false;
		}

		[Test]
		public void CheckLevelCritical()
		{
			LogAssert.ignoreFailingMessages = true;
			settings.SetLevel(Log.Level.Critical);
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Debug), "Debug");
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Info), "Info");
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Warning), "Warning");
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Error), "Error");
			Assert.IsTrue(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Critical), "Critical");
			LogAssert.ignoreFailingMessages = false;
		}

		[Test]
		public void CheckLevelNone()
		{
			LogAssert.ignoreFailingMessages = true;
			settings.SetLevel(Log.Level.None);
			if (!Log.CanTypeLog(loggerEnabled) || !Log.CanLevelLog(Log.Level.Debug))
				return;

			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Debug), "Debug");
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Info), "Info");
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Warning), "Warning");
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Error), "Error");
			Assert.IsFalse(Log.CanTypeLog(loggerEnabled) && Log.CanLevelLog(Log.Level.Critical), "Critical");
			LogAssert.ignoreFailingMessages = false;
		}

		private class TestSettings : LogSettings
		{
			public void SetLoggers(List<LoggerSetting> settings) => loggers = settings;

			public void SetLevel(Log.Level level) => logLevel = level;
		}

		private class TestLogger1 : ILogger
		{
			public void LogDebug() => Log.Debug("TEST 1");

			public void LogInfo() => Log.Info("TEST 1");

			public void LogWarning() => Log.Warning("TEST 1");

			public void LogError() => Log.Error("TEST 1");

			public void LogCritical() => Log.Critical("TEST 1");
		}

		private class TestLogger2 : ILogger
		{
			public void LogDebug() => Log.Info("TEST 2");
		}
	}

	public class TestLogger3 : ILogger
	{
		public class TestLogger4 : ILogger
		{
			public class TestLogger5 : ILogger { }
		}

		public class TestLogger6 : ILogger { }
	}

	public class TestLogger6 : ILogger { }
}
