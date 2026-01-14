using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AP.Logging
{
	[Serializable]
	public struct LoggerSetting
	{
		public string typeName;
		public string assemblyName;
		public string LoggerName => assemblyName + "." + typeName;

		public bool isEnabled;
	}
	
	[CreateAssetMenu(fileName = "logs_settings", menuName = "AP/Log/Log Settings", order = 1)]
	public partial class LogSettings : ScriptableObject
	{
		[SerializeField] protected Log.Level logLevel;
		public Log.Level LogLevel => logLevel;

		[SerializeField] private bool enableLogs;
		public bool EnableLogs => enableLogs;

		[SerializeField] protected List<LoggerSetting> loggers = new List<LoggerSetting>();
		public IReadOnlyList<LoggerSetting> Loggers => loggers.AsReadOnly();
	}
}