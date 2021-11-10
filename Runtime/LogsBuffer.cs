using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CustomLogger
{
    public class LogsBuffer : ILogHandler
    {
        public class LogItem
        {
            public readonly LogType logType;
            private readonly DateTime time;
            public readonly string message;

            public LogItem(LogType logType, string message)
            {
                time = DateTime.Now;
                this.logType = logType;
                this.message = message;
            }

            public override string ToString()
            {
                return string.Format("[{0}][{1}] {2}", time, logType, message);
            }
        }

        public static LogsBuffer Instance { get; private set; }

        public static event Action<LogItem> OnLog;

        private readonly ILogHandler unityLogHandler;

        private LinkedList<LogItem> logsBuffer;
        private bool shouldLogToConsole;
        private LogType consoleLogLevel;
        private int bufferSize;
        private LogType bufferLogLevel;

        private LogsBuffer()
        {
            unityLogHandler = Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = this;
        }

        public LogItem[] Logs { get { return logsBuffer.ToArray(); } }

        public static void Init(Config.ILoggerConfig logerConfig)
        {
            if (Instance == null)
            {
                Instance = new LogsBuffer
                {
                    logsBuffer = new LinkedList<LogItem>()
                };
            }

            if (logerConfig != null)
            {
                Instance.shouldLogToConsole = logerConfig.ShouldLogToConsole;
                Instance.consoleLogLevel = logerConfig.ConsoleLogLevel;
                Instance.bufferSize = logerConfig.BufferSize;
                Instance.bufferLogLevel = logerConfig.BufferLogLevel;
            }
            else
            {
                Instance.shouldLogToConsole = true;
                Instance.consoleLogLevel = LogType.Log;
                Instance.bufferSize = 1000;
                Instance.bufferLogLevel = LogType.Log;
            }
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            var logItem = new LogItem(LogType.Exception, string.Format("{0}\n{1}", exception.Message, exception.StackTrace));

            lock (logsBuffer)
            {
                logsBuffer.AddLast(logItem);
            }

            RemoveOldLogsFromBuffer();
            OnLog?.Invoke(logItem);

            if (shouldLogToConsole)
            {
                unityLogHandler.LogException(exception, context);
            }
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            if (logType <= bufferLogLevel)
            {
                var logItem = new LogItem(logType, string.Format(format, args));

                lock (logsBuffer)
                {
                    logsBuffer.AddLast(logItem);
                }

                RemoveOldLogsFromBuffer();
                OnLog?.Invoke(logItem);
            }

            if (shouldLogToConsole && logType <= consoleLogLevel)
            {
                unityLogHandler.LogFormat(logType, context, format, args);
            }
        }

        public void ClearBufferedLogs()
        {
            lock (logsBuffer)
            {
                logsBuffer.Clear();
            }
        }

        private void RemoveOldLogsFromBuffer()
        {
            lock (logsBuffer)
            {
                while (logsBuffer.Count > bufferSize)
                {
                    logsBuffer.RemoveFirst();
                }
            }
        }
    }
}