#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define LOGS_ENABLED
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;


namespace AP.Logging
{
    public interface ILogger { }

    public static class Log
    {
        const string LogsDefine = "LOGS_ENABLED";

        public enum Level
        {
            /// <summary>Nessun log.</summary>
            None = 0,
            /// <summary>Errore critico che può causare arresto dell'applicazione.</summary>
            Critical = 1,
            /// <summary>Errore che impedisce il funzionamento corretto di una funzionalità.</summary>
            Error = 2,
            /// <summary>Situazioni non critiche che potrebbero richiedere attenzione.</summary>
            Warning = 3,
            /// <summary>Informazioni generali su eventi normali.</summary>
            Info = 4,
            /// <summary>Informazioni di debug durante lo sviluppo.</summary>
            Debug = 5
        }

        [Serializable]
        private class LogEntry
        {
            public string timestamp;
            public string level;
            public string message;
            public string caller;
            public string file;
            public int line;
        }

        private static LogSettings settings;

        private static Dictionary<Type, bool> loggers = new Dictionary<Type, bool>();

        private static string TimeStamp => DateTime.Now.ToString("HH:mm:ss.fff");

        private static Type CallerType
        {
            get
            {
                var stack = new StackTrace();
                StackFrame frame = stack.GetFrame(2);
                MethodBase method = frame.GetMethod();
                return method.DeclaringType;
            }
        }

        public static void Initialize(LogSettings logsSettings = null)
        {
            settings = logsSettings ?? Resources.Load<LogSettings>("logs_settings");

            Assert.IsNotNull(settings);
            loggers = new Dictionary<Type, bool>();

            // Initialize loggers based on settings
            foreach (LoggerSetting loggerSetting in settings.Loggers)
            {
                var loggerType = Type.GetType($"{loggerSetting.typeName}, {loggerSetting.assemblyName}");
                if (loggerType != null)
                    loggers.Add(loggerType, loggerSetting.isEnabled);
            }
        }

        public static bool CanTypeLog(ILogger logger) => CanTypeLog(logger.GetType());

        public static bool CanTypeLog(Type loggerType) => loggers.TryGetValue(loggerType, out bool value) && value;

        public static bool CanLevelLog(Level level) => settings != null && level <= settings.LogLevel;

        [Conditional(LogsDefine)]
        public static void Debug(string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFile = "",
            [CallerLineNumber] int callerLine = 0)
        {
            if (!CanTypeLog(CallerType) || !CanLevelLog(Level.Debug))
                return;

            LogInternal(Level.Debug, message, callerName, callerFile, callerLine);
        }

        [Conditional(LogsDefine)]
        public static void Info(string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFile = "",
            [CallerLineNumber] int callerLine = 0)
        {
            if (!CanTypeLog(CallerType) || !CanLevelLog(Level.Info))
                return;

            LogInternal(Level.Info, message, callerName, callerFile, callerLine);
        }

        [Conditional(LogsDefine)]
        public static void Warning(string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFile = "",
            [CallerLineNumber] int callerLine = 0)
        {
            if (!CanTypeLog(CallerType) || !CanLevelLog(Level.Warning))
                return;

            LogInternal(Level.Warning, message, callerName, callerFile, callerLine);
        }

        [Conditional(LogsDefine)]
        public static void Error(string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFile = "",
            [CallerLineNumber] int callerLine = 0)
        {
            if (!CanTypeLog(CallerType) || !CanLevelLog(Level.Error))
                return;

            LogInternal(Level.Error, message, callerName, callerFile, callerLine);
        }

        [Conditional(LogsDefine)]
        public static void Critical(string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFile = "",
            [CallerLineNumber] int callerLine = 0)
        {
            if (!CanTypeLog(CallerType) || !CanLevelLog(Level.Critical))
                return;

            LogInternal(Level.Critical, message, callerName, callerFile, callerLine);
        }

        private static void LogInternal(Level level, string message, string callerName, string callerFile, int callerLine)
        {
            string log = FormatHuman(level, message, callerName, callerFile, callerLine);

            switch (level)
            {
                case Level.Error:
                case Level.Critical:
                    UnityEngine.Debug.LogError(log);
                    break;

                case Level.Warning:
                    UnityEngine.Debug.LogWarning(log);
                    break;

                case Level.Info:
                case Level.Debug:
                case Level.None:
                default:
                    UnityEngine.Debug.Log(log);
                    break;
            }
        }

        private static string FormatHuman(Level level, string message, string callerName, string callerFile, int callerLine)
        {
            var sb = new StringBuilder();
#if !UNITY_EDITOR
        sb.Append($"{DateTime.Now:HH:mm:ss.fff} | ");
#endif
            sb.Append($"{GetLevelLabel(level)} | {message} ({callerName} in {Path.GetFileName(callerFile)}:{callerLine})");
            return sb.ToString();
        }

        private static string GetLevelLabel(Level level)
        {
#if !UNITY_EDITOR
        return level switch
        {
            Level.Critical => "❗️ CRT",
            Level.Error => "⛔ ERR",
            Level.Warning => "⚠️ WRN",
            Level.Info => "💬 INF",
            Level.Debug => "🐞 DBG",
            _ => "—   NON"
        };
#else
            return level switch
            {
                Level.Critical => "CRT",
                Level.Error => "ERR",
                Level.Warning => "WRN",
                Level.Info => "INF",
                Level.Debug => "DBG",
                _ => "NON"
            };
#endif
        }

        private static string FormatJson(Level level, string message, string callerName, string callerFile, int callerLine) =>
            JsonUtility.ToJson(new LogEntry
            {
                timestamp = DateTime.UtcNow.ToString("o"), // ISO 8601 UTC
                level = level.ToString(),
                message = message ?? string.Empty,
                caller = callerName ?? string.Empty,
                file = Path.GetFileName(callerFile),
                line = callerLine
            });

        private static string GetThreadInfo()
        {
            var t = Thread.CurrentThread;
            return string.IsNullOrEmpty(t.Name)
                ? $"tid:{t.ManagedThreadId}"
                : $"tid:{t.ManagedThreadId}/{t.Name}";
        }
    }
}